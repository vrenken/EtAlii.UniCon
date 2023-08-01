namespace Tests
{
    using Serilog;

    public class AvatarSystem
    {
        private readonly HolographicMenu _holographicMenu = new ();
        
        private readonly ILogger _logger = Log.ForContext<AvatarSystem>();

        public void DoLogEntry()
        {
            SerilogLogEntryWriter.WriteEntry(_logger);
            _holographicMenu.DoLogEntry();
        }
    }
}