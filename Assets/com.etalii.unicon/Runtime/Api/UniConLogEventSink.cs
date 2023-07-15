#nullable enable
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using EtAlii.UniCon;

// ReSharper disable once CheckNamespace
// We want to add these extensions to the correct Serilog space.
namespace Serilog.Sinks.UniCon
{
    public sealed class UniConLogEventSink : ILogEventSink
    {
        private readonly ITextFormatter _formatter;

        public UniConLogEventSink(ITextFormatter formatter)
        {
            _formatter = formatter;
        }

        public void Emit(LogEvent logEvent)
        {
            LogSink.LogEvents.Add(logEvent);
            
            // using var buffer = new StringWriter();
            //
            // _formatter.Format(logEvent, buffer);
            // var logType = logEvent.Level switch
            // {
            //     LogEventLevel.Verbose or LogEventLevel.Debug or LogEventLevel.Information => LogType.Log,
            //     LogEventLevel.Warning => LogType.Warning,
            //     LogEventLevel.Error or LogEventLevel.Fatal => LogType.Error,
            //     _ => throw new ArgumentOutOfRangeException(nameof(logEvent.Level), "Unknown log level"),
            // };
            //
            // object message = buffer.ToString().Trim();
            //
            // UnityEngine.Object? unityContext = null;
            // if (logEvent.Properties.TryGetValue(UnityObjectEnricher.UnityContextKey, out var contextPropertyValue) && contextPropertyValue is ScalarValue contextScalarValue)
            // {
            //     unityContext = contextScalarValue.Value as UnityEngine.Object;
            // }
            //
            // string? unityTag = null;
            // if (logEvent.Properties.TryGetValue(UnityTagEnricher.UnityTagKey, out var tagPropertyValue) && tagPropertyValue is ScalarValue tagScalarValue)
            // {
            //     unityTag = tagScalarValue.Value as string;
            // }
            //
            //
            // if (unityContext != null)
            // {
            //     if (unityTag != null)
            //     {
            //         _unityLogger.Log(logType, unityTag, message, unityContext);
            //     }
            //     else
            //     {
            //         _unityLogger.Log(logType, message, unityContext);
            //     }
            // }
            // else if (unityTag != null)
            // {
            //     _unityLogger.Log(logType, unityTag, message);
            // }
            // else
            // {
            //     _unityLogger.Log(logType, message);
            // }
        }
    }
}