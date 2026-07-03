using CatalogoMultiTenant.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CatalogoMultiTenant.Application.Features.Catalog.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand>
{
    private readonly IAppDbContext _db;

    public DeleteCategoryCommandHandler(IAppDbContext db) => _db = db;

    public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        // Global query filter restricts this to the current tenant's categories.
        // A category belonging to another tenant is invisible here → null → 404.
        var category = await _db.Categories
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (category is null)
            throw new KeyNotFoundException($"Category {request.Id} not found.");

        // Products referencing this category get CategoryId = null via the FK's
        // ON DELETE SET NULL configured on Product.CategoryId.
        _db.Categories.Remove(category);

        await _db.SaveChangesAsync(cancellationToken);
    }
}
