using CatalogoMultiTenant.Application.Features.Catalog.Queries.GetProducts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CatalogoMultiTenant.WebApi.Controllers;

[ApiController]
[Route("{slug}")]
public class CatalogController : ControllerBase
{
    private readonly ISender _sender;

    public CatalogController(ISender sender) => _sender = sender;

    [HttpGet("products")]
    public async Task<IActionResult> GetProducts(CancellationToken ct)
    {
        var result = await _sender.Send(new GetProductsQuery(), ct);
        return Ok(result);
    }
}
