﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using System.Threading.Tasks;

public class Program
{
    private static ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
    private static IDatabase db = redis.GetDatabase();
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        await host.RunAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();  // Use Startup class to configure API
            });
}
