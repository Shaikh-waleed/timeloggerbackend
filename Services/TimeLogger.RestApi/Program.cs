using TimeLogger.Infrastructure.DependencyResolution;
using TimeLogger.RestApi.Models;
using Microsoft.IdentityModel.Logging;

var builder = WebApplication.CreateBuilder(args);
IdentityModelEventSource.ShowPII = true;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


builder.Services.RegisterServices(builder.Configuration);

builder.Services.AddHttpContextAccessor();

//Local dependencies
builder.Services.AddMvc();

//builder.Services.AddSwaggerGen();

var app = builder.Build();

app.RegisterApps();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}


app.UseHttpsRedirection();

app.MapControllers();

app.UseStaticFiles();

app.Run();
