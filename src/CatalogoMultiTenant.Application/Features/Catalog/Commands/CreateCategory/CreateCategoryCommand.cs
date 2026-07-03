using MediatR;

namespace CatalogoMultiTenant.Application.Features.Catalog.Commands.CreateCategory;

public record CreateCategoryCommand(string Name) : IRequest<Guid>;
