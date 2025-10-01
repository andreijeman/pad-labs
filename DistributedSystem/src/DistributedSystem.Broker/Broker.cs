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
    
    public Broker(ILogger logger, IPostman<Message> postman)
    {
        _logger = logger;
        _postman = postman;
        
        _socket =  new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    }
    
    public void Start(BrokerArgs args, CancellationToken ct = default)
    {
        _socket.Bind(new IPEndPoint(args.IpAddress, args.Port));
        _socket.Listen(args.MaxConnections);

        _ = AcceptClientsAsync(ct);
        _ = HandleQueueAsync(args.QueueHandlerDelay, ct);
    }
    
    private async Task AcceptClientsAsync(CancellationToken ct = default)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                Socket client = await _socket.AcceptAsync(ct);
                _ = AuthenticateClientAsync(client, ct);
                
                _logger.LogInfo($"Socket <{client.RemoteEndPoint}> accepted.");
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }
    }

    private async Task AuthenticateClientAsync(Socket client, CancellationToken ct = default)
    {
        try
        {
            var message = await _postman.ReceivePacketAsync(client);
            
            // Here can be implemented logic for password verification and more. At this point it just trust the client.
            if (message.Code == MessageCode.Authenticate)
            {
                _logger.LogInfo($"Socket <{client.RemoteEndPoint}> authenticated as <{message.Body}>.");
                
                if(!_idSocketDict.TryAdd(message.Body, client)) _idSocketDict[message.Body] = client;
                
                await SendMessageAsync(client, new Message { Code = MessageCode.Ok, Body = "Authentication succeeded. I see you (-_-)." });
                _ = HandleUnsentMsgAsync(client, message.Body);
                _ = HandleClientAsync(client, message.Body, ct);
            }
            else
            {
                _logger.LogInfo($"Socket <{client.RemoteEndPoint}> failed authentication.");
                
                _ = SendMessageAsync(client, new Message { Code = MessageCode.Fail, Body = "Identifier is already in use." });
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
        }
    }
    
    private async Task HandleClientAsync(Socket client, string clientId, CancellationToken ct)
    {
        while (!ct.IsCancellationRequested && client.Connected)
        {
            try
            {
                var message = await _postman.ReceivePacketAsync(client);
                _queue.Enqueue(new QueueItem(clientId, message));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }

        if (!client.Connected)
            _idUnsendedMsgDict.TryAdd(clientId, new List<Message>());
    }

    private async Task HandleUnsentMsgAsync(Socket client, string clientId)
    {
        if (_idUnsendedMsgDict.TryGetValue(clientId, out var messages))
        {
            foreach (var message in messages)
            {
                _logger.LogInfo($"Send unsent message to <{clientId}>: {message.Body}");
                await SendMessageAsync(client, message);
                
                // Here seems to arise a bug.
                // Messages sometimes are sent too fast so they are glued together and JsonCodec cannot deserialize it.
                // Delay is a temp solution. Or not =) ?
                await Task.Delay(50);
            }
        }
        
        _idUnsendedMsgDict.TryRemove(clientId, out _);
    }

    private async Task HandleQueueAsync(int delay, CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            if (_queue.IsEmpty)
                await Task.Delay(delay, ct);

            if (_queue.TryDequeue(out var item))
            {
                switch (item.Message.Code)
                {
                    case MessageCode.Subscribe: 
                        HandleSubscribeCommand(item.SenderId, item.Message.Body);
                        break;
                    case MessageCode.Unsubscribe:
                        HandleUnsubscribeCommand(item.SenderId, item.Message.Body);
                        break;
                    case MessageCode.RegisterPublisher:
                        HandleRegisterPublisherCommand(item.SenderId, item.Message.Body);
                        break;
                    case MessageCode.Publish:
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
            if (_pubSubDict[publisherIdentifier].TryAdd(subscriberId, true))
            {
                _logger.LogInfo($"Client <{subscriberId}> subscribed to <{publisherAlias}>.");
                _ = SendMessageAsync(_idSocketDict[subscriberId], new Message { Code = MessageCode.Ok, Body = "Subscription succeeded"});
            }
            else _ = SendMessageAsync(_idSocketDict[subscriberId], new Message { Code = MessageCode.Ok, Body = "Already subscribed" });
        }
        else
        {
            _ = SendMessageAsync(_idSocketDict[subscriberId], new Message { Code = MessageCode.Fail, Body = "Subscription failed" });
        }
    }
    
    private void HandleUnsubscribeCommand(string subscriberId, string publisherAlias)
    {
        if (_idAliasDict.TryGetValue(publisherAlias, out var publisherIdentifier))
        {
            if (_pubSubDict[publisherIdentifier].TryRemove(subscriberId, out _))
            {
                _logger.LogInfo($"Client <{subscriberId}> unsubscribed from <{publisherAlias}>.");
                _ = SendMessageAsync(_idSocketDict[subscriberId], new Message { Code = MessageCode.Ok, Body = "Unsubscription succeeded" });
            }
            else _ = SendMessageAsync(_idSocketDict[subscriberId], new Message { Code = MessageCode.Ok, Body = "Not subscribed to this publisher" });
            
            
        }
        else
        {
            _ = SendMessageAsync(_idSocketDict[subscriberId], new Message { Code = MessageCode.Fail, Body = "Unsubscription succeeded" });
        }
    }
    
    private void HandleRegisterPublisherCommand(string publisherId, string alias)
    {
        if (_idAliasDict.TryAdd(publisherId, alias))
        {
            _pubSubDict.TryAdd(publisherId, new());
            
            _logger.LogInfo($"Client <{publisherId}> registered as publisher <{alias}>.");
            
            _ = SendMessageAsync(_idSocketDict[publisherId], new Message { Code = MessageCode.Ok, Body = "Publisher alias registration succeeded" });
        }
        else
        {
            _ = SendMessageAsync(_idSocketDict[publisherId], new Message { Code = MessageCode.Fail });
        }
    }
    
    private void HandlePublishCommand(string publisherId, Message message)
    {
        _logger.LogInfo($"Publisher <{publisherId}> published <{message.Body}>.");
        
        foreach (var identifier in _pubSubDict[publisherId].Keys)
        {
            var socket = _idSocketDict[identifier];
            if (socket.Connected)
                _ = SendMessageAsync(_idSocketDict[identifier], message);
        }
        
        foreach (var messages in _idUnsendedMsgDict.Values)
        {
            messages.Add(message);
        }
    }

    private async Task SendMessageAsync(Socket client, Message message)
    {
        try
        {
            await _postman.SendPacketAsync(client, message);
        }
        catch(Exception e)
        {
            _logger.LogError(e.Message);
        }
    }
}