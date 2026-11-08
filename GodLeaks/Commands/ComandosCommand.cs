using System.Text;
using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.Commands;

namespace GodLeaks.Commands;

public class ComandosCommand : CommandModule<CommandContext>
{
    private const string ImagePath = "Assets/doxbin.jpg";
    private const string ImageFileName = "doxbin.jpg";

    private static readonly (string Command, string Description)[] Commands =
    [
        ("/cpf", "Consulta CPF"),
        ("/email", "Consulta e-mail"),
        ("/username", "Consulta username"),
        ("/placa", "Consulta placa de veículo"),
        ("/nome", "Consulta nome completo"),
        ("/senha", "Consulta senha"),
        ("/url", "Consulta logins de uma URL"),
        ("/telefone", "Consulta telefone")
    ];

    [SlashCommand("comandos", "Exibe a central de comandos")]
    public async ValueTask ComandosAsync()
    {
        await using var stream = File.OpenRead(ImagePath);

        var messageProperties = new MessageProperties
        {
            Content = "💀 **GodLeaks | Central de comandos** 💀",
            Attachments = [new AttachmentProperties(ImageFileName, stream)],
            Embeds = [BuildEmbedMenu()]
        };

        await Context.Message.SendAsync(messageProperties);
    }

    private static EmbedProperties BuildEmbedMenu()
    {
        var descriptionBuilder = new StringBuilder()
            .AppendLine("Escolha uma consulta e deixe o restante com a gente!")
            .AppendLine();

        foreach (var (command, description) in Commands)
        {
            descriptionBuilder.AppendLine($"🔹 `{command}` — {description}");
        }

        descriptionBuilder.AppendLine()
            .Append($"📚 **Total:** {Commands.Length} comandos disponíveis");

        return new EmbedProperties
        {
            Title = "👑 Menu Principal de Consultas",
            Description = descriptionBuilder.ToString(),
            Color = new Color(0xFF00FF),

            Image = new EmbedImageProperties(url: $"attachment://{ImageFileName}"),

            Timestamp = DateTimeOffset.UtcNow
        };
    }
}