using CatalogoMultiTenant.Application.Common.Interfaces;
using CatalogoMultiTenant.Domain.Entities;
using MediatR;

namespace CatalogoMultiTenant.Application.Features.Catalog.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IAppDbContext _db;

    public CreateProductCommandHandler(IAppDbContext db) => _db = db;

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = Product.Create(request.Name, request.Price, request.Description, request.ImageUrl);

        _db.Products.Add(product);

        // SaveChangesAsync auto-assigns TenantId from ITenantContext (JWT claim).
        await _db.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}
