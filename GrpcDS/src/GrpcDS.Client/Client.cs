using System.Net.Sockets;
using GrpcDS.Broker.Client;
using GrpcDS.Broker.Messages;
using GrpcDS.Logger;
using GrpcDS.Network;

namespace GrpcDS.Client;

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
    
    public async Task ConnectAsync(ConnectionArgs args)
    {
        try
        {
            await Socket.ConnectAsync(args.IpAddress, args.Port);
            Logger.LogInfo($"Client connected to <{args.IpAddress}:{args.Port}>");
            _ = ReceiveMessagesAsync();
        }
        catch (Exception e)
        {
            Logger.LogError(e.Message);
        }
    }

    public Task AuthenticateAsync(AuthenticationArgs args)
    {
         return SendMessageAsync(new Message { Code = MessageCode.Authenticate, Body = args.Username });
    }

    public async Task SendMessageAsync(Message message)
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

    private async Task ReceiveMessagesAsync(CancellationToken ct = default)
    {
        while (!ct.IsCancellationRequested && Socket.Connected)
        {
            try
            {
                var message = await Postman.ReceivePacketAsync(Socket);
                Logger.LogInfo($"Received message: Code[ {message.Code} ] Body[ {message.Body} ]");
                MessageReceived?.Invoke(this, message);
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
            }
        }
    }

    public event EventHandler<Message>? MessageReceived;
}