using MediatR;

namespace CatalogoMultiTenant.Application.Features.Catalog.Commands.DeleteProduct;

public record DeleteProductCommand(Guid Id) : IRequest;
