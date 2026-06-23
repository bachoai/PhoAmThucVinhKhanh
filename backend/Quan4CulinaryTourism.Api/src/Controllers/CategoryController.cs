using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quan4CulinaryTourism.Api.Common;
using Quan4CulinaryTourism.Api.DTOs;
using Quan4CulinaryTourism.Api.Models;
using Quan4CulinaryTourism.Api.Services;

namespace Quan4CulinaryTourism.Api.Controllers;

[Route($"{AppConstants.ApiVersionPrefix}/categories")]
public class CategoryController : BaseApiController
{
    private readonly CategoryService _categoryService;
    public CategoryController(CategoryService categoryService) => _categoryService = categoryService;

    [HttpGet]
    public Task<IActionResult> GetAll() => ExecuteAsync(() => _categoryService.GetAllAsync(true), "Lấy danh mục thành công");

    [HttpGet("{id}")]
    public Task<IActionResult> GetById(string id) => ExecuteAsync(() => _categoryService.GetByIdAsync(id), "Lấy danh mục thành công");

    [Authorize(Roles = SharedConstants.UserRoles.Admin)]
    [HttpPost]
    public Task<IActionResult> Create([FromBody] CreateCategoryRequest request) => ExecuteAsync(() => _categoryService.CreateAsync(request), "Tạo danh mục thành công");

    [Authorize(Roles = SharedConstants.UserRoles.Admin)]
    [HttpPut("{id}")]
    public Task<IActionResult> Update(string id, [FromBody] UpdateCategoryRequest request) => ExecuteAsync(() => _categoryService.UpdateAsync(id, request), "Cập nhật danh mục thành công");

    [Authorize(Roles = SharedConstants.UserRoles.Admin)]
    [HttpDelete("{id}")]
    public Task<IActionResult> Delete(string id) => ExecuteAsync(() => _categoryService.DeleteAsync(id), "Xóa danh mục thành công");
}
