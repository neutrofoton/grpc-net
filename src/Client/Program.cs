using FizzWare.NBuilder;
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


//parameter input
var greetingRequest = new GreetingRequest()
{
    Greeting = new Greeting()
    {
        FirstName = "neutro",
        LastName = "foton"
    }
};

var grpcApi = new GreetingService.GreetingServiceClient(channel);

//unary streaming
Console.WriteLine($"=============={"unary streaming"}==========");
AsyncUnaryCall<GreetingResponse> unaryStreaming = grpcApi.GreetAsync(greetingRequest);

var hello = unaryStreaming.GetAwaiter().GetResult().Result;
Console.WriteLine($"{Environment.NewLine}Grpc Unary : {hello}");


//server streaming
Console.WriteLine($"=============={"server streaming"}==========");
AsyncServerStreamingCall<GreetingResponse> serverStreaming = grpcApi.GreetOneRequestMultiResponse(greetingRequest);

Console.WriteLine($"{Environment.NewLine}Grpc Server Streaming :");
while (await serverStreaming.ResponseStream.MoveNext())
{
    Console.WriteLine(serverStreaming.ResponseStream.Current.Result);
}


//client streaming
Console.WriteLine($"=============={"client streaming"}==========");
AsyncClientStreamingCall<GreetingRequest, GreetingResponse> clientStreaming = grpcApi.GreetMultiRequestOneResponse();

foreach (int i in Enumerable.Range(1, 10))
{
    await clientStreaming.RequestStream.WriteAsync(greetingRequest);
}

await clientStreaming.RequestStream.CompleteAsync();
Console.WriteLine(clientStreaming.ResponseAsync.Result);

//bidirectional streaming
Console.WriteLine($"=============={"bidirectional streaming"}==========");
AsyncDuplexStreamingCall<GreetingRequest, GreetingResponse> bidirectionalStreaming = grpcApi.GreetMultiRequestMultiResponse();

var responseReaderTask = Task.Run(async () =>
{
    while (await bidirectionalStreaming.ResponseStream.MoveNext())
    {
        Console.WriteLine($"Received: {bidirectionalStreaming.ResponseStream.Current.Result}");
    }
});

IList<Greeting> greetings = Builder<Greeting>.CreateListOfSize(10).Build();
foreach (var g in greetings)
{
    Console.WriteLine($"Sending: {g.Index} | {g.FirstName} {g.LastName}");
    await bidirectionalStreaming.RequestStream.WriteAsync(new GreetingRequest()
    {
        Greeting = g
    });
}

await bidirectionalStreaming.RequestStream.CompleteAsync();

//=======================================================
channel.ShutdownAsync().Wait();
Console.ReadLine();