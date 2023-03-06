using Grpc.Core;


const string ip = "127.0.0.1";
const int port = 50051;
Server? server = null;

try
{
    server = new Server()
    {
        Ports = {
        new ServerPort(ip, port, ServerCredentials.Insecure)
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