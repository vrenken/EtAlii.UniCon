namespace Tests
{
    using Serilog;

    public class HolographicMenu
    {
        private readonly GrpcNetworking _grpcNetworking = new ();
        
        private readonly ILogger _logger = Log.ForContext<HolographicMenu>();

        public void DoLogEntry()
        {
            LogEntryWriter.WriteEntry(_logger);
            _grpcNetworking.DoLogEntry();
        }
    }
}