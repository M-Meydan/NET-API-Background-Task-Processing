using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging;
using System.IO;
using System;

namespace WebAPI.Configuration
{
    public class CustomConsoleFormatter : ConsoleFormatter
    {
        public CustomConsoleFormatter() : base("custom") { }

        public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider scopeProvider, TextWriter textWriter)
        {
            var logLevel = ShortenLogLevel(logEntry.LogLevel);
            var categoryName = logEntry.Category;
            var message = logEntry.Formatter?.Invoke(logEntry.State, logEntry.Exception);

            if (message != null)
            {
                var timeStamp = DateTime.Now.ToString("HH:mm:ss");
                var lastPartOfCategory = categoryName?.Substring(categoryName.LastIndexOf('.') + 1); // Extract class name
                textWriter.WriteLine($"[{lastPartOfCategory}:{timeStamp}] {logLevel}: {message}");
            }
        }

        private string ShortenLogLevel(LogLevel logLevel)
        {
            return logLevel switch
            {
                LogLevel.Trace => "trce",
                LogLevel.Debug => "dbug",
                LogLevel.Information => "info",
                LogLevel.Warning => "warn",
                LogLevel.Error => "err",
                LogLevel.Critical => "crit",
                LogLevel.None => "none",
                _ => logLevel.ToString()
            };
        }
    }
}
