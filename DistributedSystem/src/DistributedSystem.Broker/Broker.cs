using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using DistributedSystem.Broker.Messages;
using DistributedSystem.Logger;
using DistributedSystem.Network;

namespace DistributedSystem.Broker;

public class Broker : IBroker
{
    private readonly ILogger _logger;
    private readonly IPostman<Message> _postman;
    
    private readonly Socket _socket;
    
    private readonly ConcurrentQueue<QueueItem> _queue = new();
    
    // For client with multiple connection here should be impl a list of sockets
    private readonly ConcurrentDictionary<string, Socket> _idSocketDict = new();
    
    private readonly ConcurrentDictionary<string, string> _idAliasDict = new();
    
    // Here ConcurrentDictionary<string, bool> is used just to avoid hustle with locker and list.
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, bool>> _pubSubDict = new();
    
    // !Not performant solution.
    private readonly ConcurrentDictionary<string, List<Message>> _idUnsendedMsgDict = new();
    
    
    public Broker(BrokerArgs args, ILogger logger, IPostman<Message> postman)
    {
        Args = args;
        _logger = logger;
        _postman = postman;
        
        _socket =  new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    }
    
    public BrokerArgs Args { get; }
    
    public void Start(CancellationToken cancellationToken = default)
    {
        _socket.Bind(new IPEndPoint(Args.IpAddress, Args.Port));
        _socket.Listen(Args.MaxConnections);

        _ = AcceptClientsAsync(cancellationToken);
        _ = HandleQueueAsync(cancellationToken);
    }
    
    private async Task AcceptClientsAsync(CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                Socket client = await _socket.AcceptAsync(cancellationToken);
                _ = AuthenticateClientAsync(client, cancellationToken);
                
                _logger.LogInfo($"Client <{client.RemoteEndPoint?.ToString()}> accepted");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }

    private async Task AuthenticateClientAsync(Socket client, CancellationToken cancellationToken = default)
    {
        try
        {
            var message = await _postman.ReceivePacketAsync(client);
            
            // Here can be implemented logic for password verification and more. At this point it just trust the client.
            if (message.Command == MessageCommand.Authenticate && _idSocketDict.TryAdd(message.Body, client))
            {
                _ = HandleClientAsync(client, message.Body, cancellationToken);
            }
            else
            {
                await _postman.SendPacketAsync(client,
                    new Message { Command = MessageCommand.Unauthorized, Body = string.Empty });
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    private async Task HandleClientAsync(Socket client, string clientId, CancellationToken cancellationToken)
    {
        await HandleUnsendedMsgAsync(client, clientId);
        
        while (!cancellationToken.IsCancellationRequested && client.Connected)
        {
            try
            {
                var message = await _postman.ReceivePacketAsync(client);
                _queue.Enqueue(new QueueItem(clientId, message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        if (!client.Connected) _idUnsendedMsgDict.TryAdd(clientId, new List<Message>());
    }

    private async Task HandleUnsendedMsgAsync(Socket client, string clientId)
    {
        if (_idUnsendedMsgDict.TryGetValue(clientId, out var messages))
        {
            foreach (var message in messages)
            {
               await _postman.SendPacketAsync(client, message);
            }
        }
        
        _idUnsendedMsgDict.TryRemove(clientId, out _);
    }

    private async Task HandleQueueAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (_queue.IsEmpty)
                await Task.Delay(Args.QueueHandlerDelay, cancellationToken);

            if (_queue.TryDequeue(out var item))
            {
                switch (item.Message.Command)
                {
                    case MessageCommand.Subscribe: 
                        HandleSubscribeCommand(item.SenderId, item.Message.Body);
                        break;
                    case MessageCommand.Unsubscribe:
                        HandleUnsubscribeCommand(item.SenderId, item.Message.Body);
                        break;
                    case MessageCommand.RegisterPublisher:
                        HandleRegisterPublisherCommand(item.SenderId, item.Message.Body);
                        break;
                    case MessageCommand.Publish:
                        HandlePublishCommand(item.SenderId, item.Message);
                        break;
                }
            }
        }
    }

    private void HandleSubscribeCommand(string subscriberId, string publisherAlias)
    { 
        if (_idAliasDict.TryGetValue(publisherAlias, out var publisherIdentifier))
        {
            _pubSubDict[publisherIdentifier].TryAdd(subscriberId, true);
        }
    }
    
    private void HandleUnsubscribeCommand(string subscriberId, string publisherAlias)
    {
        if (_idAliasDict.TryGetValue(publisherAlias, out var publisherIdentifier))
        {
            _pubSubDict[publisherIdentifier].TryRemove(subscriberId,  out _);
        }
    }
    
    public void HandleRegisterPublisherCommand(string publisherId, string alias)
    {
        if (_idAliasDict.TryAdd(publisherId, alias))
        {
            _pubSubDict.TryAdd(publisherId, new());
        }
    }
    
    public void HandlePublishCommand(string publisherId, Message message)
    {
        foreach (var identifier in _pubSubDict[publisherId])
        {
            _postman.SendPacketAsync(_idSocketDict[identifier.Key], message);
        }
        
        foreach (var messages in _idUnsendedMsgDict.Values)
        {
            messages.Add(message);
        }
    }
}