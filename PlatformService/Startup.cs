using System.IO;
using System;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using PlatformService.Data;
using PlatformService.SyncDataServices.Grpc;
using PlatformService.SyncDataServices.Http;

namespace PlatformService
{
  public class Startup
  {
    public IConfiguration Configuration { get; }
    private readonly IWebHostEnvironment _env;

    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
      Configuration = configuration;
      _env = env;
    }

    public void ConfigureServices(IServiceCollection services)
    {
      if (_env.IsProduction())
      {
        Console.WriteLine("--> Using SqlServer Db!");
        services.AddDbContext<AppDbContext>(
          options => options.UseSqlServer(Configuration.GetConnectionString("PlatformsConnection"))
        );
      }
      else
      {
        Console.WriteLine("--> Using InMem Db!");
        services.AddDbContext<AppDbContext>(
          options => options.UseInMemoryDatabase("InMem")
        );
      }

      services.AddScoped<IPlatformRepository, PlatformRepository>();

      services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

      services.AddMassTransit(x =>
      {
        x.UsingRabbitMq((context, cfg) =>
        {
          cfg.Host($"amqp://guest:guest@{Configuration["RabbitMQHost"]}:{Configuration["RabbitMQPort"]}");
        });
      });
      
      services.AddMassTransitHostedService();

      services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();

      services.AddGrpc();
      services.AddControllers();

      services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Platform Service", Version = "v1" });
      });

      Console.WriteLine($"--> CommandService Endpoint: {Configuration["CommandService"]}");
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
          c.SwaggerEndpoint("/swagger/v1/swagger.json", "PlatformService v1");
          c.RoutePrefix = string.Empty;
        });
      }

      // app.UseHttpsRedirection();

      app.UseRouting();

      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
        endpoints.MapGrpcService<GrpcPlatformService>();

        endpoints.MapGet("/protos/platforms.proto", async context => {
          await context.Response.WriteAsync(File.ReadAllText("Protos/platforms.proto"));
        });
      });

      app.PopulateDatabase(env.IsProduction());
    }
  }
}
