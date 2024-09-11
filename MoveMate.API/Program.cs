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
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddConfigSwagger();

            // JWT Authentication
            builder.Services.AddJwtAuthentication(builder.Configuration);

            // Dependency Injection
            DotNetEnv.Env.Load();
            builder.Services.AddDbFactory();
            builder.Services.AddUnitOfWork();
            builder.Services.AddHangfire();
            builder.Services.AddServices();
            builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            builder.Services.AddAutoMapper(typeof(AutoMapperService));
            builder.Services.AddExceptionMiddleware();
            builder.Services.AddFirebaseServices(builder.Configuration);

            // CORS Policy
            builder.Services.AddCors(cors => cors.AddPolicy(
                name: CorsConstants.PolicyName,
                policy =>
                {
                    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                }));
            
            // Fluent Validation
            builder.Services.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<AccountRequestValidator>());
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
            app.Run();
        }
    }
}
