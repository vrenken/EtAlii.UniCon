using EtAlii.UniCon;
using UnityEngine;
using Serilog;
using Serilog.Sinks.UniCon;

public static partial class UniConStartup
{
    // Debug initialization should happen as early as possible, before any other system is started.
#if DEBUG
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
#else 
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen]     
#endif
    private static void Load()
    {
        if (Log.Logger == Serilog.Core.Logger.None)
        {
            var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Verbose();

            // ReSharper disable once InvocationIsSkipped
            // This method is used as it provides a way to fine-tune the UniCon Serilog configuration. 
            ConfigureLogging(loggerConfiguration);
            
            Log.Logger = loggerConfiguration
                .WriteTo.UniCon()
                .CreateLogger();
        }

        if (Debug.unityLogger.logHandler is not UniConLogHandler)
        {
            Debug.unityLogger.logHandler = new UniConLogHandler(Debug.unityLogger.logHandler, Log.Logger);
        }
    }

    // ReSharper disable once PartialMethodWithSinglePart
    /// <summary>
    /// Implement this method to customize the UniCon Serilog configuration. For example to also log to other systems
    /// e.g. Seq, or to file.  
    /// </summary>
    /// <param name="loggerConfiguration"></param>
    static partial void ConfigureLogging(LoggerConfiguration loggerConfiguration);
}