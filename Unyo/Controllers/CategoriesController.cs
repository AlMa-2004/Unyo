using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unyo.Models;
using Unyo.Services;

namespace Unyo.Controllers;

[ApiController]
[Route("api/[controller]")] // Endpoint: api/categories
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    // GET: api/categories
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Category>>> GetCategories(CancellationToken cancellationToken)
    {
        var categories = await _categoryService.GetAllCategoriesAsync(cancellationToken);
        return Ok(categories);
    }

    // GET: api/categories/5
    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult<Category>> GetCategory(int id, CancellationToken cancellationToken)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id, cancellationToken);
        if (category == null)
            return NotFound();

        return Ok(category);
    }

    // POST: api/categories
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<Category>> CreateCategory(Category category, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var createdCategory = await _categoryService.CreateCategoryAsync(category, cancellationToken);
        return CreatedAtAction(nameof(GetCategory), new { id = createdCategory.Id }, createdCategory);
    }

    // DELETE: api/categories/5
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id, CancellationToken cancellationToken)
    {
        var result = await _categoryService.DeleteCategoryAsync(id, cancellationToken);
        if (!result)
            return NotFound();

        return NoContent();
    }
}