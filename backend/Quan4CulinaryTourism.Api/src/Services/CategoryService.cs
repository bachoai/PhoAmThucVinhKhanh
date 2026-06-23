using Microsoft.AspNetCore.Http;
using Quan4CulinaryTourism.Api.Common;
using Quan4CulinaryTourism.Api.DTOs;
using Quan4CulinaryTourism.Api.Models;
using Quan4CulinaryTourism.Api.Repositories;

namespace Quan4CulinaryTourism.Api.Services;

public class CategoryService
{
    private readonly CategoryRepository _categoryRepository;

    public CategoryService(CategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<List<CategoryResponse>> GetAllAsync(bool activeOnly = true, CancellationToken cancellationToken = default)
    {
        var categories = activeOnly
            ? await _categoryRepository.GetAllActiveAsync(cancellationToken)
            : await _categoryRepository.GetAllAsync(cancellationToken);
        return categories.Select(ToResponse).ToList();
    }

    public async Task<CategoryResponse> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new ApiException("Không tìm thấy danh mục.", StatusCodes.Status404NotFound);
        return ToResponse(category);
    }

    public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await _categoryRepository.GetByCodeAsync(request.Code, cancellationToken);
        if (existing is not null)
        {
            throw new ApiException("Mã danh mục đã tồn tại.");
        }

        var category = new Category
        {
            Code = request.Code,
            Name = request.Name,
            Description = request.Description,
            IconUrl = request.IconUrl,
            SortOrder = request.SortOrder
        };

        await _categoryRepository.CreateAsync(category, cancellationToken);
        return ToResponse(category);
    }

    public async Task<CategoryResponse> UpdateAsync(string id, UpdateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new ApiException("Không tìm thấy danh mục.", StatusCodes.Status404NotFound);

        category.Code = request.Code;
        category.Name = request.Name;
        category.Description = request.Description;
        category.IconUrl = request.IconUrl;
        category.SortOrder = request.SortOrder;
        category.IsActive = request.IsActive;

        await _categoryRepository.UpdateAsync(category, cancellationToken);
        return ToResponse(category);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new ApiException("Không tìm thấy danh mục.", StatusCodes.Status404NotFound);
        await _categoryRepository.SoftDeleteAsync(category.Id, cancellationToken);
    }

    private static CategoryResponse ToResponse(Category category) => new()
    {
        Id = category.Id,
        Code = category.Code,
        Name = category.Name,
        Description = category.Description,
        IconUrl = category.IconUrl,
        SortOrder = category.SortOrder,
        IsActive = category.IsActive
    };
}
