using DistributedSystem.Broker.Messages;
using System.Net.Sockets;
using DistributedSystem.Broker.Client;
using DistributedSystem.Logger;
using DistributedSystem.Network;

namespace DistributedSystem.Publisher;

public class Publisher : IClient, IPublisher
{
    private readonly IPostman<Message> _postman;
    private readonly ILogger _logger;

    private readonly Socket _socket;
    
    public Publisher(IPostman<Message> postman, ILogger logger)
    {
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _postman = postman;
        _logger = logger;
    }
    
    public async Task<bool> ConnectAsync(ConnectionArgs args)
    {
        try
        {
            await _socket.ConnectAsync(args.IpAddress, args.Port);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return false;
        }

        _logger.LogInfo("Connection succeeded.");
        return true;
    }

    public async Task<bool> AuthenticateAsync(AuthenticationArgs args)
    {
        try
        {
            await _postman.SendPacketAsync(_socket, 
                new Message { Code = MessageCode.Authenticate, Body = args.Username });
            
            var response = await _postman.ReceivePacketAsync(_socket);

            if (response.Code == MessageCode.Ok)
            {
                _logger.LogInfo("Authentication succeeded.");
                return true;
            }

            _logger.LogWarning("Authentication failed.");
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
        }
        
        return false;
    }

    public async Task SendMessageAsync(Message message)
    {
        try
        {
            await _postman.SendPacketAsync(_socket, message);
        }
        catch(Exception e) 
        {
            _logger.LogError(e.Message);
        }
    }

    public async Task<bool> RegisterPubliher(string name)
    {
        try
        {
            await _postman.SendPacketAsync(_socket, 
                new Message { Code = MessageCode.RegisterPublisher, Body = name });
            
            var response = await _postman.ReceivePacketAsync(_socket);
            
            if (response.Code == MessageCode.Ok)
            {
                _logger.LogInfo("Registration succeeded.");
                return true;
            }

            _logger.LogWarning("Registration failed.");
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
        }

        return false;
    }
    
}
