using Granite.IO;
using Container = Granite.UI.Entities.Container;

namespace Granite.UI.Utilities;

public static class ContainerExtensions
{
    public static void BindIO(this Container container)
    {
        ConsoleOutput.Bind(container.GObject);
        KeyboardInput.Bind(container.Controller); 
    }
}