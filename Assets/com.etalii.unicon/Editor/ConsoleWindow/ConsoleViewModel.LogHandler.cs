namespace EtAlii.UniCon.Editor
{
    using System;
    using UnityEngine;
    using Object = UnityEngine.Object;
    
    public partial class ConsoleViewModel : ILogHandler
    {
        private readonly ILogHandler _originalLogHandler;

        public void LogFormat(LogType logType, Object context, string format, params object[] args)
        {
            _originalLogHandler.LogFormat(logType, context, format, args);

            var logger = ExpandLogger(_logger, context);
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
            var logger = ExpandLogger(_logger, context);
            logger.Error(exception, "Exception occurred");
        }

        private Serilog.ILogger ExpandLogger(Serilog.ILogger logger, Object context)
        {
            if (context != null)
            {
                logger = logger
                    .ForContext("GameObjectName", context.name)
                    .ForContext("SourceContext", context.GetType().FullName);
            }

            return logger;
        }
    }    
}
