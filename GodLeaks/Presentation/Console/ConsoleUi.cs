using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace GodLeaks.Presentation.Console;

public class ConsoleUi : IConsoleUi
{
    public void WriteError(string message)
    {
        AnsiConsole.MarkupLine(
            $"[bold red][[ERROR]][/] [grey70]{Markup.Escape(message)}[/]");
    }

    public void WriteSuccess(string message)
    {
        AnsiConsole.MarkupLine(
            $"[bold green][[SUCCESS]][/] [grey70]{Markup.Escape(message)}[/]");
    }

    public void WriteLog(LogLevel logLevel, string message)
    {
        var color = logLevel switch
        {
            LogLevel.Trace => "grey",
            LogLevel.Debug => "silver",
            LogLevel.Information => "dodgerblue1",
            LogLevel.Warning => "yellow",
            LogLevel.Error => "red",
            LogLevel.Critical => "bold red",
            LogLevel.None => "grey",
            _ => "white"
        };

        AnsiConsole.MarkupLine(
            $"[bold {color}][[{logLevel.ToString().ToUpperInvariant()}]][/] " +
            $"[bold white]{Markup.Escape(message)}[/]");
    }
}