namespace Quan4CulinaryTourism.Api.Common;

public class ErrorResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = "Thất bại";
    public List<string> Errors { get; set; } = [];

    public static ErrorResponse From(string message, params string[] errors)
    {
        return new ErrorResponse
        {
            Success = false,
            Message = message,
            Errors = errors.Where(static item => !string.IsNullOrWhiteSpace(item)).ToList()
        };
    }
}
