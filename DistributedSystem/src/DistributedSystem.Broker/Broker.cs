using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using DistributedSystem.Logger;
using DistributedSystem.Network;

namespace DistributedSystem.Broker;

public class Broker : IBroker
{
    private readonly ILogger _logger;
    
    private readonly Socket _publisherSocket;
    private readonly Socket _subscriberSocket;
    
    private readonly ConcurrentDictionary<Socket, Socket> _publisherSubscriberDict;
    private readonly ConcurrentDictionary<string, Socket> _identifierPublisherDict;

    public Broker(BrokerArgs args, ILogger logger)
    {
        Args = args;
        _logger = logger;
        
        _publisherSocket =  new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _subscriberSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _publisherSubscriberDict = new ConcurrentDictionary<Socket, Socket>();
        _identifierPublisherDict = new ConcurrentDictionary<string, Socket>();
    }
    
    public BrokerArgs Args { get; }
    
    public void Start(CancellationToken cancellationToken = default)
    {
        _publisherSocket.Bind(new IPEndPoint(Args.IpAddress, Args.PublisherPort));
        _publisherSocket.Bind(new IPEndPoint(Args.IpAddress, Args.SubscriberPort));
        
        _publisherSocket.Listen(Args.PublisherMaxConnections);
        _subscriberSocket.Listen(Args.SubscriberMaxConnections);

        _ = AcceptClientsAsync(HandleSubcriberAsync, cancellationToken);
        _ = AcceptClientsAsync(HandlePublisherAsync, cancellationToken);
    }
    
    private async Task AcceptClientsAsync(Func<Socket, CancellationToken, Task> handleClientAsync, CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                Socket client = await _subscriberSocket.AcceptAsync(cancellationToken);
                _ = handleClientAsync(client, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    } 

    private async Task HandleSubcriberAsync(Socket subscriber, CancellationToken cancellationToken)
    {
        
    }
    
    
    private async Task HandlePublisherAsync(Socket publisher, CancellationToken cancellationToken)
    {
        
    }
    
    
    
    
}