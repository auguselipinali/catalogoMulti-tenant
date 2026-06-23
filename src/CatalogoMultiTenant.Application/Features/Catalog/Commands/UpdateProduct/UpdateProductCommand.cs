using MediatR;

namespace CatalogoMultiTenant.Application.Features.Catalog.Commands.UpdateProduct;

public record UpdateProductCommand(Guid Id, string Name, decimal Price, string? Description, string? ImageUrl)
    : IRequest;
