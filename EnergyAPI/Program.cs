using EnergyAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<ICarbonIntensity, CarbonIntensity>();
builder.Services.AddScoped<IEnergyMixService, EnergyMixService>();
builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
