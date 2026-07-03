using MediatR;

namespace CatalogoMultiTenant.Application.Features.Catalog.Queries.GetCategories;

public record CategoryDto(Guid Id, string Name);

public record GetCategoriesQuery : IRequest<IReadOnlyList<CategoryDto>>;
