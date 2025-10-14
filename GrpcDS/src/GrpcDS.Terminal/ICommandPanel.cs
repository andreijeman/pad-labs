using GrpcDS.Logger;

namespace GrpcDS.Terminal;

public interface ICommandPanel : ILogger
{
    Task StartAsync(CancellationToken cancellationToken = default);

    void Clear();
    void Help(string? context = null);
    void AddCommand(ICommand command);
    void ExecuteTextCommand(string text);
    
    void ShowMessageAction(Action printAction);
}