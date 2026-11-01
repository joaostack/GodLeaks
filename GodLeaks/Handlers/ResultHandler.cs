using Microsoft.Extensions.Logging;
using NetCord.Gateway;
using NetCord.Services;
using NetCord.Services.Commands;
using NetCord.Hosting.Services.Commands;

namespace GodLeaks.Handlers;

public class CustomCommandResultHandler<TContext>
    : ICommandResultHandler<TContext>
    where TContext : ICommandContext
{
    public async ValueTask HandleResultAsync(
        IExecutionResult result,
        TContext context,
        GatewayClient client,
        ILogger logger,
        IServiceProvider services)
    {
        if (result is not IFailResult failResult)
            return;
        
        if (failResult is IExceptionResult exceptionResult)
        {
            logger.LogError(
                exceptionResult.Exception,
                "An exception occurred while executing a command."
            );

            await context.Message.SendAsync(
                "❌ **Erro ao executar comando!**"
            );
        }
        else
        {
            logger.LogWarning(
                "Command execution failed: {Message}",
                failResult.Message
            );

            if (failResult == ParameterCountMismatchResult.TooFew)
            {
                await context.Message.SendAsync(
                    $"❌ **Especifique o alvo como parâmetro!**"
                );
                return;
            }

            await context.Message.SendAsync(
                $"❌ **Erro ao executar comando:** {failResult.Message}"
            );
        }
    }
}