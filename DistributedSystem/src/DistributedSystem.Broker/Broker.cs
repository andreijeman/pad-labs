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
    
    private readonly ConcurrentDictionary<Socket, ConcurrentBag<Socket>> _publisherSubscriberDict;
    private readonly ConcurrentDictionary<string, Socket> _identifierPublisherDict;
    
    private readonly ConcurrentQueue<BrokerQueueItem> _queue;

    public Broker(BrokerArgs args, ILogger logger, IPostman<Message> postman)
    {
        Args = args;
        _logger = logger;
        _postman = postman;
        
        _socket =  new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _publisherSubscriberDict = new ConcurrentDictionary<Socket, ConcurrentBag<Socket>>();
        _identifierPublisherDict = new ConcurrentDictionary<string, Socket>();
        _queue = new ConcurrentQueue<BrokerQueueItem>();
    }
    
    public BrokerArgs Args { get; }
    
    public void Start(CancellationToken cancellationToken = default)
    {
        _socket.Bind(new IPEndPoint(Args.IpAddress, Args.Port));
        _socket.Listen(Args.MaxConnections);

        _ = AcceptClientsAsync(HandleClientAsync, cancellationToken);
    }
    
    private async Task AcceptClientsAsync(Func<Socket, CancellationToken, Task> handleClientAsync, CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                Socket client = await _socket.AcceptAsync(cancellationToken);
                _ = handleClientAsync(client, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }

    private async Task HandleMessageAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested && !_queue.IsEmpty)
        {
            if (_queue.TryDequeue(out var item))
            {
            }
        }
        
        await Task.Delay(Args.MessageQueueHandlerSleepTime);
        
        _ = HandleMessageAsync(cancellationToken);
    }

    private async Task HandleClientAsync(Socket client, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested && client.Connected)
        {
            try
            {
                var message = await _postman.ReceivePacketAsync(client);
                _queue.Enqueue(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }

    public void HandleSubscribeCommand(Socket subscriber, string publisherIdentifier)
    {
        if (_identifierPublisherDict.ContainsKey(publisherIdentifier))
        {
            var publisher = _identifierPublisherDict[publisherIdentifier];
            
            if(!_publisherSubscriberDict[publisher].Contains(subscriber))
                _publisherSubscriberDict[publisher].Add(subscriber);
        }
    }
    
    
    private async Task HandlePublisherAsync(Socket publisher, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            while (!cancellationToken.IsCancellationRequested && publisher.Connected)
            {
                try
                {
                    var message = await _postman.ReceivePacketAsync(publisher);
                
                    switch (message.Type)
                    {
                        case PublisherMessageType.SetNameCommand: 
                            HandleSetPublisherNameCommand(publisher, message.Body); 
                            break;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
            }
        }
    }
    
    public void HandleSetPublisherNameCommand(Socket publisher, string publisherIdentifier)
    {
        // todo: handle key updating.
        _identifierPublisherDict.TryAdd(publisherIdentifier, publisher);
        _publisherSubscriberDict.TryAdd(publisher, new ConcurrentBag<Socket>());
    }
    
    
    
}