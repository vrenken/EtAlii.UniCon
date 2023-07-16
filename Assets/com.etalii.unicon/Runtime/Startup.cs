namespace EtAlii.UniCon
{
    using UnityEngine;
    using Serilog;
    using Serilog.Sinks.UniCon;
    using Serilog.Sinks.Unity3D;

    public static class Startup
    {
        // Debug initialization should happen as early as possible, before any other system is started.
#if DEBUG
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
#else 
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen]     
#endif
        private static void Load()
        {
            Debug.Log($"Startup running");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Unity3D()
                .WriteTo.UniCon()
                .CreateLogger();
        }
    }
}