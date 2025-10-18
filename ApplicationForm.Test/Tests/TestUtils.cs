using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace ApplicationForm.Test.Tests
{
    public class NoopLogger<T> : ILogger<T>
    {
        public IDisposable BeginScope<TState>(TState state) => null!;
        public bool IsEnabled(LogLevel logLevel) => false;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) { }
    }

    public class SinkLogger<T> : ILogger<T>
    {
        public List<(LogLevel Level, string Message)> Logs { get; } = new();
        public IDisposable BeginScope<TState>(TState state) => null!;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            var message = formatter(state, exception);
            Logs.Add((logLevel, message));
        }
    }
}
