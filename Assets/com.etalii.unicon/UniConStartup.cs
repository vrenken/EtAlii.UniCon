﻿using EtAlii.UniCon;
using UnityEngine;
using Serilog;
using Serilog.Sinks.UniCon;

public static partial class UniConStartup
{
    // Debug initialization should happen as early as possible, before any other system is started.
    // This seems to be the earliest possible:
    // https://uninomicon.com/runtimeinitializeonload
    // https://gist.github.com/hybridherbst/36ae70b6520981c8edc7b478423fae5e
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Load()
    {
        if (Log.Logger == Serilog.Core.Logger.None)
        {
            var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Verbose();

            loggerConfiguration = loggerConfiguration
                .Enrich.FromLogContext()
                .Enrich.WithThreadId()
                //.Enrich.WithThreadName()
                .Enrich.WithMemoryUsage();

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