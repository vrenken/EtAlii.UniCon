namespace Tests
{
    using Serilog;

    public class HolographicMenu
    {
        private readonly Networking _networking = new ();

        private readonly ILogger _logger = Log.ForContext<HolographicMenu>();

        public void DoLogEntry()
        {
            LogEntryWriter.WriteEntry(_logger);
            _networking.DoLogEntry();
        }
    }
}