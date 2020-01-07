using GreetServer;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrpcConsoleClient
{
    public class GreetClient
    {
        public GreetClient(string serverUrl)
        {
            GrpcChannel greetChannel = GrpcChannel.ForAddress(serverUrl);
            client = new Greeter.GreeterClient(greetChannel);
        }

        private Greeter.GreeterClient client;

        public async Task<string> DoTheGreet(string name)
        {
            HelloRequest request = new HelloRequest
            {
                Name = name
            };

            try
            {
                var response = await client.SayHelloAsync(request);
                return response.Message;
            }
            catch(RpcException ex)
            {
                Console.WriteLine($"Error making GRPC request: {ex}");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }

            return null;
        }
    }
}
