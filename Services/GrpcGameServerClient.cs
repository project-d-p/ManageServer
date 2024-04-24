using System;
using System.Threading.Tasks;
using Grpc.Core;
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

        public async Task<Room> CreateChannelAsync()
        {
            try
            {
                var response = await _client.CreateChannelAsync(new Empty());
                Room newRoom = new Room
                {
                    RoomId = response.ChannelId,
                    IP = response.UdpIp,
                    UdpPort = response.UdpPort,
                    TcpPort = response.TcpPort
                };
                return newRoom;
            }
            catch (RpcException rpcEx)
            {
                Console.WriteLine($"RPC Error: {rpcEx.StatusCode} - {rpcEx.Message}");
                throw; // Re-throwing can be replaced with more sophisticated error handling
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                throw; // Ensure all exceptions are caught and logged
            }
        }

        public async Task<Matching.ResponseLaunch> AttachPlayerAsync(Matching.RequestLaunch request)
        {
            try
            {
                var response = await _client.AttachPlayerAsync(request);
                return response;
            }
            catch (RpcException rpcEx)
            {
                Console.WriteLine($"RPC Error: {rpcEx.StatusCode} - {rpcEx.Message}");
                throw; // Re-throwing can be replaced with more sophisticated error handling
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                throw; // Ensure all exceptions are caught and logged
            }
        }
    }
}
