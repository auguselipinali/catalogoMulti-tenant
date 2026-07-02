using CatalogoMultiTenant.Application.Features.Catalog.Commands.CreateCategory;
using CatalogoMultiTenant.Application.Features.Catalog.Commands.CreateProduct;
using CatalogoMultiTenant.Application.Features.Catalog.Commands.DeleteCategory;
using CatalogoMultiTenant.Application.Features.Catalog.Commands.DeleteProduct;
using CatalogoMultiTenant.Application.Features.Catalog.Commands.UpdateCategory;
using CatalogoMultiTenant.Application.Features.Catalog.Commands.UpdateProduct;
using CatalogoMultiTenant.Application.Features.Catalog.Queries.GetCategories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CatalogoMultiTenant.WebApi.Controllers;

[ApiController]
[Route("admin")]
[Authorize]
public class AdminController : ControllerBase
{
    private readonly ISender _sender;

    public AdminController(ISender sender) => _sender = sender;

    // ----- Products -----

    [HttpPost("products")]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command, CancellationToken ct)
    {
        try
        {
            var id = await _sender.Send(command, ct);
            return StatusCode(StatusCodes.Status201Created, new { id });
        }
        catch (KeyNotFoundException)
        {
            // Invalid CategoryId (does not exist for this tenant).
            return NotFound();
        }
    }

    [HttpPut("products/{id:guid}")]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductCommand command, CancellationToken ct)
    {
        try
        {
            await _sender.Send(command with { Id = id }, ct);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("products/{id:guid}")]
    public async Task<IActionResult> DeleteProduct(Guid id, CancellationToken ct)
    {
        try
        {
            await _sender.Send(new DeleteProductCommand(id), ct);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // ----- Categories -----

    [HttpPost("categories")]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryCommand command, CancellationToken ct)
    {
        var id = await _sender.Send(command, ct);
        return StatusCode(StatusCodes.Status201Created, new { id });
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories(CancellationToken ct)
    {
        var result = await _sender.Send(new GetCategoriesQuery(), ct);
        return Ok(result);
    }

    [HttpPut("categories/{id:guid}")]
    public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] UpdateCategoryCommand command, CancellationToken ct)
    {
        try
        {
            await _sender.Send(command with { Id = id }, ct);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("categories/{id:guid}")]
    public async Task<IActionResult> DeleteCategory(Guid id, CancellationToken ct)
    {
        try
        {
            await _sender.Send(new DeleteCategoryCommand(id), ct);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
