namespace EtAlii.UniCon.Editor
{
    using System;
    using Serilog.Expressions;
    using UniRx;

    public class CustomFilter
    {
        /// <summary>
        /// The name with which the custom filter should be shown in the Filter panel. 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Set this property to true to apply the custom filter to the stream of LogEvents.
        /// </summary>
        public readonly ReactiveProperty<bool> IsActive = new(true);

        /// <summary>
        /// Set this property to true to indicate that the custom filter is being edited.
        /// </summary>
        public readonly ReactiveProperty<bool> IsEditing = new(false);

        /// <summary>
        /// The expression that should be run on the stream of event signals.
        /// This gets compiled into the <see cref="CompiledExpression"/>.
        /// </summary>
        public readonly ReactiveProperty<string> Expression = new ();

        public ReactiveProperty<CompiledExpression> CompiledExpression = new ();
        
        /// <summary>
        /// Any error that occurred when compiling the <see cref="Expression"/> into the <see cref="CompiledExpression"/>.
        /// Null indicates no error and that the CompiledExpression can be used.
        /// </summary>
        public string Error { get; private set; }

        public CustomFilter()
        {
            Expression.Subscribe(_ =>
            {
                Update();
            });

        }
        private void Update()
        {
            try
            {
                // Let's not start complaining if there is no expression to compile.
                var expressionToCompile = string.IsNullOrWhiteSpace(Expression.Value)
                    ? "true"
                    : Expression.Value;

                if (SerilogExpression.TryCompile(expressionToCompile, out var compiledExpression, out var error))
                {
                    // `compiledExpression` is a function that can be executed against `LogEvent`s:
                    // `result` will contain a `LogEventPropertyValue`, or `null` if the result of evaluating the
                    // expression is undefined (for example if the event has no `RequestPath` property).
                    CompiledExpression.Value = compiledExpression;
                }
                else
                {
                    // `error` describes a syntax error.
                    Error = error;
                    CompiledExpression = null;
                }
                
            }
            catch (Exception e)
            {
                Error = e.Message;
                CompiledExpression = null;
            }
        }
    }
}
