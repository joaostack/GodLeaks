using GodLeaks.Presentation.Console;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace GodLeaks.Logging;

public class BotLogger : ILogger
{
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default;

    public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;

        var message = formatter(state, exception);
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        var consoleUi = new ConsoleUi();

        consoleUi.WriteLog(logLevel, $"{timestamp} {message}");
            
        if (exception != null)
        {
            AnsiConsole.WriteException(exception);
        }
    }
}