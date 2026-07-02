using MediatR;

namespace CatalogoMultiTenant.Application.Features.Catalog.Commands.UpdateCategory;

public record UpdateCategoryCommand(Guid Id, string Name) : IRequest;
