using CatalogoMultiTenant.Application.Common.Interfaces;
using CatalogoMultiTenant.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CatalogoMultiTenant.Application.Features.Catalog.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IAppDbContext _db;

    public CreateProductCommandHandler(IAppDbContext db) => _db = db;

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        if (request.CategoryId is Guid categoryId)
        {
            // _db.Categories is already scoped to the current tenant by the global
            // query filter. A category from another tenant is invisible → rejected.
            var belongsToTenant = await _db.Categories
                .AnyAsync(c => c.Id == categoryId, cancellationToken);

            if (!belongsToTenant)
                throw new KeyNotFoundException($"Category {categoryId} not found.");
        }

        var product = Product.Create(request.Name, request.Price, request.Description, request.ImageUrl, request.CategoryId);

        _db.Products.Add(product);

        // SaveChangesAsync auto-assigns TenantId from ITenantContext (JWT claim).
        await _db.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}
