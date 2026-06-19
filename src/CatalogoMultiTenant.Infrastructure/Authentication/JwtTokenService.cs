using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CatalogoMultiTenant.Application.Common.Interfaces;
using CatalogoMultiTenant.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CatalogoMultiTenant.Infrastructure.Authentication;

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public (string Token, DateTime ExpiresAtUtc) GenerateToken(User user, string tenantSlug)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(
            double.Parse(jwtSettings["ExpirationMinutes"]!));

        // Los nombres de claim deben coincidir exactamente con los que
        // TenantContextService lee: "tenant_id" y "tenant_slug".
        var claims = new[]
        {
            new Claim("tenant_id", user.TenantId.ToString()),
            new Claim("tenant_slug", tenantSlug),
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAtUtc);
    }
}
