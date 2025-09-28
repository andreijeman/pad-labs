namespace DistributedSystem.Terminal;

public interface ICommand
{
    string Name { get; }
    string Description { get; }
    void Execute(Dictionary<string, string> args);
}