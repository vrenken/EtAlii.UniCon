namespace EtAlii.UniCon.Editor
{
    using System;
    using EtAlii.Unicon;
    using UniRx;

    public class StreamConfiguration
    {
        public ReplaySubject<LogEntry> ForwardSubject;
        public IObservable<LogEntry> Forward;
        public CompositeDisposable ForwardPipeline;

        public ReplaySubject<LogEntry> BackwardSubject;
        public IObservable<LogEntry> Backward;
        public CompositeDisposable BackwardPipeline;
    }
}
