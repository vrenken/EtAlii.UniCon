#nullable enable

namespace EtAlii.UniCon.Editor
{
    using Serilog.Events;

    public class ExpressionHelpers
    {
        public static LogEventPropertyValue? DateTime(LogEventPropertyValue? corpus)
        {
            if (!Coerce.DateTime(corpus, out var dateTimeValue))
            {
                return null;
            }
            return new ScalarValue(dateTimeValue);
        }
        
        public static LogEventPropertyValue? Between(LogEventPropertyValue? moment, LogEventPropertyValue? min, LogEventPropertyValue? max)
        {
            if (!Coerce.DateTime(moment, out var momentValue) || 
                !Coerce.DateTime(min, out var minValue) || 
                !Coerce.DateTime(max, out var maxValue))
            {
                return null;
            }
            var ok = minValue < momentValue && momentValue < maxValue;
            return new ScalarValue(ok);
        }
    }
}
