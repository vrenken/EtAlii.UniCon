namespace EtAlii.UniCon.Editor
{
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
        /// The expression that should be run on the stream of event signals.
        /// This gets compiled into the <see cref="CompiledExpression"/>.
        /// </summary>
        public string Expression 
        { 
            get => _expression; 
            set
            {
                if (string.Equals(value, _expression)) return;
                _expression = value;
                Update();
            } 
        }
        private string _expression;

        public CompiledExpression CompiledExpression { get; private set; }
        
        /// <summary>
        /// Any error that occurred when compiling the <see cref="Expression"/> into the <see cref="CompiledExpression"/>.
        /// Null indicates no error and that the CompiledExpression can be used.
        /// </summary>
        public string Error { get; private set; }
        
        private void Update()
        {
            if (SerilogExpression.TryCompile(Expression, out var compiledExpression, out var error))
            {
                // `compiledExpression` is a function that can be executed against `LogEvent`s:
                // `result` will contain a `LogEventPropertyValue`, or `null` if the result of evaluating the
                // expression is undefined (for example if the event has no `RequestPath` property).
                CompiledExpression = compiledExpression;
            }
            else
            {
                // `error` describes a syntax error.
                Error = error;
                CompiledExpression = null;
            }
        }
    }
}
