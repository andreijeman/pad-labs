namespace DistributedSystem.Terminal.DefaultCommands;

public abstract class PanelCommandBase : ICommand
{
    public PanelCommandBase(ICommandPanel panel)
    {
        Panel = panel;
    }

    public string Name { get; protected set; } = null!;
    public string Description { get; protected set; } = null!;
    protected ICommandPanel Panel { get; }

    public abstract Task Execute(Dictionary<string, string> args);
}