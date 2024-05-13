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
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration; // Add this line
using GameEnd;
using ManageServer.Models;

public class Startup
{
    public IConfiguration Configuration { get; } // Configuration 프로퍼티 추가

    public Startup(IConfiguration configuration) // 생성자에서 IConfiguration 주입
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // CustomInfo configuration
        var customInfo = new CustomInfo();
        Configuration.GetSection("CustomInfo").Bind(customInfo);
        services.AddSingleton(customInfo);

        // Rate limit services
        services.AddMemoryCache();

        services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));
        services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
        services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddInMemoryRateLimiting();

        services.AddControllers();

        // Enable controllers for the web API
        services.AddControllers();
        // gRPC server service
        services.AddGrpc();

        // Register radis connection and game room manager
        services.AddSingleton<IConnectionMultiplexer>(provider =>
        {
            var configuration = ConfigurationOptions.Parse(
                customInfo.RedisSettings?.ConnectionString != null 
                ? customInfo.RedisSettings.ConnectionString
                : "localhost:6379");
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
            
            var channel = GrpcChannel.ForAddress(
                customInfo.GrpcSettings?.ServerIP != null
                ? customInfo.GrpcSettings.ServerIP
                : "localhost:5255",
                new GrpcChannelOptions { HttpHandler = httpHandler });
            return new GrpcGameServerClient(channel);
        });
        services.AddScoped<RoomMatchManager>();
        services.AddSingleton<RoomManagementService.RoomManagementServiceBase, RoomManagementServiceImpl>();
        services.AddScoped<GameResultService.GameResultServiceBase, GameResultServiceImpl>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseIpRateLimiting();
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();  // Map controller routes
            endpoints.MapGrpcService<RoomManagementServiceImpl>();
            endpoints.MapGrpcService<GameResultServiceImpl>();
        });
    }
}
