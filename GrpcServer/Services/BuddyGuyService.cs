using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using GrpcServer.BuddyGuy;

namespace GrpcServer.Services
{
    /// <summary>
    /// Sample of bi-directional streaming
    /// Having a litle fun with famous South Park "I'm not your friend buddy" banter
    /// </summary>
    public class BuddyGuyService : BuddyGuy.BuddyGuy.BuddyGuyBase
    {
        Dictionary<string, string> BuddyGuyDictionary = new Dictionary<string, string>
            {
                {"friend", "buddy"},
                {"buddy", "guy" },
                {"guy", "friend" }
            };

        public override async Task ImNotYour(IAsyncStreamReader<BuddyRequest> requestStream, IServerStreamWriter<GuyResponse> responseStream, ServerCallContext context)
        {
            while (await requestStream.MoveNext())
            {
                GuyResponse response = new GuyResponse();
                response.Message = BuddyGuyDictionary[requestStream.Current.Message];
                await Task.Delay(1000);
                await responseStream.WriteAsync(response);
            }
        }
    }
}
