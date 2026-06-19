using CatalogoMultiTenant.Application;
using CatalogoMultiTenant.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.Services.SeedDevelopmentDataAsync();

app.Run();
