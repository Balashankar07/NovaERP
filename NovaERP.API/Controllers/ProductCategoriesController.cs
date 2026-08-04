using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NovaERP.API.Authorization;
using NovaERP.Application.Common.Models;
using NovaERP.Application.Features.ProductCategories.DTOs;
using NovaERP.Application.Interfaces.Services;

namespace NovaERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductCategoriesController : ControllerBase
{
    private readonly IProductCategoryService _productCategoryService;

    public ProductCategoriesController(IProductCategoryService productCategoryService)
    {
        _productCategoryService = productCategoryService;
    }

    [HttpGet]
    [HasPermission("Permissions.ProductCategories.View")]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] string? sortBy = null, [FromQuery] string? sortOrder = null)
    {
        var categories = await _productCategoryService.GetAllAsync(pageNumber, pageSize, search, sortBy, sortOrder);
        return Ok(ApiResponse.SuccessResponse("Operation completed successfully.", categories));
    }

    [HttpGet("{id:guid}")]
    [HasPermission("Permissions.ProductCategories.View")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var category = await _productCategoryService.GetByIdAsync(id);
        if (category == null) return NotFound(ApiResponse.ErrorResponse("Resource not found."));
        return Ok(ApiResponse.SuccessResponse("Operation completed successfully.", category));
    }

    [HttpPost]
    [HasPermission("Permissions.ProductCategories.Create")]
    public async Task<IActionResult> Create(CreateProductCategoryDto dto)
    {
        var category = await _productCategoryService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, ApiResponse.SuccessResponse("Operation completed successfully.", category));
    }

    [HttpPut("{id:guid}")]
    [HasPermission("Permissions.ProductCategories.Update")]
    public async Task<IActionResult> Update(Guid id, UpdateProductCategoryDto dto)
    {
        var category = await _productCategoryService.UpdateAsync(id, dto);
        if (category == null) return NotFound(ApiResponse.ErrorResponse("Resource not found."));
        return Ok(ApiResponse.SuccessResponse("Operation completed successfully.", category));
    }

    [HttpDelete("{id:guid}")]
    [HasPermission("Permissions.ProductCategories.Delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _productCategoryService.DeleteAsync(id);
        if (!deleted) return NotFound(ApiResponse.ErrorResponse("Resource not found."));
        return NoContent();
    }
}
