namespace EtAlii.UniCon.Editor
{
    using Serilog.Events;

    public record PipelineItem
    {
        public int Index;
        public LogEvent LogEvent;
    }
}