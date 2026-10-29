using System.Text;
using NetCord.Rest;
using NetCord.Services.Commands;

namespace GodLeaks.Commands;

public class ComandosCommand : CommandModule<CommandContext>
{
    private const string ImagePath = "Assets/doxbin.jpg";
    private const string ImageFileName = "doxbin.jpg";

    private static readonly (string Command, string Description)[] Commands =
    [
        ("!cpf", "Consulta CPF"),
        ("!email", "Consulta e-mail"),
        ("!username", "Consulta username"),
        ("!placa", "Consulta placa de veículo"),
        ("!nome", "Consulta nome completo"),
        ("!senha", "Consulta senha"),
        ("!url", "Consulta logins de uma URL"),
        ("!telefone", "Consulta telefone")
    ];

    [Command("comandos")]
    public async ValueTask ComandosAsync()
    {
        var channel = Context?.Channel
                      ?? throw new InvalidOperationException("The command context does not contain a channel.");
        await using var stream = File.OpenRead(ImagePath);
        var messageProperties = new MessageProperties
        {
            Attachments = [new AttachmentProperties(ImageFileName, stream)],
            Content = BuildCommandsMessage()
        };

        await channel.SendMessageAsync(messageProperties);
    }

    private static string BuildCommandsMessage()
    {
        var message = new StringBuilder()
            .AppendLine("**GodLeaks | Central de comandos**")
            .AppendLine();

        foreach (var (command, description) in Commands)
        {
            message.Append('`')
                .Append(command)
                .Append("` - ")
                .AppendLine(description);
        }

        return message.AppendLine()
            .Append($"**Total:** {Commands.Length} comandos")
            .ToString();
    }
}