using CatalogoMultiTenant.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CatalogoMultiTenant.Application.Features.Catalog.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand>
{
    private readonly IAppDbContext _db;

    public UpdateCategoryCommandHandler(IAppDbContext db) => _db = db;

    public async Task Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        // Global query filter restricts this to the current tenant's categories.
        // A category belonging to another tenant is invisible here → null → 404.
        var category = await _db.Categories
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (category is null)
            throw new KeyNotFoundException($"Category {request.Id} not found.");

        category.Rename(request.Name);

        await _db.SaveChangesAsync(cancellationToken);
    }
}
