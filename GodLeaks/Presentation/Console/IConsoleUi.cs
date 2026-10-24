namespace GodLeaks.Presentation.Console;

public interface IConsoleUi
{
    void WriteError(string message);
    void WriteSuccess(string message);
}