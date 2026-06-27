using CatalogoMultiTenant.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CatalogoMultiTenant.Application.Features.Catalog.Commands.DeleteProduct;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
{
    private readonly IAppDbContext _db;

    public DeleteProductCommandHandler(IAppDbContext db) => _db = db;

    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        // Global query filter restricts this to the current tenant's products.
        // A product belonging to another tenant is invisible here → null → 404.
        var product = await _db.Products
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (product is null)
            throw new KeyNotFoundException($"Product {request.Id} not found.");

        _db.Products.Remove(product);

        await _db.SaveChangesAsync(cancellationToken);
    }
}
