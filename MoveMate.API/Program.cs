using FluentValidation.AspNetCore;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MoveMate.API.Extensions;
using MoveMate.API.Middleware;
using Quartz;
using System.Reflection;
using System.Text;
using Hangfire;
using Hangfire.Storage.SQLite;
using HangfireBasicAuthenticationFilter;
using MoveMate.Service.Commons;
using MoveMate.API.Constants;
using MoveMate.Domain.Models;
using MoveMate.Service.ViewModels.ModelRequests;
using MoveMate.Service.IServices;
using MoveMate.Service.Services;
using System.Configuration;
using MoveMate.Service.Commons.AutoMapper;
using MoveMate.Service.Commons.Validates;
using DotNetEnv;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

namespace MoveMate.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers().ConfigureApiBehaviorOptions(opts
                => opts.SuppressModelStateInvalidFilter = true);
            builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
            
            /*// add prometheus exporter
            builder.Services.AddOpenTelemetry()
                .WithMetrics(opt =>
    
                    opt
                        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("MoveMate.API"))
                        .AddMeter(builder.Configuration.GetValue<string>("OpenRemoteManageMeterName"))
                        .AddAspNetCoreInstrumentation()
                        .AddRuntimeInstrumentation()
                        .AddProcessInstrumentation()
                        .AddOtlpExporter(opts =>
                        {
                            opts.Endpoint = new Uri(builder.Configuration["Otel:Endpoint"]);
                            Console.WriteLine("OTLP Endpoint: " + builder.Configuration["Otel:Endpoint"]);
                        })
                );   */
            // Add OpenTelemetry with Prometheus exporter
            builder.Services.AddOpenTelemetry()
                .WithMetrics(opt =>
                {
                    // Log thông tin để gỡ lỗi
                    string meterName = builder.Configuration.GetValue<string>("MoveMateMeterName");
                    string otelEndpoint = builder.Configuration["Otel:Endpoint"];

                    Console.WriteLine($"Initializing OpenTelemetry with Meter: {meterName}");
                    Console.WriteLine($"OTLP Endpoint: {otelEndpoint}");

                    // Cấu hình metrics
                    opt
                        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("MoveMate.API"))
                        .AddMeter(meterName) // Tên Meter từ cấu hình
                        .AddAspNetCoreInstrumentation() // Theo dõi request/response từ ASP.NET
                        .AddRuntimeInstrumentation() // Theo dõi thông tin runtime
                        .AddProcessInstrumentation() // Theo dõi quy trình thực thi
                        .AddOtlpExporter(opts =>
                        {
                            opts.Endpoint = new Uri(otelEndpoint);
                            Console.WriteLine("OpenTelemetry Exporter configured successfully.");
                        });
                });
            
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddConfigSwagger();

            // JWT Authentication
            builder.Services.AddJwtAuthentication(builder.Configuration);

            // Dependency Injection
            DotNetEnv.Env.Load();
            
            
            
            builder.Services.AddDbFactory();
            builder.Services.AddUnitOfWork();
            builder.Services.AddHangfire();
            builder.Services.AddRedis();
            builder.Services.AddServices();
            builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            builder.Services.AddAutoMapper(typeof(AutoMapperService));
            builder.Services.AddExceptionMiddleware();
            builder.Services.AddFirebaseServices(builder.Configuration);
            builder.Services.AddCloudinaryService(builder.Configuration);
            builder.Services.AddPayOS(builder.Configuration);
            builder.Services.AddZaloPayConfig(builder.Configuration);
            builder.Services.AddMomoConfig(builder.Configuration);
            builder.Services.AddVNPConfig(builder.Configuration);
            builder.Services.AddRabbitMQ();

            // CORS Policy
            string[] allowedOrigins = builder.Environment.IsDevelopment()
                 ? new[] { "http://localhost:3000", "https://movematee.vercel.app" }
                 : new[] { "https://movematee.vercel.app" };

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin", builder =>
                {
                    builder.WithOrigins(allowedOrigins)
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            // Fluent Validation
            builder.Services.AddFluentValidation(fv =>
                fv.RegisterValidatorsFromAssemblyContaining<AccountRequestValidator>());
            builder.Services.AddValidatorsFromAssemblyContaining<AccountTokenValidator>();


            // Add Quartz (Optional, uncomment if needed)
            //builder.Services.AddQuartz(q =>
            //{
            //    q.UseMicrosoftDependencyInjectionJobFactory();
            //    q.AddJobAndTrigger<HelloJob>(builder.Configuration);
            //    q.AddJobAndTrigger<EveryDay1AmJob>(builder.Configuration);
            //    q.AddJobAndTrigger<MonthlyAt1AMOn1st>(builder.Configuration);
            //    q.AddJobAndTrigger<EveryMinute>(builder.Configuration);
            //});
            //builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.AddApplicationConfig();
            app.UseCors("AllowSpecificOrigin");
            app.Run();
        }
    }
}