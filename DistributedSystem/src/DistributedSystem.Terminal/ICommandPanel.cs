using DistributedSystem.Logger;

namespace DistributedSystem.Terminal;

public interface ICommandPanel : ILogger
{
    void Start(CancellationToken cancellationToken = default);
    void OnMessageReceived(string message);
    event EventHandler<string> MessageSent;
    void AddCommand(string command, CommandAction action);
}