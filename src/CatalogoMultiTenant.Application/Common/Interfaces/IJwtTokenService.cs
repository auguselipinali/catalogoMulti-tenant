using CatalogoMultiTenant.Domain.Entities;

namespace CatalogoMultiTenant.Application.Common.Interfaces;

public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAtUtc) GenerateToken(User user, string tenantSlug);
}
