using MediatR;

namespace CatalogoMultiTenant.Application.Features.Auth.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<LoginResponse>;

public record LoginResponse(string Token, DateTime ExpiresAtUtc);
