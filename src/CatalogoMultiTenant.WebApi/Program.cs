using CatalogoMultiTenant.Application;
using CatalogoMultiTenant.Infrastructure;
using CatalogoMultiTenant.Infrastructure.Middleware;

var builder = WebApplication.CreateBuilder(args);

const string FrontendCorsPolicy = "FrontendCorsPolicy";

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? Array.Empty<string>();
builder.Services.AddCors(options =>
    options.AddPolicy(FrontendCorsPolicy, policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()));

var app = builder.Build();

app.UseRouting();                              // populates route values before middleware reads them
app.UseCors(FrontendCorsPolicy);              // must run before authentication/authorization
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<TenantSlugMiddleware>();     // runs after auth (JWT already parsed); reads {slug} from route values

app.MapControllers();

await app.Services.SeedDevelopmentDataAsync();

app.Run();
