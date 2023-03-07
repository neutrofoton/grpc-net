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

        //server streaming
        public override async Task GreetOneRequestMultiResponse(GreetingRequest request, IServerStreamWriter<GreetingResponse> responseStream, ServerCallContext context)
        {
            Console.WriteLine("[GreetOneRequestManyTimesResponse] The server received a request : ");
            Console.WriteLine(request.ToString());

            string hello = $"hello {request.Greeting.FirstName} {request.Greeting.LastName}";
            foreach (int i in Enumerable.Range(1, 10))
            {
                await responseStream.WriteAsync(new GreetingResponse()
                {
                    Result = $"{i} - {hello}"
                });
            }
        }

        public override async Task<GreetingResponse> GreetMultiRequestOneResponse(IAsyncStreamReader<GreetingRequest> requestStream, ServerCallContext context)
        {
            Console.WriteLine("On GreetMultiRequestOneResponse");

            string result = string.Empty;
            while(await requestStream.MoveNext())
            {
                var from = $"{requestStream.Current.Greeting.FirstName} {requestStream.Current.Greeting.LastName}";
                Console.WriteLine(from);

                result += $"Hello {from} {Environment.NewLine}";
            }

            return new GreetingResponse()
            {
                Result = result
            };
        }

        public override async Task GreetMultiRequestMultiResponse(IAsyncStreamReader<GreetingRequest> requestStream, IServerStreamWriter<GreetingResponse> responseStream, ServerCallContext context)
        {
            while(await requestStream.MoveNext())
            {
                var hello = $"[{requestStream.Current.Greeting.Index}] => Hello {requestStream.Current.Greeting.FirstName} {requestStream.Current.Greeting.LastName}";
                Console.WriteLine(hello);

                await responseStream.WriteAsync(new GreetingResponse() { Result = hello });
            }
        }
    }
}
