using System;
using System.Threading.Tasks;
using Grpc.Net.Client;
using MatchingClient.Models;
using Matching;

namespace MatchingClient.Services
{   
    public class GrpcGameServerClient
    {
        private readonly GrpcChannel _channel;
        private readonly MatchingService.MatchingServiceClient _client;

        public GrpcGameServerClient(string serverAddress)
        {
            _channel = GrpcChannel.ForAddress(serverAddress);
            _client = new MatchingService.MatchingServiceClient(_channel);
        }

        public async Task<ChannelResponse> CreateChannelAsync()
        {
            try
            {
                var response = await _client.CreateChannelAsync(new Empty());
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }

        public async Task<ResponseLaunch> AttachPlayerAsync(RequestLaunch request)
        {
            try
            {
                var response = await _client.AttachPlayerAsync(request);
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
    }
}
