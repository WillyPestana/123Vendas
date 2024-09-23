using _123Vendas.API.Configurations;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog configuration
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information() // Global log level
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.Logger(lc => lc
        .MinimumLevel.Information()
        .Filter.ByIncludingOnly(logEvent => logEvent.Properties.ContainsKey("MessageBus"))
        .WriteTo.Console(outputTemplate: "[MessageBus] [{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"))
    .CreateLogger();

builder.Host.UseSerilog();

// Call the ConfigureServices method from the DependencyInjectionConfig class
DependencyInjectionConfig.ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

#region Configure Pipeline

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("corsapp");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
#endregion