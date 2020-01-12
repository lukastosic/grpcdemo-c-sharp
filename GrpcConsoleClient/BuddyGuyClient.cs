using Grpc.Core;
using Grpc.Net.Client;
using GrpcServer.BuddyGuy;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
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
        /// <param name="duration">Duration in miliseconds on how long the banter should last</param>
        /// <returns></returns>
        public async Task StartBuddyGuy(long duration)
        {
            using (var call = buddyGuyClient.ImNotYour())
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                Console.WriteLine($"Enjoy {duration/1000} seconds of banter :)");

                BuddyRequest request = new BuddyRequest { Message = "friend" };

                Console.WriteLine($"Client: You'll regret this day {request.Message}");
                
                // Initiate stream with first request
                await call.RequestStream.WriteAsync(request);

                // Define what to do when response is received
                var responseReaderTask = Task.Run(async () =>
                {
                    while (await call.ResponseStream.MoveNext())
                    {
                        // Receive answer from server
                        Console.WriteLine($"Server: I'm not your {request.Message} {call.ResponseStream.Current.Message}");

                        await Task.Delay(1000);

                        if (stopwatch.ElapsedMilliseconds < duration)
                        {
                            // Prepare new request
                            request = new BuddyRequest { Message = BuddyGuyDictionary[call.ResponseStream.Current.Message] };
                            Console.WriteLine($"Client: I'm not your {call.ResponseStream.Current.Message} {request.Message}");

                            await call.RequestStream.WriteAsync(request);
                        }
                        else
                        {   
                            Console.WriteLine($"{duration/1000} seconds have passed .. stopping the banter");

                            // Close request stream to notify server that there are no more requests
                            await call.RequestStream.CompleteAsync();
                        }
                    }
                });

                // Start listening for responses
                await responseReaderTask;                  
            }

            Console.WriteLine("\r\nPress ENTER to return to main menu");
            Console.ReadLine();
        }
    }
}
