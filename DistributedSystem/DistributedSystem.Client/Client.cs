using System.Net.Sockets;
using DistributedSystem.Broker.Client;
using DistributedSystem.Broker.Messages;
using DistributedSystem.Logger;
using DistributedSystem.Network;

namespace DistributedSystem.Client;

public class Client : IClient
{
    protected readonly IPostman<Message> Postman;
    protected readonly ILogger Logger;

    protected readonly Socket Socket;
    
    public Client(IPostman<Message> postman, ILogger logger)
    {
        Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        Postman = postman;
        Logger = logger;
    }
    
    public virtual async Task<bool> ConnectAsync(ConnectionArgs args)
    {
        try
        {
            await Socket.ConnectAsync(args.IpAddress, args.Port);
        }
        catch (Exception e)
        {
            Logger.LogError(e.Message);
            return false;
        }

        Logger.LogInfo("Connection succeeded.");
        return true;
    }

    public virtual async Task<bool> AuthenticateAsync(AuthenticationArgs args)
    {
        try
        {
            await Postman.SendPacketAsync(Socket, 
                new Message { Code = MessageCode.Authenticate, Body = args.Username });
            
            var response = await Postman.ReceivePacketAsync(Socket);

            if (response.Code == MessageCode.Ok)
            {
                Logger.LogInfo("Authentication succeeded.");
                return true;
            }

            Logger.LogWarning("Authentication failed.");
        }
        catch (Exception e)
        {
            Logger.LogError(e.Message);
        }
        
        return false;
    }

    public virtual async Task SendMessageAsync(Message message)
    {
        try
        {
            await Postman.SendPacketAsync(Socket, message);
        }
        catch(Exception e) 
        {
            Logger.LogError(e.Message);
        }
    }
}