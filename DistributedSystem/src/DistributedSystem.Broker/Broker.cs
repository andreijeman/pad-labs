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
    
    private readonly ConcurrentQueue<BrokerQueueItem> _queue;
    
    private readonly Dictionary<Socket, List<Socket>> _publisherSubscriberDict;
    private readonly Dictionary<string, Socket> _identifierPublisherDict;

    public Broker(BrokerArgs args, ILogger logger, IPostman<Message> postman)
    {
        Args = args;
        _logger = logger;
        _postman = postman;
        
        _socket =  new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _publisherSubscriberDict = new Dictionary<Socket, List<Socket>>();
        _identifierPublisherDict = new Dictionary<string, Socket>();
        _queue = new ConcurrentQueue<BrokerQueueItem>();
    }
    
    public BrokerArgs Args { get; }
    
    public void Start(CancellationToken cancellationToken = default)
    {
        _socket.Bind(new IPEndPoint(Args.IpAddress, Args.Port));
        _socket.Listen(Args.MaxConnections);

        _ = AcceptClientsAsync(cancellationToken);
    }
    
    private async Task AcceptClientsAsync(CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                Socket client = await _socket.AcceptAsync(cancellationToken);
                _ = HandleClientAsync(client, cancellationToken);
                
                _logger.LogInfo($"Client {client.GetIp()} accepted");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
    
    private async Task HandleClientAsync(Socket client, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested && client.Connected)
        {
            try
            {
                var message = await _postman.ReceivePacketAsync(client);
                _queue.Enqueue(new BrokerQueueItem(client, message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
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
                        HandleSubscribeCommand(item.Sender, item.Message.Body);
                        break;
                    case MessageCommand.Unsubscribe:
                        HandleUnsubscribeCommand(item.Sender, item.Message.Body);
                        break;
                    case MessageCommand.RegisterPublisher:
                        HandleRegisterPublisherCommand(item.Sender, item.Message.Body);
                        break;
                    case MessageCommand.Publish:
                        HandlePublishCommand(item.Sender, item.Message);
                        break;
                }
            }
        }
    }

    private void HandleSubscribeCommand(Socket subscriber, string publisherIdentifier)
    {
        if (_identifierPublisherDict.TryGetValue(publisherIdentifier, out var publisher))
        {
            if(!_publisherSubscriberDict[publisher].Contains(subscriber))
                _publisherSubscriberDict[publisher].Add(subscriber);
        }
    }
    
    private void HandleUnsubscribeCommand(Socket subscriber, string publisherIdentifier)
    {
        if (_identifierPublisherDict.TryGetValue(publisherIdentifier, out var publisher))
        {
            if(!_publisherSubscriberDict[publisher].Contains(subscriber))
                _publisherSubscriberDict[publisher].Remove(subscriber);
        }
    }
    
    public void HandleRegisterPublisherCommand(Socket publisher, string publisherIdentifier)
    {
        if (_publisherSubscriberDict.TryAdd(publisher, new  List<Socket>()))
        {
            _identifierPublisherDict.Add(publisherIdentifier, publisher);
        }
    }
    
    public void HandlePublishCommand(Socket publisher, Message message)
    {
        //todo
    }
}