namespace EtAlii.UniCon
{
    using System;
    using System.Text.RegularExpressions;
    using UnityEngine;
    using Object = UnityEngine.Object;
    
    public class UniConLogHandler : ILogHandler
    {
        private readonly ILogHandler _originalLogHandler;
        private readonly Serilog.ILogger _logger;

        private readonly Regex _namedMatching = new (@"([A-Za-z_][A-Za-z0-9_]*)=\{([0-9]*)\}", RegexOptions.Compiled);
        private readonly Regex _interpolatedMatching = new (@"\{[A-Za-z_][A-Za-z0-9_]*\}", RegexOptions.Compiled);
        private readonly MatchEvaluator _namedSerilogMatchEvaluator = m => m.Result($"{{{m.Groups[1]}}}");
        private readonly MatchEvaluator _namedUnityMatchEvaluator = m => m.Result($"<b><color={WellKnownColor.UnityMarkerTagColor}>{m.Value}</color></b>");

        
        public UniConLogHandler(ILogHandler originalLogHandler, Serilog.ILogger logger)
        {
            _originalLogHandler = originalLogHandler;
            _logger = logger;
        }
        
        [HideInCallstack]
        public void LogFormat(LogType logType, Object context, string format, params object[] args)
        {
            if (format == "{0}" && args.Length == 1)
            {
                format = args[0].ToString();
                args = Array.Empty<object>();
            }

            var unityLogFormat = format;
            var namedMatches = _namedMatching.Matches(unityLogFormat);
            if (namedMatches.Count > 0)
            {
                format = _namedMatching.Replace(format, _namedSerilogMatchEvaluator);
                unityLogFormat = _namedMatching.Replace(unityLogFormat, _namedUnityMatchEvaluator);
            }
            else
            {
                var index = 0;
                var unityEvaluator = new MatchEvaluator(m => m.Result($"<b><color={WellKnownColor.UnityMarkerTagColor}>{m.Value.TrimStart('{').TrimEnd('}')} (=\"{{{index++}}}\")</color></b>"));
                unityLogFormat = _interpolatedMatching.Replace(unityLogFormat, unityEvaluator);
            }
            
            if (context != null && !string.IsNullOrWhiteSpace(context.name))
            {
                unityLogFormat = $"<b><color={WellKnownColor.UnityHeaderHexColor}>[{context.name}]</color></b> {unityLogFormat}";
            }
            _originalLogHandler.LogFormat(logType, context, unityLogFormat, args);

            var logger = ExpandLoggerForUnity(_logger, context);
            switch (logType)
            {
                // ReSharper disable TemplateIsNotCompileTimeConstantProblem
                case LogType.Log: logger.Information(format, args); break;
                case LogType.Error: logger.Error(format, args); break;
                case LogType.Exception: logger.Error(format, args); break;
                case LogType.Assert: logger.Error(format, args); break;
                case LogType.Warning: logger.Warning(format, args); break;
                default: throw new ArgumentOutOfRangeException(nameof(logType), logType, null);
                // ReSharper restore TemplateIsNotCompileTimeConstantProblem
            }
        }

        [HideInCallstack]
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
                .ForContext(WellKnownProperty.LogSource, WellKnownPropertyValue.UnityLogSource);

            if (context != null)
            {
                logger = logger.ForContext("SourceContext", context.GetType().FullName);
            }
            return logger;
        }
    }    
}
