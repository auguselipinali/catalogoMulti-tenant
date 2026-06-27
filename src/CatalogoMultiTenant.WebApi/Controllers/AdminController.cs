using CatalogoMultiTenant.Application.Features.Catalog.Commands.CreateProduct;
using CatalogoMultiTenant.Application.Features.Catalog.Commands.DeleteProduct;
using CatalogoMultiTenant.Application.Features.Catalog.Commands.UpdateProduct;
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

    [HttpPost("products")]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command, CancellationToken ct)
    {
        var id = await _sender.Send(command, ct);
        return StatusCode(StatusCodes.Status201Created, new { id });
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
}
