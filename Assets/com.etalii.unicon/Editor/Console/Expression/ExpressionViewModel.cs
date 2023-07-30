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

        public readonly ReactiveCommand<ClickEvent> ToggleExpressionPanel = new();
        // public readonly ReactiveProperty<string> ExpressionText = new();

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

            // ExpressionText.Subscribe(s =>
            // {
            //     SelectedCustomFilter.Expression = s;
            //     ExpressionChanged?.Invoke(nameof(SelectedCustomFilter));
            //     ConfigureStream();
            // });
            
            // Log level.
            AddFindByLogLevelToExpression.Subscribe(logLevel => AddExpressionPart($"@l = '{logLevel}'", filtersViewModel, streamingViewModel));
            AddExcludeByLogLevelToExpression.Subscribe(logLevel => AddExpressionPart($"@l <> '{logLevel}'", filtersViewModel, streamingViewModel));
            
            // Event type.
            
            // Properties.
            AddFindByEventTypeToExpression.Subscribe(eventIdHash => AddExpressionPart($"@i = 0x{eventIdHash:X8}", filtersViewModel, streamingViewModel));
            AddExcludeByEventTypeToExpression.Subscribe(eventIdHash => AddExpressionPart($"@i <> 0x{eventIdHash:X8}", filtersViewModel, streamingViewModel));
            AddFindByPropertyToExpression.Subscribe(property => AddExpressionPart($"{property.Key} = '{property.Value.ToString().Trim('"')}'", filtersViewModel, streamingViewModel));
            AddFindWithAnyPropertyValueToExpression.Subscribe(propertyName => AddExpressionPart($"IsDefined({propertyName})", filtersViewModel, streamingViewModel));
            AddExcludeWithAnyPropertyValueToExpression.Subscribe(propertyName => AddExpressionPart($"IsDefined({propertyName}) = false", filtersViewModel, streamingViewModel));
            AddExcludeByPropertyToExpression.Subscribe(property => AddExpressionPart($"{property.Key} != '{property.Value.ToString().Trim('"')}'", filtersViewModel, streamingViewModel));
            AddSeekToTimeSpanToExpression.Subscribe(tuple => 
            {
                var moment = tuple.Item1;
                var timeSpan = tuple.Item2;
                var low = tuple.Item1 - timeSpan;
                var high = moment + timeSpan;
                AddExpressionPart($"'{low}' < @t < '{high}'", filtersViewModel, streamingViewModel);
            });
        }

        private void AddExpressionPart(
            string expression, 
            FiltersViewModel filtersViewModel, 
            StreamingViewModel streamingViewModel)
        {
            if (UserSettings.ShowExpressionPanel.Value == false)
            {
                ToggleExpressionPanel.Execute(new ClickEvent());
            }

            filtersViewModel.SelectedCustomFilter.Expression.Value = string.IsNullOrWhiteSpace(filtersViewModel.SelectedCustomFilter.Expression.Value) 
                ? $"{expression}" 
                : $"{filtersViewModel.SelectedCustomFilter.Expression.Value.TrimEnd()}\n and {expression}";
            streamingViewModel.ConfigureStream();
        }
    }
}