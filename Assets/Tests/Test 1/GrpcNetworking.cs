namespace Tests
{
    using Serilog;

    public class GrpcNetworking
    {
        private readonly ILogger _logger = Log.ForContext<GrpcNetworking>();

        public void DoLogEntry()
        {
            LogEntryWriter.WriteEntry(_logger);
        }
    }
}