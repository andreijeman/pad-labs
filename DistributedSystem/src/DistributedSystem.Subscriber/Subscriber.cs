using DistributedSystem.Broker.Messages;
using System.Net.Sockets;
using DistributedSystem.Logger;
using DistributedSystem.Network;

namespace DistributedSystem.Subscriber;

public class Subscriber : ISubscriber
{
    private readonly IPostman<Message> _postman;
    private readonly ILogger _logger;

    private readonly Socket _socket;

    private bool isConnected = false;

    private string _topic = String.Empty;
    private string _name;

    public Subscriber(IPostman<Message> postman, ILogger logger, string name)
    {
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _postman = postman;
        _logger = logger;
        _name = name;
    }

    public async Task ConnectAsync(ConnectionArgs configuration)
    {
        try
        {
            await _socket.ConnectAsync(configuration.IpAddress, configuration.Port);
            
            if (_socket.Connected)
            {
                await AuthenticateAsync();
                await CheckAuthentication();
            }
            else
                _logger.LogError("Failed connection to Broker");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Connection-Exeption {ex.Message}");
        }
    }

    private async Task CheckAuthentication()
    {
        try
        {
            var result = await _postman.ReceivePacketAsync(_socket);
            switch (result.Code)
            {
                case MessageCode.Ok:
                    isConnected = true;
                    _logger.LogInfo(result.Body);
                    return;

                case MessageCode.Fail:
                    isConnected = false;
                    _logger.LogWarning(result.Body);
                    CloseConnection();
                    return;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Authentication-Exception: {ex.Message}");
            isConnected = false;
        }
    }

    private async Task AuthenticateAsync()
    {
        var message = new Message { Code = MessageCode.Authenticate, Body = _name };
        await _postman.SendPacketAsync(_socket, message);
    }

    private async Task SubscribeAsync()
    {
        var message = new Message { Code = MessageCode.Subscribe, Body = _topic };
        await _postman.SendPacketAsync(_socket, message);
    }

    public async Task StartReceiveAsync()
    {
        try
        {
            while (true)
            {
                Message message = await _postman.ReceivePacketAsync(_socket);

                if (message is null) break;

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
        var message = new Message { Code = MessageCode.Unsubscribe, Body = topic };
        await _postman.SendPacketAsync(_socket, message);
    }

    public bool IsConnected() => isConnected;

    private void CloseConnection()
    {
        try
        {
            if (isConnected)
            {
                _socket.Shutdown(SocketShutdown.Both);
                _logger.LogInfo($"Closing connection to {_socket.RemoteEndPoint}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Connection-Exeption: {ex.Message}");
        }
    }
}
