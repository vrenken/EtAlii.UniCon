// Copyright (c) Peter Vrenken. All rights reserved. 

namespace EtAlii.Unicon
{
    using Serilog.Events;

    // ReSharper disable once InconsistentNaming
    public class LogEntry
    {
        public long Index;
        // ReSharper disable once InconsistentNaming
        public LogEvent LogEvent;
    }
}