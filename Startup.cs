using MatchingClient.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Grpc.Net.Client;
using Roommanagement;
using ManageServer.Data;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {  
        // Enable controllers for the web API
        services.AddControllers();
        // gRPC server service
        services.AddGrpc();

        // Register radis connection and game room manager
        services.AddSingleton<IConnectionMultiplexer>(provider =>
        {
            var configuration = ConfigurationOptions.Parse("localhost:6379");
            var multiplexer = ConnectionMultiplexer.Connect(configuration);
            return multiplexer;
        });
        services.AddSingleton<IRedisCacheManager, RedisCacheManager>();
        
        // Register the database context
        services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite("Data Source=Registration.db"));
        
        // GrpcGameServerClient registration
        services.AddSingleton<GrpcGameServerClient>(provider =>
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = 
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            
            var channel = GrpcChannel.ForAddress("https://localhost:5255", new GrpcChannelOptions { HttpHandler = httpHandler });
            return new GrpcGameServerClient(channel);
        });
        services.AddScoped<RoomMatchManager>();
        services.AddSingleton<RoomManagementService.RoomManagementServiceBase, RoomManagementServiceImpl>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();  // Map controller routes
            endpoints.MapGrpcService<RoomManagementServiceImpl>();
        });
    }
}
