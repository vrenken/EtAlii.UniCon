namespace Tests
{
    using Serilog;

    public class Networking
    {
        private readonly MySubSystem _mySubSystem = new();
        
        private readonly ILogger _logger = Log.ForContext<Networking>();

        public void DoLogEntry()
        {
            LogEntryWriter.WriteEntry(_logger);
            
            _mySubSystem.DoSomething();
        }
    }
}