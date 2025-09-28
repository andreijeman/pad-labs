using DistributedSystem.Broker.Messages;
using DistributedSystem.Broker.Client;
using DistributedSystem.Logger;
using DistributedSystem.Network;

namespace DistributedSystem.Publisher;

public class Publisher : Client.Client, IPublisher
{
    public Publisher(IPostman<Message> postman, ILogger logger) : base(postman, logger)
    {
    }
    
    public async Task<bool> RegisterPubliher(string name)
    {
        try
        {
            await Postman.SendPacketAsync(Socket, 
                new Message { Code = MessageCode.RegisterPublisher, Body = name });
            
            var response = await Postman.ReceivePacketAsync(Socket);
            
            if (response.Code == MessageCode.Ok)
            {
                Logger.LogInfo("Registration succeeded.");
                return true;
            }

            Logger.LogWarning("Registration failed.");
        }
        catch (Exception e)
        {
            Logger.LogError(e.Message);
        }

        return false;
    }
    
}
