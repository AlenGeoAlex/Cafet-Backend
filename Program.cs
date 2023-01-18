using System.Reflection;
using Cafet_Backend;
using Cafet_Backend.Extensions;
using Cafet_Backend.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.InitConfigurationOptions();
builder.Services.AddControllers();
builder.InitServices();
builder.ConfigureAuthentication();
builder.CreateCorsPolicy();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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

app.MapControllers();

app.Run();
