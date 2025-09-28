using System.Windows.Input;
using DistributedSystem.Terminal;
using Logger;

var commands = new Dictionary<string, Action<List<string>, ILogger>>();

commands.Add("bip", (s, logger) =>
{
    Console.Beep();
    logger.LogInfo("It was beep");
} );

var panel = new CommandPanel(commands);

panel.Start();