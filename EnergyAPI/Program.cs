using EnergyAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<ICarbonIntensity, CarbonIntensity>();
builder.Services.AddScoped<IEnergyMixService, EnergyMixService>();
builder.Services.AddScoped<IChargingWindowService, ChargingWindowService>();
builder.Services.AddControllers();

var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.MapControllers();

app.Run();
