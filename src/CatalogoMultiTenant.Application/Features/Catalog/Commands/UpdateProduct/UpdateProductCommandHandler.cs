using CatalogoMultiTenant.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CatalogoMultiTenant.Application.Features.Catalog.Commands.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand>
{
    private readonly IAppDbContext _db;

    public UpdateProductCommandHandler(IAppDbContext db) => _db = db;

    public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        // Global query filter restricts this to the current tenant's products.
        // A product belonging to another tenant is invisible here → null → 404.
        var product = await _db.Products
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (product is null)
            throw new KeyNotFoundException($"Product {request.Id} not found.");

        if (request.CategoryId is Guid categoryId)
        {
            // _db.Categories is already scoped to the current tenant by the global
            // query filter. A category from another tenant is invisible → rejected.
            var belongsToTenant = await _db.Categories
                .AnyAsync(c => c.Id == categoryId, cancellationToken);

            if (!belongsToTenant)
                throw new KeyNotFoundException($"Category {categoryId} not found.");
        }

        product.Update(request.Name, request.Price, request.Description, request.ImageUrl, request.CategoryId);

        await _db.SaveChangesAsync(cancellationToken);
    }
}
