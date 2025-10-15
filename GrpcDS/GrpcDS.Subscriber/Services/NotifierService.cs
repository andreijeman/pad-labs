using GrpcAgent;

namespace GrpcDS.Subscriber.Services;

public class NotifierService : Notifier.NotifierBase
{
    public override Task<NotifyReply> Notify(NotifyRequest request, Grpc.Core.ServerCallContext context)
    {
        Console.WriteLine(request.Content);
        return Task.FromResult(new NotifyReply { IsSuccess = true });
    }
}
