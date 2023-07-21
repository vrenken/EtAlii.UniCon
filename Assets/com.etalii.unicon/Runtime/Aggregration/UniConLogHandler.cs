namespace EtAlii.UniCon
{
    using System;
    using UnityEngine;
    using Object = UnityEngine.Object;
    
    public class UniConLogHandler : ILogHandler
    {
        private readonly ILogHandler _originalLogHandler;
        private readonly Serilog.ILogger _logger;
        
        public UniConLogHandler(ILogHandler originalLogHandler, Serilog.ILogger logger)
        {
            _originalLogHandler = originalLogHandler;
            _logger = logger;
        }
        
        public void LogFormat(LogType logType, Object context, string format, params object[] args)
        {
            _originalLogHandler.LogFormat(logType, context, format, args);

            var logger = ExpandLoggerForUnity(_logger, context);
            switch (logType)
            {
                // ReSharper disable TemplateIsNotCompileTimeConstantProblem
                case LogType.Log: logger.Information(string.Format(format, args)); break;
                case LogType.Error: logger.Error(string.Format(format, args)); break;
                case LogType.Exception: logger.Error(string.Format(format, args)); break;
                case LogType.Assert: logger.Error(string.Format(format, args)); break;
                case LogType.Warning: logger.Warning(string.Format(format, args)); break;
                default: throw new ArgumentOutOfRangeException(nameof(logType), logType, null);
                // ReSharper restore TemplateIsNotCompileTimeConstantProblem
            }
        }

        public void LogException(Exception exception, Object context)
        {
            _originalLogHandler.LogException(exception, context);
            var logger = ExpandLoggerForUnity(_logger, context);
            var exceptionType = exception.GetType().FullName;
            // ReSharper disable TemplateIsNotCompileTimeConstantProblem
            logger
                .ForContext("ExceptionType", exceptionType)
                .ForContext("ExceptionMessage", exception.Message)
                .Error(exception, $"{exception.GetType().Name}: {exception.Message}");
            // ReSharper restore TemplateIsNotCompileTimeConstantProblem
        }

        private Serilog.ILogger ExpandLoggerForUnity(Serilog.ILogger logger, Object context)
        {
            logger = logger
                .ForContext(WellKnownProperties.IsUnityLogEvent, true);

            if (context != null)
            {
                logger = logger.ForContext("SourceContext", context.GetType().FullName);
            }
            return logger;
        }
    }    
}
