namespace Quan4CulinaryTourism.Api.Common;

public class ApiException(string message, int statusCode = StatusCodes.Status400BadRequest, IEnumerable<string>? errors = null) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
    public IReadOnlyCollection<string> Errors { get; } = errors?.ToList() ?? [];
}
