using MediatR;

namespace CatalogoMultiTenant.Application.Features.Catalog.Queries.GetProducts;

public record ProductDto(Guid Id, string Name, decimal Price, string? Description, string? ImageUrl, Guid? CategoryId, string? CategoryName);

public record GetProductsQuery : IRequest<IReadOnlyList<ProductDto>>;
