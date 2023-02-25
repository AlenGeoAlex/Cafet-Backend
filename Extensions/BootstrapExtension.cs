using System.Collections.Immutable;
using System.Text;
using Cafet_Backend.Abstracts;
using Cafet_Backend.Configuration;
using Cafet_Backend.Context;
using Cafet_Backend.Dto.Errors;
using Cafet_Backend.Helper;
using Cafet_Backend.Interfaces;
using Cafet_Backend.Manager;
using Cafet_Backend.Provider;
using Cafet_Backend.Provider.RTProvider;
using Cafet_Backend.Repository;
using Cafet_Backend.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Cafet_Backend.Extensions;

public static class BootstrapExtension
{
    
    
    public static WebApplicationBuilder InitServices(this WebApplicationBuilder applicationBuilder)
    {
        JwtConfig options = new JwtConfig();
        applicationBuilder.Configuration.GetSection(options.ConfigBinder()).Bind(options);
        
        StripeConfiguration stripeConfiguration = new StripeConfiguration();
        applicationBuilder.Configuration.GetSection(stripeConfiguration.ConfigBinder()).Bind(stripeConfiguration);

        Stripe.StripeConfiguration.ApiKey = stripeConfiguration.SecretToken;
        
        applicationBuilder.Services.AddDbContext<CafeContext>(builderOptions =>
        {
            builderOptions.UseSqlServer(applicationBuilder.Configuration.GetConnectionString("DefaultConnection"));
        });

        applicationBuilder.Services.AddSignalR();
        applicationBuilder.Services.AddScoped<IRoleRepository, RoleRepository>();
        applicationBuilder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
        applicationBuilder.Services.AddScoped<IFoodRepository, FoodRepository>();
        applicationBuilder.Services.AddScoped<IUserRepository, UserRepository>();
        applicationBuilder.Services.AddScoped<ICartRepository, CartRepository>();
        applicationBuilder.Services.AddScoped<IStockRepository, StockRepository>();
        applicationBuilder.Services.AddScoped<IOrderRepository, OrderRepository>();
        applicationBuilder.Services.AddScoped<IWalletRepository, WalletRepository>();
        applicationBuilder.Services.AddScoped<IStatisticsRepository, StatisticsRepository>();
        applicationBuilder.Services.AddSingleton(typeof(ImageProviderManager));
        applicationBuilder.Services.AddSingleton<IStripeSessionManager, StripeSessionManager>();
        applicationBuilder.Services.AddAutoMapper(typeof(MapProvider));
        applicationBuilder.Services.AddSingleton(typeof(TokenService));
        applicationBuilder.Services.AddSingleton(typeof(MailModelManager));
        applicationBuilder.Services.AddScoped<IMailService, MailService>();

        applicationBuilder.Services.AddHostedService<StatisticsHostService>();
        applicationBuilder.Services.AddScoped<IScopedProcessingService, ScopedStatisticsService>();        
        if (options.RefreshTokenSettings.CacheMethod == "MEMORY")
        {
            applicationBuilder.Services.AddScoped<AbstractRefreshTokenManager, InMemoryProvider>();
        }
        else
        {
            Console.WriteLine("Failed to register RefreshToken Provider");
        }
        
        applicationBuilder.Services.Configure<FormOptions>(o =>
        {
            o.ValueLengthLimit = int.MaxValue;
            o.MultipartBodyLengthLimit = int.MaxValue;
            o.MemoryBufferThreshold = int.MaxValue;
        });
        applicationBuilder.Services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                ModelStateDictionary modelStateDictionary = context.ModelState;

                var errors = modelStateDictionary
                    .Where(e => e.Value.Errors.Count > 0)
                    .SelectMany(x => x.Value.Errors)
                    .Select(x => x.ErrorMessage)
                    .ToImmutableList();
                var errorResponse = new ApiValidationErrorResponse()
                {
                    Errors = errors,
                };
                return new BadRequestObjectResult(errorResponse);
            };
        });
        
        return applicationBuilder;
    }

    public static WebApplicationBuilder InitConfigurationOptions(this WebApplicationBuilder applicationBuilder)
    {
        applicationBuilder.Services.Configure<CorsOrigins>(options =>
            applicationBuilder.Configuration.GetSection(options.ConfigBinder()).Bind(options));

        applicationBuilder.Services.Configure<JwtConfig>(options =>
        {
            applicationBuilder.Configuration.GetSection(options.ConfigBinder()).Bind(options);
        });

        applicationBuilder.Services.Configure<MailConfiguration>(options =>
        {
            applicationBuilder.Configuration.GetSection(options.ConfigBinder()).Bind(options);
        });


        applicationBuilder.Services.Configure<StripeConfiguration>(opt =>
        {
            applicationBuilder.Configuration.GetSection(opt.ConfigBinder()).Bind(opt);
        });
        
        return applicationBuilder;
    }
    


    public static WebApplicationBuilder CreateCorsPolicy(this WebApplicationBuilder applicationBuilder)
    {
        CorsOrigins options = new CorsOrigins();
        applicationBuilder.Configuration.GetSection(options.ConfigBinder()).Bind(options);
        if (options.OpenEndPoints.Count == 0 && options.SecuredEndPoints.Count == 0)
        {
            Console.WriteLine("No CorsPolicy Origins are mentioned in the config. Skipping it!");
            return applicationBuilder;
        }

        List<string> possibleNewEndPoints = new List<string>();
        possibleNewEndPoints.AddRange(options.OpenEndPoints);
        possibleNewEndPoints.AddRange(options.SecuredEndPoints);

        applicationBuilder.Services.AddCors(corsBuilder =>
        {
            corsBuilder.AddPolicy(Constants.CorsPolicyName, builder =>
            {
                builder.AllowAnyHeader();
                builder.AllowAnyMethod();
                builder.WithOrigins(possibleNewEndPoints.ToArray());
                builder.AllowCredentials();
            });
        });
        
        Console.WriteLine($"Added {possibleNewEndPoints.Count} endpoints as Cors Origins");
        return applicationBuilder;
    }

    public static WebApplicationBuilder ConfigureAuthentication(this WebApplicationBuilder applicationBuilder)
    {
        JwtConfig options = new JwtConfig();
        applicationBuilder.Configuration.GetSection(options.ConfigBinder()).Bind(options);
        applicationBuilder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(bearerOptions =>
            {
                
                bearerOptions.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Token))
                };
                
            });
        return applicationBuilder;
    }
}