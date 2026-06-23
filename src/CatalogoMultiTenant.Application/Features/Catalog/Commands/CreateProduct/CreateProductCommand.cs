using MediatR;

namespace CatalogoMultiTenant.Application.Features.Catalog.Commands.CreateProduct;

public record CreateProductCommand(string Name, decimal Price, string? Description, string? ImageUrl)
    : IRequest<Guid>;
