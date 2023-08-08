namespace EtAlii.UniCon.Editor
{
    using System;
    using System.Collections.Generic;
    using Serilog.Events;
    using UniRx;
    using UnityEngine.UIElements;

    public class ExpressionViewModel
    {
        public LogFilter ExpressionFilter { get; private set; }
        public readonly ReactiveProperty<bool> HasCompiledExpression = new();
        public readonly ReactiveProperty<string> ExpressionError = new();

        public readonly ReactiveCommand<ClickEvent> ToggleExpressionPanel = new();
        public readonly ReactiveProperty<string> ExpressionText = new();

        // Log level.
        public readonly ReactiveCommand<LogEventLevel> AddFindByLogLevelToExpression = new();
        public readonly ReactiveCommand<LogEventLevel> AddExcludeByLogLevelToExpression = new();
        
        // Properties.
        public readonly ReactiveCommand<KeyValuePair<string, LogEventPropertyValue>> AddFindByPropertyToExpression = new();
        public readonly ReactiveCommand<string> AddFindWithAnyPropertyValueToExpression = new();
        public readonly ReactiveCommand<string> AddExcludeWithAnyPropertyValueToExpression = new();
        public readonly ReactiveCommand<KeyValuePair<string, LogEventPropertyValue>> AddExcludeByPropertyToExpression = new();

        // Event type.
        public readonly ReactiveCommand<uint> AddFindByEventTypeToExpression = new();
        public readonly ReactiveCommand<uint> AddExcludeByEventTypeToExpression = new();

        // Time
        public readonly ReactiveCommand<(DateTimeOffset, TimeSpan)> AddSeekToTimeSpanToExpression = new();
        
        private readonly DataWindowStreamer _dataWindowStreamer;

        public ExpressionViewModel(DataWindowStreamer dataWindowStreamer)
        {
            _dataWindowStreamer = dataWindowStreamer;
        }
        
        public void Bind()
        {
            ExpressionFilter = new();
            ExpressionFilter.Bind();
            
            ToggleExpressionPanel
                .Subscribe(_ =>
                {
                    UserSettings.instance.ShowExpressionPanel.Value = !UserSettings.instance.ShowExpressionPanel.Value;
                    UserSettings.instance.ExpressionPanelHeight.Value = UserSettings.instance.ExpressionPanelHeight.Value;
                });

            ExpressionFilter.IsActive.Subscribe(_ => _dataWindowStreamer.Configure());
            ExpressionFilter.CompiledExpression.Subscribe(_ => _dataWindowStreamer.Configure());
            
            ExpressionText.Subscribe(text =>
            {
                ExpressionFilter.Expression.Value = text;
                HasCompiledExpression.Value = ExpressionFilter.CompiledExpression.Value != null;
                ExpressionError.Value = ExpressionFilter.Error;
                ExpressionFilter.IsActive.Value = HasCompiledExpression.Value;
            });

            // Log level.
            AddFindByLogLevelToExpression.Subscribe(logLevel => AddExpressionPart($"@l = '{logLevel}'"));
            AddExcludeByLogLevelToExpression.Subscribe(logLevel => AddExpressionPart($"@l <> '{logLevel}'"));
            
            // Event type.
            
            // Properties.
            AddFindByEventTypeToExpression.Subscribe(eventIdHash => AddExpressionPart($"@i = 0x{eventIdHash:X8}"));
            AddExcludeByEventTypeToExpression.Subscribe(eventIdHash => AddExpressionPart($"@i <> 0x{eventIdHash:X8}"));
            AddFindByPropertyToExpression.Subscribe(property => AddExpressionPart($"{property.Key} = '{property.Value.ToString().Trim('"')}'"));
            AddFindWithAnyPropertyValueToExpression.Subscribe(propertyName => AddExpressionPart($"IsDefined({propertyName})"));
            AddExcludeWithAnyPropertyValueToExpression.Subscribe(propertyName => AddExpressionPart($"IsDefined({propertyName}) = false"));
            AddExcludeByPropertyToExpression.Subscribe(property => AddExpressionPart($"{property.Key} != '{property.Value.ToString().Trim('"')}'"));
            AddSeekToTimeSpanToExpression.Subscribe(tuple => 
            {
                var moment = tuple.Item1;
                var timeSpan = tuple.Item2;
                var low = tuple.Item1 - timeSpan;
                var high = moment + timeSpan;
                AddExpressionPart($"Between(@t, '{low:yyyy-MM-dd HH:mm:ss.fffffff}', '{high:yyyy-MM-dd HH:mm:ss.fffffff}')");
            });
        }

        private void AddExpressionPart(
            string expression)
        {
            if (UserSettings.instance.ShowExpressionPanel.Value == false)
            {
                ToggleExpressionPanel.Execute(new ClickEvent());
            }

            ExpressionText.Value = string.IsNullOrWhiteSpace(ExpressionText.Value) 
                ? $"{expression}" 
                : $"{ExpressionText.Value.TrimEnd()}\n and {expression}";
        }
    }
}