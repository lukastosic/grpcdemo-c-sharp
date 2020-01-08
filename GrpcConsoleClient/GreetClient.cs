using GreetServer;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrpcConsoleClient
{
    /// <summary>
    /// Basic class for "Greet" sample. This is basic gRPC call sample.
    /// </summary>
    public class GreetClient
    {
        public GreetClient(string serverUrl)
        {
            GrpcChannel greetChannel = GrpcChannel.ForAddress(serverUrl);
            client = new Greeter.GreeterClient(greetChannel);
        }

        private Greeter.GreeterClient client;

        /// <summary>
        /// Basic client implementation of simple gRPC
        /// Overall it has 3 steps:
        /// 1. Initiate client (in the constructor)
        /// 2. Prepare request object (as defined in proto file)
        /// 3. Execute rpc and handle response (as defined in proto file)
        /// </summary>
        /// <param name="name">Name that will be sent as a request</param>
        /// <returns>Message that is received back from rpc</returns>
        public async Task<string> DoTheGreet(string name)
        {
            // Prepare rpc request object
            HelloRequest request = new HelloRequest
            {
                Name = name
            };

            try
            {
                // Execute rpc and handle response
                HelloReply response = await client.SayHelloAsync(request);
                return response.Message;
            }
            catch(RpcException ex)
            {
                // Any exception coming from RPC will be in object RpcException
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
