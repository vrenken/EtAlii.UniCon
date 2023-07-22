namespace EtAlii.UniCon.Editor
{
    using Serilog.Expressions;

    public class FilterRule
    {
        public string Expression 
        { 
            get => _expression; 
            set 
            {
                if(!string.Equals(value, _expression))
                {
                    _expression = value;
                    Update();
                }
            } 
        }
        private string _expression;

        public CompiledExpression CompiledExpression { get; private set; }
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
