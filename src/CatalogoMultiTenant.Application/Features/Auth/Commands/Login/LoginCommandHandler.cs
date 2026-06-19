using CatalogoMultiTenant.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CatalogoMultiTenant.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IAppDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public LoginCommandHandler(
        IAppDbContext context,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Credenciales inválidas.");

        var tenant = await _context.Tenants
            .FirstOrDefaultAsync(t => t.Id == user.TenantId, cancellationToken);

        if (tenant is null)
            throw new InvalidOperationException("Tenant no encontrado.");

        var (token, expiresAtUtc) = _jwtTokenService.GenerateToken(user, tenant.Slug);

        return new LoginResponse(token, expiresAtUtc);
    }
}
