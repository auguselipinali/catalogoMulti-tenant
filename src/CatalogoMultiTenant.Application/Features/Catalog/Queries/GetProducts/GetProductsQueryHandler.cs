using CatalogoMultiTenant.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CatalogoMultiTenant.Application.Features.Catalog.Queries.GetProducts;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, IReadOnlyList<ProductDto>>
{
    private readonly IAppDbContext _db;

    public GetProductsQueryHandler(IAppDbContext db) => _db = db;

    public async Task<IReadOnlyList<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        // Left join to Categories (both DbSets tenant-filtered by the global query
        // filter) so products without a category are kept, with null CategoryName.
        var query =
            from p in _db.Products.AsNoTracking()
            join c in _db.Categories on p.CategoryId equals c.Id into gj
            from c in gj.DefaultIfEmpty()
            select new ProductDto(
                p.Id, p.Name, p.Price, p.Description, p.ImageUrl,
                p.CategoryId, c != null ? c.Name : null);

        return await query.ToListAsync(cancellationToken);
    }
}
