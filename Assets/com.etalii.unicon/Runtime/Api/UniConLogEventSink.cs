#nullable enable
using Serilog.Core;
using Serilog.Events;
using EtAlii.UniCon;

// ReSharper disable once CheckNamespace
// We want to add these extensions to the correct Serilog space.
namespace Serilog.Sinks.UniCon
{
    public sealed class UniConLogEventSink : ILogEventSink
    {
        public void Emit(LogEvent logEvent) => LogSink.Instance.LogEvents.Add(logEvent);
    }
}