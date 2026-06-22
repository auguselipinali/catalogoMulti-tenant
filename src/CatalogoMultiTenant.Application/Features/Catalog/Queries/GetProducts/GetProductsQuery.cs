using MediatR;

namespace CatalogoMultiTenant.Application.Features.Catalog.Queries.GetProducts;

public record ProductDto(Guid Id, string Name, decimal Price, string? Description, string? ImageUrl);

public record GetProductsQuery : IRequest<IReadOnlyList<ProductDto>>;
