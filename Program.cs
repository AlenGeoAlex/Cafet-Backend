using System.Reflection;
using Cafet_Backend;
using Cafet_Backend.Extensions;
using Cafet_Backend.Hub;
using Cafet_Backend.Middleware;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.InitConfigurationOptions();
builder.Services.AddControllers();
builder.InitServices();
builder.ConfigureAuthentication();
builder.CreateCorsPolicy();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});
builder.Host.ConfigureLogging(configureLogging =>
{
    configureLogging.ClearProviders();
    configureLogging.AddConsole();
});

var app = builder.Build();


// Configure the HTTP request pipeline.
//Things should be in order from here on

    

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(Constants.CorsPolicyName);

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<JwtMiddleware>();

app.UseStatusCodePages();
app.UseStaticFiles();

app.MapHub<OrderHub>("api/live/order");
app.MapHub<StatisticsHub>("api/live/stats");

app.MapControllers();

app.Run();
