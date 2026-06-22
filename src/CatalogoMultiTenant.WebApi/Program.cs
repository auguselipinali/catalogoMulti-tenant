using CatalogoMultiTenant.Application;
using CatalogoMultiTenant.Infrastructure;
using CatalogoMultiTenant.Infrastructure.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();

var app = builder.Build();

app.UseRouting();                              // populates route values before middleware reads them
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<TenantSlugMiddleware>();     // runs after auth (JWT already parsed); reads {slug} from route values

app.MapControllers();

await app.Services.SeedDevelopmentDataAsync();

app.Run();
