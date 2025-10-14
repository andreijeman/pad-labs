namespace GrpcDS.Terminal.DefaultCommands;

public class HelpCommand : PanelCommandBase
{
    public HelpCommand(ICommandPanel panel) : base(panel)
    {
        Name = "help";
        Description = new CommandDescriptionBuilder
        {
            Name = this.Name,
            Description = "Show help information",
            Signature = "help [-c arg]"
        }.Build();
    }

    public override Task Execute(Dictionary<string, string> args)
    {
        Panel.Help(args.GetValueOrDefault("-c"));
        return Task.CompletedTask;
    }
}