using MediatR;

namespace CatalogoMultiTenant.Application.Features.Catalog.Commands.DeleteCategory;

public record DeleteCategoryCommand(Guid Id) : IRequest;
