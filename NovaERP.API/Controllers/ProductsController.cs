using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NovaERP.API.Authorization;
using NovaERP.Application.Common.Models;
using NovaERP.Application.Features.Products.DTOs;
using NovaERP.Application.Interfaces.Services;

namespace NovaERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    [HasPermission("Permissions.Products.View")]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] string? sortBy = null, [FromQuery] string? sortOrder = null)
    {
        var products = await _productService.GetAllAsync(pageNumber, pageSize, search, sortBy, sortOrder);
        return Ok(ApiResponse.SuccessResponse("Operation completed successfully.", products));
    }

    [HttpGet("{id:guid}")]
    [HasPermission("Permissions.Products.View")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null) return NotFound(ApiResponse.ErrorResponse("Resource not found."));
        return Ok(ApiResponse.SuccessResponse("Operation completed successfully.", product));
    }

    [HttpPost]
    [HasPermission("Permissions.Products.Create")]
    public async Task<IActionResult> Create(CreateProductDto dto)
    {
        try
        {
            var product = await _productService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, ApiResponse.SuccessResponse("Operation completed successfully.", product));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse.ErrorResponse(ex.Message));
        }
    }

    [HttpPut("{id:guid}")]
    [HasPermission("Permissions.Products.Update")]
    public async Task<IActionResult> Update(Guid id, UpdateProductDto dto)
    {
        try
        {
            var product = await _productService.UpdateAsync(id, dto);
            if (product == null) return NotFound(ApiResponse.ErrorResponse("Resource not found."));
            return Ok(ApiResponse.SuccessResponse("Operation completed successfully.", product));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse.ErrorResponse(ex.Message));
        }
    }

    [HttpDelete("{id:guid}")]
    [HasPermission("Permissions.Products.Delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _productService.DeleteAsync(id);
        if (!deleted) return NotFound(ApiResponse.ErrorResponse("Resource not found."));
        return NoContent();
    }
}
