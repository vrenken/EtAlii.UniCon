namespace EtAlii.UniCon.Editor
{
    using Serilog.Expressions;

    public class FilterRule
    {
        public string Expression { get; }
        public CompiledExpression CompiledExpression { get; }
        public string Error { get; }
        
        public FilterRule(string expression)
        {
            Expression = expression;
            
            if (SerilogExpression.TryCompile(expression, out var compiledExpression, out var error))
            {
                // `compiled` is a function that can be executed against `LogEvent`s:
                // var result = compiled(someEvent);

                // `result` will contain a `LogEventPropertyValue`, or `null` if the result of evaluating the
                // expression is undefined (for example if the event has no `RequestPath` property).
                // if (result is ScalarValue { Value: true })
                // {
                    CompiledExpression = compiledExpression;

                    // Console.WriteLine("The event matched.");
                // }
            }
            else
            {
                Error = error;
                // `error` describes a syntax error.
                //Console.WriteLine($"Couldn't compile the expression; {error}.");
            }
        }
    }
}
