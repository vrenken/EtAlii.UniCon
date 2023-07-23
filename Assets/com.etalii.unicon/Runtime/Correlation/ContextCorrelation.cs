// Copyright (c) Peter Vrenken. All rights reserved. See the license on https://github.com/vrenken/EtAlii.Ubigia

// ReSharper disable once CheckNamespace
namespace EtAlii.UniCon
{

    using System;
    using System.Collections.Immutable;
    using System.Threading;


    /// <summary>
    /// The ContextCorrelation facilitates the parameterless transfer of objects across a call-stack.
    /// The current implementation is not directly bound to logging, but one of the most important use cases is to
    /// transfer logging correlation ID's over long distances easily.
    /// </summary>
    public static class ContextCorrelation 
    {
        // Source: https://github.com/serilog/serilog/issues/1015
        private static ImmutableDictionary<string, string> Items
        {
            get => InternalItems.Value ?? (InternalItems.Value = ImmutableDictionary<string, string>.Empty);
            set => InternalItems.Value = value;
        }

        private static readonly AsyncLocal<ImmutableDictionary<string, string>> InternalItems = new();

        public static bool TryGetValue(string key, out string value)
        {
            return Items.TryGetValue(key, out value);
        }

        /// <summary>
        /// Begin a correlation scope for th given key/value pair. The intended setup is to use the using pattern,
        /// to make sure that the scope is used automatically when needed.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="throwWhenAlreadyCorrelated"></param>
        /// <returns></returns>
        public static IDisposable BeginCorrelationScope(string key, string value, bool throwWhenAlreadyCorrelated = true)
        {
            return BeginCorrelationScope(key, value, null, throwWhenAlreadyCorrelated);
        }

        /// <summary>
        /// Begin a correlation scope for the given key/value pair. The intended setup is to use the using pattern,
        /// to make sure that the scope is used automatically when needed.
        ///
        /// The provided relatedDisposable instance will be disposed at the end of the scope as well.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="relatedDisposable"></param>
        /// <param name="throwWhenAlreadyCorrelated"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static IDisposable BeginCorrelationScope(string key, string value, IDisposable relatedDisposable,
            bool throwWhenAlreadyCorrelated = true)
        {
            if (throwWhenAlreadyCorrelated)
            {
                if (Items.ContainsKey(key))
                {
                    throw new InvalidOperationException($"{key} is already being correlated!");
                }
            }
            else
            {
                var valueAlreadyInContext = Items.ContainsKey(key);
                if (valueAlreadyInContext)
                {
                    return new UnrelatedCorrelationScope(Items);
                }
            }

            IDisposable scope = relatedDisposable != null
                ? new RelatedCorrelationScope(Items, relatedDisposable)
                : new UnrelatedCorrelationScope(Items);

            Items = Items.Add(key, value);
            return scope;
        }

        /// <summary>
        /// A correlation scope that has nothing to do with a LogContext.
        /// </summary>
        private sealed class UnrelatedCorrelationScope : IDisposable
        {
            private readonly ImmutableDictionary<string, string> _bookmark;

            public UnrelatedCorrelationScope(ImmutableDictionary<string, string> bookmark)
            {
                _bookmark = bookmark ?? throw new ArgumentNullException(nameof(bookmark));
            }

            public void Dispose()
            {
                Items = _bookmark;
            }
        }

        /// <summary>
        /// A correlation scope that takes into account a disposable LogContext.
        /// </summary>
        private sealed class RelatedCorrelationScope : IDisposable
        {
            private readonly ImmutableDictionary<string, string> _bookmark;
            private readonly IDisposable _relatedDisposable;

            public RelatedCorrelationScope(ImmutableDictionary<string, string> bookmark, IDisposable relatedDisposable)
            {
                _bookmark = bookmark ?? throw new ArgumentNullException(nameof(bookmark));
                _relatedDisposable = relatedDisposable ?? throw new ArgumentNullException(nameof(relatedDisposable));
            }

            public void Dispose()
            {
                _relatedDisposable.Dispose();
                Items = _bookmark;
            }
        }
    }
}