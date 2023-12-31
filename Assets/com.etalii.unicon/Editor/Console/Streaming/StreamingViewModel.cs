namespace EtAlii.UniCon.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;
    using Serilog.Events;
    using UniRx;
    using UnityEngine.UIElements;

    public partial class StreamingViewModel
    {
        public ReactiveProperty<IObservable<LogEvent>> Stream = new();
        private ReplaySubject<LogEvent> _subject;
        
        public readonly ReactiveCommand<ClickEvent> ToggleScrollToTail = new();
        
        private FiltersViewModel _filtersViewModel;
        private ExpressionViewModel _expressionViewModel;
        private CompositeDisposable _pipeline;
        private IDisposable _subscription;

        public void Bind(FiltersViewModel filtersViewModel, ExpressionViewModel expressionViewModel)
        {
            _filtersViewModel = filtersViewModel;
            _expressionViewModel = expressionViewModel;
            ToggleScrollToTail.Subscribe(_ =>
            {
                UserSettings.instance.ScrollToTail.Value = !UserSettings.instance.ScrollToTail.Value;
            });
        }
        
        /// <summary>
        /// Reconfigure the stream with the right filtering rules applied.
        /// </summary>
        public void ConfigureStream()
        {
            _pipeline?.Dispose();
            _subscription?.Dispose();

            var blockOptions = new ExecutionDataflowBlockOptions
            {
                TaskScheduler = TaskScheduler.Default, 
                EnsureOrdered = false,
                MaxDegreeOfParallelism = Environment.ProcessorCount * 2,
            };
            var linkOptions = new DataflowLinkOptions { PropagateCompletion = false };

            _subject = new ReplaySubject<LogEvent>();

            var index = 0;
            var input = new BufferBlock<PipelineItem>(blockOptions);

            var filterBySource = new TransformBlock<PipelineItem, PipelineItem>(FilterBySource, blockOptions);
            var filterByLogLevel = new TransformBlock<PipelineItem, PipelineItem>(FilterByLogLevel, blockOptions);
            var filterByCustomFilter = new TransformBlock<PipelineItem, PipelineItem>(FilterByCustomFilter, blockOptions);
            var filterByExpression = new TransformBlock<PipelineItem, PipelineItem>(FilterByExpression, blockOptions);
            var sortEvents = CreateRestoreOrderBlock<PipelineItem>(item => item.Index);
            
            var output = new ActionBlock<PipelineItem>(OutputLogEvent);

            _pipeline = new CompositeDisposable
            (
                input.LinkTo(filterBySource, linkOptions),
                filterBySource.LinkTo(filterByLogLevel, linkOptions),
                filterByLogLevel.LinkTo(filterByCustomFilter, linkOptions),
                filterByCustomFilter.LinkTo(filterByExpression, linkOptions),
                filterByExpression.LinkTo(sortEvents, linkOptions),
                sortEvents.LinkTo(output)
            );
            
            _subscription = LogSink.Instance
                .Observe()
                //.SubscribeOnMainThread()
                .ObserveOn(Scheduler.ThreadPool)
                .SubscribeOn(Scheduler.ThreadPool)
                .Subscribe(logEvent => input.Post(new PipelineItem { Index = index++, LogEvent = logEvent }));
            
            Stream.Value = _subject.AsObservable();
        }

        private bool LogFilterIsValid(LogFilter rule, LogEvent logEvent)
        {
            var result = rule.CompiledExpression.Value?.Invoke(logEvent);
            if (result is not ScalarValue scalarValue)
            {
                return false;
            }

            return scalarValue.Value is not false;
        }
        
        /// <summary>Creates a dataflow block that restores the order of
        /// a shuffled pipeline.</summary>
        private static IPropagatorBlock<T, T> CreateRestoreOrderBlock<T>(
            Func<T, long> indexSelector,
            long startingIndex = 0L,
            DataflowBlockOptions options = null)
        {
            if (indexSelector == null) throw new ArgumentNullException(nameof(indexSelector));
            var executionOptions = new ExecutionDataflowBlockOptions();
            if (options != null)
            {
                executionOptions.CancellationToken = options.CancellationToken;
                executionOptions.BoundedCapacity = options.BoundedCapacity;
                executionOptions.EnsureOrdered = options.EnsureOrdered;
                executionOptions.TaskScheduler = options.TaskScheduler;
                executionOptions.MaxMessagesPerTask = options.MaxMessagesPerTask;
                executionOptions.NameFormat = options.NameFormat;
            }

            var buffer = new Dictionary<long, T>();
            var minIndex = startingIndex;

            IEnumerable<T> Transform(T item)
            {
                // No synchronization needed because MaxDegreeOfParallelism = 1
                var index = indexSelector(item);
                if (index < startingIndex)
                    throw new InvalidOperationException($"Index {index} is out of range.");
                if (index < minIndex)
                    throw new InvalidOperationException($"Index {index} has been consumed.");
                if (!buffer.TryAdd(index, item)) // .NET Core only API
                    throw new InvalidOperationException($"Index {index} is not unique.");
                while (buffer.Remove(minIndex, out var minItem)) // .NET Core only API
                {
                    minIndex++;
                    yield return minItem;
                }
            }

            // Ideally the assertion buffer.Count == 0 should be checked on the completion
            // of the block.
            return new TransformManyBlock<T, T>(i => Transform(i), executionOptions);
        }
    }
}
