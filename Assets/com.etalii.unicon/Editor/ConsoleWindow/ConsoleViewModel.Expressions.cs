namespace EtAlii.UniCon.Editor
{
    using System;
    using System.Collections.Generic;
    using Serilog.Events;
    using UniRx;
    using UnityEngine.UIElements;

    public partial class ConsoleViewModel
    {
        /// <summary>
        /// Gets raised when any property related to the rules has changed and the view needs to update itself accordingly.
        /// </summary>
        public event Action<string> ExpressionChanged;

        public readonly List<FilterRule> SelectedFilterRules = new ();

        public readonly ReactiveCommand<ClickEvent> OnExpressionButtonClick = new();

        // log level
        public readonly ReactiveCommand<LogEventLevel> AddFindByLogLevelToExpression = new();
        public readonly ReactiveCommand<LogEventLevel> AddExcludeByLogLevelToExpression = new();
        
            
        public readonly ReactiveCommand<KeyValuePair<string, LogEventPropertyValue>> AddFindByPropertyToExpression = new();
        public readonly ReactiveCommand<string> AddFindWithAnyPropertyValueToExpression = new();
        public readonly ReactiveCommand<string> AddExcludeWithAnyPropertyValueToExpression = new();
        
        
        public readonly ReactiveCommand<KeyValuePair<string, LogEventPropertyValue>> AddExcludeByPropertyToExpression = new();

        
        public readonly ReactiveCommand<uint> AddFindByEventTypeToExpression = new();
        public readonly ReactiveCommand<uint> AddExcludeByEventTypeToExpression = new();

        public readonly ReactiveCommand<(DateTimeOffset, TimeSpan)> AddSeekToTimeSpanToExpression = new();
            
        public readonly ReactiveProperty<string> ExpressionText = new();
        
        public readonly FilterRule ActiveFilterRule = new();
        
        private void SetupExpression()
        {
            OnExpressionButtonClick.Subscribe(_ => Settings.ShowExpressionPanel.Value = !Settings.ShowExpressionPanel.Value);

            ExpressionText.Subscribe(s =>
            {
                ActiveFilterRule.Expression = s;
                ExpressionChanged?.Invoke(nameof(ActiveFilterRule));
                ConfigureStream();
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
                AddExpressionPart($"'{low}' < @t < '{high}'");
            });
        }

        private void AddExpressionPart(string expression)
        {
            Settings.ShowExpressionPanel.Value = true;

            ActiveFilterRule.Expression = string.IsNullOrWhiteSpace(ActiveFilterRule.Expression) 
                ? $"{expression}" 
                : $"{ActiveFilterRule.Expression.TrimEnd()}\n and {expression}";
            ExpressionChanged?.Invoke(nameof(ActiveFilterRule));
            ConfigureStream();
        }
    }    
}
