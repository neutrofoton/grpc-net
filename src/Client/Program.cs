using Grpc.Core;
using Shared.GRpc.Dummy;

const string ip = "127.0.0.1";
const int port = 50051;

Channel channel = new Channel(ip, port, ChannelCredentials.Insecure);
channel.ConnectAsync().ContinueWith(task =>
{
    if (task.Status == TaskStatus.RanToCompletion)
        Console.WriteLine($"The client connected successfully on {ip}:{port}");
}).Wait();

var client = new DummyService.DummyServiceClient(channel);
channel.ShutdownAsync().Wait();

Console.ReadLine();