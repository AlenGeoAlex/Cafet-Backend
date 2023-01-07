using Cafet_Backend.Configuration;
using Cafet_Backend.Context;
using Cafet_Backend.Interfaces;
using Cafet_Backend.Manager;
using Cafet_Backend.Repository;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;

namespace Cafet_Backend.Extensions;

public static class BootstrapExtension
{
    public static WebApplicationBuilder InitServices(this WebApplicationBuilder applicationBuilder)
    {
        applicationBuilder.Services.AddDbContext<CafeContext>(builderOptions =>
        {
            builderOptions.UseSqlServer(applicationBuilder.Configuration.GetConnectionString("DefaultConnection"));
        });


        applicationBuilder.Services.AddScoped<IRoleRepository, RoleRepository>();
        applicationBuilder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
        applicationBuilder.Services.AddScoped<IFoodRepository, FoodRepository>();
        applicationBuilder.Services.AddSingleton(typeof(ImageProviderManager));
        applicationBuilder.Services.Configure<FormOptions>(o =>
        {
            o.ValueLengthLimit = int.MaxValue;
            o.MultipartBodyLengthLimit = int.MaxValue;
            o.MemoryBufferThreshold = int.MaxValue;
        });
        return applicationBuilder;
    }

    /*public static WebApplicationBuilder InitJsonBuilder(this WebApplicationBuilder applicationBuilder)
    {
        applicationBuilder.Services.Add
    }*/

    public static WebApplicationBuilder InitConfigurationOptions(this WebApplicationBuilder applicationBuilder)
    {
        applicationBuilder.Services.Configure<CorsOrigins>(options =>
            applicationBuilder.Configuration.GetSection(options.ConfigBinder()).Bind(options));
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
            });
        });
        
        Console.WriteLine($"Added {possibleNewEndPoints.Count} endpoints as Cors Origins");
        return applicationBuilder;
    }
}