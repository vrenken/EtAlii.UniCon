// Copyright (c) Peter Vrenken. All rights reserved. See the license on https://github.com/vrenken/EtAlii.Ubigia

// ReSharper disable once CheckNamespace
namespace Serilog
{
    using System;
    using EtAlii.UniCon;
    using Serilog.Context;

    public static class LoggerCorrelationScopeExtension
    {
        /// <summary>
        /// Begin a correlation scope for the given key/value pair.
        /// All log method calls inside of the using scope will have this key/value assigned as a property.
        /// The provided relatedDisposable instance will be disposed at the end of the scope as well.
        /// </summary>
        public static IDisposable BeginCorrelationScope(
            this ILogger _,
            string key, string value,
            bool throwWhenAlreadyCorrelated = true)
        {
            return ContextCorrelation.BeginCorrelationScope(key, value, LogContext.PushProperty(key, value),
                throwWhenAlreadyCorrelated);
        }
        
        /// <summary>
        /// Begin a correlation scope for the given key pair. A base-36 encoded Guid (a ShortGuid) will be used as a value.
        /// All log method calls inside of the using scope will have this key/value assigned as a property.
        /// The provided relatedDisposable instance will be disposed at the end of the scope as well.
        /// </summary>
        public static IDisposable BeginCorrelationScope(
            this ILogger _,
            string key,
            bool throwWhenAlreadyCorrelated = true)
        {
            var value = ShortGuid.New();
            return ContextCorrelation.BeginCorrelationScope(key, value, LogContext.PushProperty(key, value),
                throwWhenAlreadyCorrelated);
        }

    }
}
