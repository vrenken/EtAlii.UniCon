namespace EtAlii.UniCon.Editor
{
    using System;
    using System.Collections.Generic;
    using Serilog.Events;
    using UniRx;
    using UnityEngine.UIElements;

    public class ExpressionViewModel
    {
        internal UserSettings UserSettings => UserSettings.instance;
        internal ProjectSettings ProjectSettings => ProjectSettings.instance;

        public readonly CustomFilter ExpressionFilter = new();
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
        
        public void Bind(FiltersViewModel filtersViewModel, StreamingViewModel streamingViewModel)
        {
            ToggleExpressionPanel
                .Subscribe(_ =>
                {
                    UserSettings.ShowExpressionPanel.Value = !UserSettings.ShowExpressionPanel.Value;
                    UserSettings.ExpressionPanelHeight.Value = UserSettings.ExpressionPanelHeight.Value;
                });

            ExpressionFilter.CompiledExpression.Subscribe(value =>
            {
                HasCompiledExpression.Value = value != null;
                streamingViewModel.ConfigureStream();
            });
            
            ExpressionText.Subscribe(text =>
            {
                ExpressionFilter.Expression.Value = text;
                ExpressionFilter.IsActive.Value = ExpressionFilter.CompiledExpression != null;
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

        private void AddExpressionPart(
            string expression)
        {
            if (UserSettings.ShowExpressionPanel.Value == false)
            {
                ToggleExpressionPanel.Execute(new ClickEvent());
            }

            ExpressionText.Value = string.IsNullOrWhiteSpace(ExpressionText.Value) 
                ? $"{expression}" 
                : $"{ExpressionText.Value.TrimEnd()}\n and {expression}";
        }
    }
}