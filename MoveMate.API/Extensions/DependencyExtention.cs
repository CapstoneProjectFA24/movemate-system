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
using MoveMate.API.Authorization;
using MoveMate.API.Middleware;
using MoveMate.Service.IServices;
using MoveMate.Service.Services;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.Commons;
using MoveMate.API.Constants;
using MoveMate.API.Utils;
using MoveMate.Service.BackgroundServices;
using MoveMate.Service.ThirdPartyService;
using ErrorUtil = MoveMate.Service.Utils.ErrorUtil;
using MoveMate.Service.ViewModels.ModelRequests;
using Microsoft.Extensions.Options;
using Net.payOS;
using MoveMate.Service.ThirdPartyService.Payment.VNPay;
using MoveMate.Service.ThirdPartyService.Firebase;
using MoveMate.Service.ThirdPartyService.Payment.Zalo;
using MoveMate.Service.ThirdPartyService.Payment.PayOs;
using MoveMate.Service.ThirdPartyService.Payment.Zalo;
using MoveMate.Service.ThirdPartyService.Payment.Momo;
using MoveMate.Service.ThirdPartyService.Payment.VNPay;


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
            services.AddHttpClient();
            //services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserServices, UserService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<ITruckServices, TruckServices>();
            services.AddScoped<IScheduleServices, ScheduleServices>();
            services.AddScoped<IBookingServices, BookingServices>();
            services.AddScoped<IGoogleMapsService,GoogleMapsService>();
            services.AddScoped<IHouseTypeServices, HouseTypeServices>();
            services.AddScoped<IHouseTypeSettingServices, HouseTypeSettingServices>();
            services.AddScoped<IServiceServices , ServiceServices>();
            services.AddScoped<IServiceDetails, ServiceDetails>();
            services.AddScoped<IFeeSettingServices, FeeSettingServices>();
            services.AddScoped<IWalletServices, WalletServices>();
            services.AddScoped<IPaymentServices, PaymentService>();
            services.AddScoped<IVnPayService, VnPayService>();
            services.AddScoped<IZaloPayService, ZaloPayServices>();
            services.AddScoped<IPayOsService,  PayOsService>();
            services.AddScoped<ZaloPaySDK>();
            // services.AddScoped<IFirebaseMiddleware, FirebaseMiddleware>();
            // services.AddScoped<IFirebaseServices, FirebaseServices>();

            return services;
        }


        
        public static IServiceCollection AddHangfire(this IServiceCollection services)
        {

            services.AddSingleton<IBackgroundServiceHangFire, BackgroundServiceHangFire>();

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


        //Firebase
        public static IServiceCollection AddFirebaseServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Retrieve the Firebase config file path from appsettings.json
            string firebaseConfigPath = configuration.GetSection("FirebaseSettings:ConfigFile").Value;

            // Register FirebaseServices as a singleton to ensure only one instance is created
            services.AddSingleton<IFirebaseServices>(sp => new FirebaseServices(firebaseConfigPath));

            services.AddTransient<IFirebaseMiddleware, FirebaseMiddleware>();

            return services;
        }

        //zalo pay
        public static IServiceCollection AddZaloPayConfig(this IServiceCollection services, IConfiguration configuration)
        {
            
            var zaloPaySettings = new ZaloPaySettings();
            configuration.GetSection("ZaloPay").Bind(zaloPaySettings);
            services.AddSingleton(zaloPaySettings); 
            services.AddScoped<ZaloPaySDK>();
            return services;
        }

        //momo
        public static IServiceCollection AddMomoConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MomoSettings>(configuration.GetSection("Momo"));
            services.AddScoped<IMomoPaymentService, MomoPaymentService>();
            return services;
        }

        //vnpay
        public static IServiceCollection AddVNPConfig(this IServiceCollection services, IConfiguration configuration)
        {
            // Bind VnPaySettings configuration from appsettings.json and register it as a singleton
            var vnPaySettings = new VnPaySettings();
            configuration.GetSection("VNPay").Bind(vnPaySettings);
            services.AddSingleton(vnPaySettings);

            // Register the VnPayService, which depends on VnPaySettings
            services.AddScoped<IVnPayService, VnPayService>();

            return services;
        }




        //Authen
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JWTAuth");
            services.Configure<JWTAuth>(jwtSettings);

            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

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

        //PayOS
        public static IServiceCollection AddPayOS(this IServiceCollection services, IConfiguration configuration)
        {
            // Read configuration values
            string clientId = configuration["PAYOS:CLIENT_ID"];
            string apiKey = configuration["PAYOS:API_KEY"];
            string checksumKey = configuration["PAYOS:CHECKSUM_KEY"];

            // Register PayOS as a singleton
            services.AddSingleton(new PayOS(clientId, apiKey, checksumKey));

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
            /*app.UseHangfireDashboard();
            
            app.MapHangfireDashboard("/hangfire", new DashboardOptions()
            {
                DashboardTitle = "MoveMateSysterm - Background Services",
                //Authorization = new[] { new MyAuthorizationFilter() }
            });*/

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                DashboardTitle = "MoveMateSysterm - Background Services",
                Authorization = new[] { new MyAuthorizationFilter() }
            });
            app.MapHangfireDashboard();
            
            BackgroundJob.Enqueue<IBackgroundServiceHangFire>(cf => cf.StartAllBackgroundJob());
            return app;
        }
    }
}