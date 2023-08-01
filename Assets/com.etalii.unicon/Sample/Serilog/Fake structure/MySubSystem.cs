namespace EtAlii.UniCon.Sample
{
    // ReSharper disable all
    // We do not want to push too much conventions in this example.

    using Serilog;
    
    public class MySubSystem
    {
        private ILogger _logger = Log.ForContext<MySubSystem>();
   
        private string _objectName = "BuyMeACoffee";
        private string _url = "https://www.buymeacoffee.com/vrenken";
        
        // In case of MonoBehaviours you need to (for now) assign the logger in the awake method. 
        // public void Awake()
        // {
        //    _logger = Log.ForContext<MySubSystem>();
        // }
        
        public void DoSomething()
        {
            _logger.Information("Done something on {ObjectName} at {ObjectUrl}", _objectName, _url);
        }
    }    
}