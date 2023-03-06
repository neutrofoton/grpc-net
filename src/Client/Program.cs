using Grpc.Core;
using Shared.GRpc.Dummy;
using Shared.GRpc.Greet;

const string ip = "127.0.0.1";
const int port = 50051;

Channel channel = new Channel(ip, port, ChannelCredentials.Insecure);
channel.ConnectAsync().ContinueWith(task =>
{
    if (task.Status == TaskStatus.RanToCompletion)
        Console.WriteLine($"The client connected successfully on {ip}:{port}");
}).Wait();

var clientDummy = new DummyService.DummyServiceClient(channel);

//unary
var clientUnary=new GreetingService.GreetingServiceClient(channel);
var hello = clientUnary.GreetAsync(new GreetingRequest()
{
    Greeting = new Greeting()
    {
        FirstName = "neutro",
        LastName = "foton"
    }
}).GetAwaiter().GetResult().Result;
Console.WriteLine($"Grpc Unary : {hello}");


//server streaming
var clientServerStreaming = new GreetingService.GreetingServiceClient(channel);
var responseStream = clientServerStreaming.GreetManyTimes(new GreetingManyTimesRequest()
{
    Greeting = new Greeting()
    {
        FirstName = "neutro",
        LastName = "foton"
    }
});

while(await responseStream.ResponseStream.MoveNext())
{
    Console.WriteLine(responseStream.ResponseStream.Current.Result);
}


channel.ShutdownAsync().Wait();

Console.ReadLine();