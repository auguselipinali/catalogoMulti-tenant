using CatalogoMultiTenant.Application.Common.Interfaces;
using CatalogoMultiTenant.Domain.Entities;
using MediatR;

namespace CatalogoMultiTenant.Application.Features.Catalog.Commands.CreateCategory;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Guid>
{
    private readonly IAppDbContext _db;

    public CreateCategoryCommandHandler(IAppDbContext db) => _db = db;

    public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = Category.Create(request.Name);

        _db.Categories.Add(category);

        // SaveChangesAsync auto-assigns TenantId from ITenantContext (JWT claim).
        await _db.SaveChangesAsync(cancellationToken);

        return category.Id;
    }
}
