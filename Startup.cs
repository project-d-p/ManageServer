using MatchingClient.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {  
        // Enable controllers for the web API
        services.AddControllers();

        // Register radis connection and game room manager
        services.AddSingleton<IConnectionMultiplexer>(provider =>
        {
            var configuration = ConfigurationOptions.Parse("localhost:6379");
            var multiplexer = ConnectionMultiplexer.Connect(configuration);
            return multiplexer;
        });
        services.AddSingleton<IRedisCacheManager, RedisCacheManager>();
        
        // GrpcGameServerClient registration
        services.AddSingleton<GrpcGameServerClient>(provider =>
        {
            var serverAddress = "http://localhost:5001"; // 직접 지정한 서버 주소
            return new GrpcGameServerClient(serverAddress);
        });
        services.AddScoped<RoomMatchManager>();
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
        });
    }
}
