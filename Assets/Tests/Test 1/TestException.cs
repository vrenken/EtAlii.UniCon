namespace Tests
{
    using System;

    public class TestException : Exception
    {
        public override string StackTrace { get; }

        public TestException(string message, string stackTrace) 
            : base(message)
        {
            StackTrace = stackTrace;
        }
    }
}