namespace EtAlii.UniCon.Editor
{
    using System;

    /// <summary>
    /// The Serilog <see cref="Serilog.Events.LogEventLevel"/> is not a flagged enumeration, hence we need to introduce our own.
    /// </summary>
    [Flags]
    public enum LogLevel
    {
        None = 0,
        /// <summary>
        /// Anything and everything you might want to know about
        /// a running block of code.
        /// </summary>
        Verbose = 1,

        /// <summary>
        /// Internal system events that aren't necessarily
        /// observable from the outside.
        /// </summary>
        Debug = 2,

        /// <summary>
        /// The lifeblood of operational intelligence - things
        /// happen.
        /// </summary>
        Information = 4,

        /// <summary>
        /// Service is degraded or endangered.
        /// </summary>
        Warning = 8,

        /// <summary>
        /// Functionality is unavailable, invariants are broken
        /// or data is lost.
        /// </summary>
        Error = 16,

        /// <summary>
        /// If you have a pager, it goes off when one of these
        /// occurs.
        /// </summary>
        Fatal = 32,
        
        All = Verbose | Debug | Information | Warning | Error | Fatal
    }
}
