
using DistributedSystem.Broker.Messages;
using DistributedSystem.Logger;
using DistributedSystem.Network;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

namespace DistributedSystem.Subscriber;

public class Subscriber : ISubscriber
{
    private readonly Socket _socket;
    private readonly Postman<Message> _postman;
    private readonly ILogger _logger;

    public bool isConnected = false;
    private string _topic;

    public Subscriber(ILogger logger, string topic)
    {
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _postman = new Postman<Message>(new JsonCodec<Message>());
        _logger = logger;
        _topic = topic;
    }

    public async Task ConnectAsync(string ip, int port)
    {
        try
        {
            await _socket.ConnectAsync(ip, port);
            isConnected = _socket.Connected;

            if (isConnected)
            {
                await SubscribeAsync();
                _logger.LogInfo("Connected to Broker");
            }
            else
                _logger.LogError("Failed connection to Broker");
        }
        catch(Exception ex)
        {
            _logger.LogError($"Connection-Exeption {ex.Message}");
        }
    }

    private async Task SubscribeAsync()
    {
        var message = new Message { Command = MessageCommand.Subscribe, Body = _topic };
        await _postman.SendPacketAsync(_socket, message);
    }

    public async Task StartReceiveAsync()
    {
        try
        {
            while (true)
            {
                Message message = await _postman.ReceivePacketAsync(_socket);

                if (message is null)  break;

                MessageHandler.Handler(message);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Receive-Exeption: {ex.Message}");
        }
        finally
        {
            CloseConnection();
        }
    }

    public async Task ChangeTopicAsync(string topic)
    {
        await UnsubscribeAsync(topic);
        _topic = topic;
        await SubscribeAsync();
    }
    
    private async Task UnsubscribeAsync(string topic)
    {
        var message = new Message { Command = MessageCommand.Unsubscribe, Body = topic };
        await _postman.SendPacketAsync(_socket, message);
    }

    private void CloseConnection()
    {
        try
        {
            if(isConnected)
            {
                _socket.Shutdown(SocketShutdown.Both);
                _logger.LogInfo($"Closing connection to {_socket.RemoteEndPoint}");
            }
        }
        catch(Exception ex)
        {
            _logger.LogError($"Connection-Exeption: {ex.Message}");
        }
    }
}
