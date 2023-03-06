using Server.Services;
using Shared.GRpc.Greet;
using gRpc = Grpc.Core;


const string ip = "127.0.0.1";
const int port = 50051;
gRpc.Server? server = null;

try
{
    server = new gRpc.Server()
    {
        Ports = 
        {
            new gRpc.ServerPort(ip, port, gRpc.ServerCredentials.Insecure)
        },
        Services =
        {
            GreetingService.BindService(new GreetingServiceImpl())
        }
        
    };
    server.Start();

    Console.WriteLine($"The server is listening on {ip}:{port}");
    Console.ReadLine();
}
catch(Exception ex)
{
    Console.WriteLine(ex.ToString());
}
finally
{
    if (server != null)
    {
        server.ShutdownAsync().Wait();
    }
}