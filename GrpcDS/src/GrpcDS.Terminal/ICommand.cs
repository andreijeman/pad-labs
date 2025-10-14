namespace GrpcDS.Terminal;

public interface ICommand
{
    string Name { get; }
    string Description { get; }
    Task Execute(Dictionary<string, string> args);
}