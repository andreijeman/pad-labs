namespace DistributedSystem.Terminal.DefaultCommands;

public class ClearCommand : PanelCommandBase
{
    public ClearCommand(ICommandPanel panel) : base(panel)
    {
        Name = "clear";
        Description = new CommandDescriptionBuilder
        {
            Name = this.Name,
            Description = "Clear the terminal buffer",
            Signature = "clear"
        }.Build();
    }

    public override Task Execute(Dictionary<string, string> args)
    {
        Panel.Clear();
        return Task.CompletedTask;
    }
}