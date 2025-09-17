using DistributedSystem.Broker.Messages;
using DistributedSystem.Logger;
using DistributedSystem.Network;
using System.Net.Sockets;

namespace DistributedSystem.Publisher;
public class Publisher : IPublisher
{
    private readonly IPostman<Message> _postman;
    private readonly ILogger _logger;

    private readonly Socket _socket;
    
    private readonly string _topic;
    public bool isConnected {  get; set; }

    public Publisher(IPostman<Message> postman, ILogger logger, string topic)
    {
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _postman = postman;
        _logger = logger;
        _topic = topic;
    }


    public async Task ConnectAsync(Configuration configuration)
    {
        try
        {
            await _socket.ConnectAsync(configuration.IpAddress, configuration.Port);
            isConnected = _socket.Connected;

            if (isConnected)
            {
                await SendAsync(new Message { Command = MessageCommand.RegisterPublisher, Body = _topic});
                _logger.LogInfo("Connected to Broker");
            }
            else
                _logger.LogError("Failed connection to Broker");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Connection-Exeption {ex.Message}");
            isConnected = false;
        }
    }

    public async Task SendAsync(Message message)
    {
        try
        {
            await _postman.SendPacketAsync(_socket, message);
        }
        catch(Exception ex) 
        {
            _logger.LogError($"Send-Exeption: {ex.Message}");
        }
    }
}
