using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;
using Hangfire;
using Hangfire.SqlServer;
using Hangfire.Storage.SQLite;
using HangfireBasicAuthenticationFilter;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using MoveMate.API.Middleware;
using MoveMate.Service.IServices;
using MoveMate.Service.Services;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.Commons;
using MoveMate.API.Constants;
using MoveMate.API.Utils;
using MoveMate.Service.BackgroundServices;
using ErrorUtil = MoveMate.Service.Utils.ErrorUtil;


namespace MoveMate.API.Extensions
{
    public static class DependencyExtention
    {
        public static IServiceCollection AddUnitOfWork(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }

        public static IServiceCollection AddDbFactory(this IServiceCollection services)
        {
            services.AddScoped<IDbFactory, DbFactory>();
            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            //services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserServices, UserService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddSingleton<IBackgroundServiceHangFire, BackgroundServiceHangFire>();
            //services.AddScoped<IAuctionService, AuctionService>();
            //services.AddScoped<IOrderService, OrderService>();
            ////services.AddScoped(typeof(IFirebaseService<>), typeof(FirebaseService<>));
            //services.AddScoped<IFirebaseService<Auction>, FirebaseService<Auction>>();
            //services.AddScoped<IVnPayService, VnPayService>();
            //services.AddScoped<IWalletService, WalletService>();
            //services.AddScoped<IBidService, BidService>();
            //services.AddScoped<IBidRepository, BidRepository>();
            //services.AddScoped<ITransactionService, TransactionService>();
            
            return services;
        }
        
        public static IServiceCollection AddHangfire(this IServiceCollection services)
        {

            string connectionString = DbUtil.getConnectString();
            
            services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(connectionString,
                    new SqlServerStorageOptions()
                    {
                        QueuePollInterval = TimeSpan.FromSeconds(1),
                        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                        UseRecommendedIsolationLevel = true,
                        DisableGlobalLocks = true,
                        JobExpirationCheckInterval = TimeSpan.FromDays(1)
                    }));
            services.AddHangfireServer();

            return services;
        }

        public static void AddJwtValidation(this WebApplicationBuilder builder)
        {
            var key = Encoding.UTF8.GetBytes(builder.Configuration["JWTAuth:Key"]);
            builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnChallenge = async context =>
                        {
                            context.HandleResponse();
                            context.Response.StatusCode = 401;
                            await context.Response.WriteAsJsonAsync(new
                            {
                                Message = JsonConvert.DeserializeObject<List<ErrorDetail>>(
                                    ErrorUtil.GetErrorString("Unauthorized", "You are not allowed to access this API."))
                            });
                        }
                    };
                });
        }

        public static IServiceCollection AddConfigSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                // using System.Reflection;
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

                options.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });

                    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });
            return services;
        }

        public static IServiceCollection AddExceptionMiddleware(this IServiceCollection services)
        {
            services.AddTransient<ExceptionMiddleware>();
            services.AddTransient<RequestDelegate>(serviceProvider =>
            {
                // Get the next middleware in the pipeline
                var next = serviceProvider.GetRequiredService<RequestDelegate>();
                return async context =>
                {
                    // Call the next middleware in the pipeline
                    await next(context);
                };
            });
            return services;
        }

        public static WebApplication AddApplicationConfig(this WebApplication app)
        {
            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseCors(CorsConstants.PolicyName);
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<ExceptionMiddleware>();
            //Add middleware extentions
            app.ConfigureExceptionMiddleware();
            app.MapControllers();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllers();
            //});
            // hangfire
            app.UseHangfireDashboard();
            app.MapHangfireDashboard("/hangfire", new DashboardOptions()
            {
                DashboardTitle = "MoveMateSysterm - Background Services",

                Authorization = new[]
                {
                    new HangfireCustomBasicAuthenticationFilter()
                    {
                        User = "root",
                        Pass = "root"
                    }
                }
            });
            BackgroundJob.Enqueue<IBackgroundServiceHangFire>(cf => cf.StartAllBackgroundJob());
            return app;
        }
    }
}