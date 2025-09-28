using DistributedSystem.Broker.Messages;
using System.Net.Sockets;
using DistributedSystem.Broker.Client;
using DistributedSystem.Common;
using DistributedSystem.Logger;
using DistributedSystem.Network;

namespace DistributedSystem.Subscriber;

public class Subscriber : Client.Client, ISubscriber
{
    public Subscriber(IPostman<Message> postman, ILogger logger) :  base(postman, logger)
    {
    }

    public async Task<bool> Subscribe(string publisher)
    {
        try
        {
            await Postman.SendPacketAsync(Socket, 
                new Message { Code = MessageCode.Subscribe, Body = publisher });
            
            var response = await Postman.ReceivePacketAsync(Socket);
            
            if (response.Code == MessageCode.Ok)
            {
                Logger.LogInfo("Subscription succeeded.");
                return true;
            }

            Logger.LogWarning("Subscription failed.");
        }
        catch (Exception e)
        {
            Logger.LogError(e.Message);
        }

        return false;
    }

    public async Task<bool> Unsubscribe(string publisher)
    {
        try
        {
            await Postman.SendPacketAsync(Socket, 
                new Message { Code = MessageCode.Unsubscribe, Body = publisher });
            
            var response = await Postman.ReceivePacketAsync(Socket);
            
            if (response.Code == MessageCode.Ok)
            {
                Logger.LogInfo("Unsubscription succeeded.");
                return true;
            }

            Logger.LogWarning("Unsubscription failed.");
        }
        catch (Exception e)
        {
            Logger.LogError(e.Message);
        }

        return false;
    }
}
