namespace EtAlii.UniCon.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;
    using EtAlii.Unicon;
    using Serilog.Events;
    using UniRx;

    public partial class DataStreamer : IDisposable
    {
        public readonly ReactiveProperty<(LogEntryList List, LinkedList<LogEntry> LinkedList, bool HasAdditions, long index)> ForwardData = new();
        public readonly ReactiveProperty<(LogEntryList List, LinkedList<LogEntry> LinkedList, bool HasAdditions, long index)> BackwardData = new();
        
        private ExpressionViewModel _expressionViewModel;
        private CompositeDisposable _forwardPipeline;
        private CompositeDisposable _backwardPipeline;
        private readonly LinkedList<LogEntry> _linkedList;
        private readonly LogEntryList _list;

        public DataStreamer()
        {
            _linkedList = new LinkedList<LogEntry>();
            _list = new LogEntryList(_linkedList);
        }
        public void Bind(ExpressionViewModel expressionViewModel)
        {
            _expressionViewModel = expressionViewModel;
        }
        /// <summary>
        /// Reconfigure the stream with the right filtering rules applied.
        /// The hard variant clears the current items and starts from scratch.
        /// </summary>
        public void ConfigureHard()
        {
            _forwardPipeline?.Dispose();
            _backwardPipeline?.Dispose();

            _linkedList.Clear();
            var handleForwardEntry = new Action<LogEntry>(entry => 
            {
                _linkedList.AddLast(entry);
                ForwardData.Value = new(_list, _linkedList, true, entry.Index);
            });
            CreatePipeline(LogSink.Instance.ObserveForward(0), handleForwardEntry, out _forwardPipeline);
            
            var handleBackwardEntry = new Action<LogEntry>(entry => 
            {
                _linkedList.AddFirst(entry);
                BackwardData.Value = new(_list, _linkedList, true, entry.Index);
            });
            CreatePipeline(LogSink.Instance.ObserveBackward(0), handleBackwardEntry, out _backwardPipeline);
        }

        private void CreatePipeline(IObservable<LogEntry> directionalStream, Action<LogEntry> handleEntry, out CompositeDisposable pipeline)
        {
            var blockOptions = new ExecutionDataflowBlockOptions
            {
                TaskScheduler = TaskScheduler.Default,
                EnsureOrdered = false,
                MaxDegreeOfParallelism = Environment.ProcessorCount * 2,
            };
            var linkOptions = new DataflowLinkOptions { PropagateCompletion = false };

            var input = new BufferBlock<LogEntry>(blockOptions);

            var filterBySource = new TransformBlock<LogEntry, LogEntry>(FilterBySource, blockOptions);
            var filterByLogLevel = new TransformBlock<LogEntry, LogEntry>(FilterByLogLevel, blockOptions);
            var filterByCustomFilter = new TransformBlock<LogEntry, LogEntry>(FilterByCustomFilter, blockOptions);
            var filterByExpression = new TransformBlock<LogEntry, LogEntry>(FilterByExpression, blockOptions);
            var sortEvents = CreateRestoreOrderBlock<LogEntry>(item => item.Index);

            var output = new ActionBlock<LogEntry>(entry => { if (entry.LogEvent != null) handleEntry(entry); });
            
            pipeline = new CompositeDisposable
            (
                input.LinkTo(filterBySource, linkOptions),
                filterBySource.LinkTo(filterByLogLevel, linkOptions),
                filterByLogLevel.LinkTo(filterByCustomFilter, linkOptions),
                filterByCustomFilter.LinkTo(filterByExpression, linkOptions),
                filterByExpression.LinkTo(sortEvents, linkOptions),
                sortEvents.LinkTo(output)
            );
            
            var forwardSubscription = directionalStream
                .ObserveOn(Scheduler.ThreadPool)
                .SubscribeOn(Scheduler.ThreadPool)
                .Subscribe(logEntry => input.Post(logEntry));
            pipeline.Add(forwardSubscription);
        }

        // ReSharper disable once InconsistentNaming
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

        public void Dispose()
        {
            _forwardPipeline?.Dispose();
            _backwardPipeline?.Dispose();
        }
    }
}
