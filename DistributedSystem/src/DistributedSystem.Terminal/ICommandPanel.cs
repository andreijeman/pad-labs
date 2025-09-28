using DistributedSystem.Logger;

namespace DistributedSystem.Terminal;

public interface ICommandPanel : ILogger
{
    void Start(CancellationToken cancellationToken = default);

    void Clear();
    void Help(string? context = null);
    void AddCommand(ICommand command);
    void ShowMessageAction(Action printAction);
    
    void OnMessageReceived(string message);
    event EventHandler<string> MessageSent;
}