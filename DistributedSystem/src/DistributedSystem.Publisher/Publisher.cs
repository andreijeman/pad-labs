using DistributedSystem.Broker.Messages;
using DistributedSystem.Logger;
using DistributedSystem.Network;
using System.Net.Sockets;

namespace DistributedSystem.Publisher;

public class Publisher : IPublisher
{
    private readonly string _identifier;
    
    private readonly IPostman<Message> _postman;
    private readonly ILogger _logger;

    private readonly Socket _socket;
    
    public Publisher(string identifier, IPostman<Message> postman, ILogger logger)
    {
        _identifier = identifier;
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _postman = postman;
        _logger = logger;
    }
    
    public bool Connected => _socket.Connected;

    public async Task ConnectAsync(ConnectionArgs args)
    {
        try
        {
            await _socket.ConnectAsync(args.IpAddress, args.Port);
            await _postman.SendPacketAsync(_socket, new Message { Command = MessageCommand.Authenticate, Body = _identifier });
            
            var response = await _postman.ReceivePacketAsync(_socket);
            
            if (response.Command != MessageCommand.Authenticated)
                _logger.LogWarning($"Authentication failed: {response.Body}");

        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
        }
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

    public async Task RegisterPubliher(string name)
    {
        try
        {
            await _postman.SendPacketAsync(_socket, 
                new Message { Command = MessageCommand.RegisterPublisher, Body = name });
            
            // Not implemented: broker status message.
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
        }
    }
    
}
