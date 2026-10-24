using Microsoft.Extensions.Logging;

namespace GodLeaks.Logging;

public class BotLoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName)
    {
        return new BotLogger();
    }

    public void Dispose()
    {
    }
}