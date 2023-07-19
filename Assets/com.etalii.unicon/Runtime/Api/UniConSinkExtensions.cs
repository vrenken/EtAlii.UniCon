#nullable enable
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using System;

// ReSharper disable once CheckNamespace
// We want to add these extensions to the correct Serilog space.
namespace Serilog.Sinks.UniCon
{
    public static class UniConSinkExtensions
    {
        // ReSharper disable once InvalidXmlDocComment
        /// <summary>
        /// Writes log events to <see cref="EtAlii.UniCon.Editor.ConsoleWindow"/>.
        /// </summary>
        /// <param name="sinkConfiguration">Logger sink configuration.</param>
        /// <param name="restrictedToMinimumLevel">The minimum level for
        /// events passed through the sink. Ignored when <paramref name="levelSwitch"/> is specified.</param>
        /// <param name="levelSwitch">A switch allowing the pass-through minimum level
        /// to be changed at runtime.</param>
        /// <returns>Configuration object allowing method chaining.</returns>
        public static LoggerConfiguration UniCon(
            this LoggerSinkConfiguration sinkConfiguration,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            LoggingLevelSwitch? levelSwitch = null)
        {
            if (sinkConfiguration == null) throw new ArgumentNullException(nameof(sinkConfiguration));

            return sinkConfiguration.Sink(new UniConLogEventSink(), restrictedToMinimumLevel, levelSwitch);
            // return sinkConfiguration.Sink(new UniConLogEventSink(formatter), restrictedToMinimumLevel, levelSwitch);
        }
    }
}
