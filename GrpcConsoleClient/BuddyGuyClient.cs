using Grpc.Core;
using Grpc.Net.Client;
using GrpcServer.BuddyGuy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrpcConsoleClient
{
    public class BuddyGuyClient
    {
        public BuddyGuyClient(string serverUrl)
        {
            GrpcChannel buddyGuyChannel = GrpcChannel.ForAddress(serverUrl);
            buddyGuyClient = new BuddyGuy.BuddyGuyClient(buddyGuyChannel);
        }

        private BuddyGuy.BuddyGuyClient buddyGuyClient;

        private Dictionary<string, string> BuddyGuyDictionary = new Dictionary<string, string>
            {
                {"friend", "buddy"},
                {"buddy", "guy" },
                {"guy", "friend" }
            };

        /// <summary>
        /// Initiate "I'm not your buddy guy" banter
        /// </summary>
        /// <returns></returns>
        public async Task StartBuddyGuy()
        {
            using(var call = buddyGuyClient.ImNotYour())
            {
                BuddyRequest request = new BuddyRequest { Message = "friend" };

                Console.WriteLine($"Client: You'll regret this day {request.Message}");

                await call.RequestStream.WriteAsync(request);

                var responseReaderTask = Task.Run(async () =>
                {
                    while (await call.ResponseStream.MoveNext())
                    {
                        // Receive answer from server
                        Console.WriteLine($"Server: I'm not your {request.Message} {call.ResponseStream.Current.Message}");
                        
                        await Task.Delay(1000);

                        // Prepare new request
                        request = new BuddyRequest { Message = BuddyGuyDictionary[call.ResponseStream.Current.Message] };
                        Console.WriteLine($"Client: I'm not your {call.ResponseStream.Current.Message} {request.Message}");
                        
                        await call.RequestStream.WriteAsync(request);
                    }
                });

                await responseReaderTask;
                
                // This is a proper way to close down request stream
                // but because reading response from server generates new request
                // this line will never be reached
                await call.RequestStream.CompleteAsync();
            }
        }
    }
}
