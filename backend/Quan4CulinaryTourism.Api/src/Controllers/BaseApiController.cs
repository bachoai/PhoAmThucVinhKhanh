using Microsoft.AspNetCore.Mvc;
using Quan4CulinaryTourism.Api.Common;

namespace Quan4CulinaryTourism.Api.Controllers;

[ApiController]
public abstract class BaseApiController : ControllerBase
{
    protected async Task<IActionResult> ExecuteAsync<T>(Func<Task<T>> action, string message = "Thành công")
    {
        try
        {
            var result = await action();
            return Ok(ApiResponse<T>.Ok(result, message));
        }
        catch (ApiException ex)
        {
            return StatusCode(ex.StatusCode, ErrorResponse.From(ex.Message, ex.Errors.ToArray()));
        }
    }

    protected async Task<IActionResult> ExecuteAsync(Func<Task> action, string message = "Thành công")
    {
        try
        {
            await action();
            return Ok(ApiResponse<object?>.Ok(null, message));
        }
        catch (ApiException ex)
        {
            return StatusCode(ex.StatusCode, ErrorResponse.From(ex.Message, ex.Errors.ToArray()));
        }
    }
}
