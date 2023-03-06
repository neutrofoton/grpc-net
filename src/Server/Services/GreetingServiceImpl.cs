using Grpc.Core;
using Shared.GRpc.Greet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Shared.GRpc.Greet.GreetingService;

namespace Server.Services
{
    public class GreetingServiceImpl : GreetingServiceBase
    {
        public override Task<GreetingResponse> Greet(GreetingRequest request, ServerCallContext context)
        {
            string hello = $"hello {request.Greeting.FirstName} {request.Greeting.LastName}";
            return Task.FromResult(new GreetingResponse()
            {
                Result = hello
            });
        }
        public override async Task GreetManyTimes(GreetingManyTimesRequest request, IServerStreamWriter<GreetingManyTimesResponse> responseStream, ServerCallContext context)
        {
            Console.WriteLine("The server received a request : ");
            Console.WriteLine(request.ToString());

            string hello = $"hello {request.Greeting.FirstName} {request.Greeting.LastName}";
            foreach(int i in Enumerable.Range(1,10))
            {
                await responseStream.WriteAsync(new GreetingManyTimesResponse()
                {
                    Result = $"{i} - {hello}"
                });
            }
        }
    }
}
