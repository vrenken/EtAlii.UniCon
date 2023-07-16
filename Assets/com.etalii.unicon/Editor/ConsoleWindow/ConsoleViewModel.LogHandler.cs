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
            
            switch (logType)
            {
                // ReSharper disable TemplateIsNotCompileTimeConstantProblem
                // ReSharper disable FormatStringProblem
                case LogType.Log: _logger.Information(string.Format("format", args)); break;
                case LogType.Error: _logger.Error(string.Format("format", args)); break;
                case LogType.Exception: _logger.Error(string.Format("format", args)); break;
                case LogType.Assert: _logger.Error(string.Format("format", args)); break;
                case LogType.Warning: _logger.Warning(string.Format("format", args)); break;
                default: throw new ArgumentOutOfRangeException(nameof(logType), logType, null);
                // ReSharper restore FormatStringProblem
                // ReSharper restore TemplateIsNotCompileTimeConstantProblem
            }
        }

        public void LogException(Exception exception, Object context)
        {
            _originalLogHandler.LogException(exception, context);
            _logger.Error(exception, "Exception occurred");
        }
    }    
}
