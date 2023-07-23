// Copyright (c) Peter Vrenken. All rights reserved. See the license on https://github.com/vrenken/EtAlii.Ubigia

// ReSharper disable once CheckNamespace
namespace Serilog.Correlation
{
    using System;
    using Serilog.Context;

    public static class LoggerCorrelationScopeExtension
    {
        public static IDisposable BeginCorrelationScope(
            this ILogger logger,
            string key, string value,
            bool throwWhenAlreadyCorrelated = true)
        {
            return ContextCorrelation.BeginCorrelationScope(key, value, LogContext.PushProperty(key, value),
                throwWhenAlreadyCorrelated);
        }
    }
}
