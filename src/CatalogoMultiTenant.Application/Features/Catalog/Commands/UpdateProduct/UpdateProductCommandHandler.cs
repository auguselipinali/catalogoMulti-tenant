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

        product.Update(request.Name, request.Price, request.Description, request.ImageUrl);

        await _db.SaveChangesAsync(cancellationToken);
    }
}
