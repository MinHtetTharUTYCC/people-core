namespace PeopleCore.Application.Common;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public IEnumerable<ValidationError> Errors { get; set; } = Enumerable.Empty<ValidationError>();

    public static ApiResponse<T> Ok(T data, string? message = null)
    {
        return new ApiResponse<T> { Success = true, Data = data, Message = message };
    }

    public static ApiResponse<T> Error(string message, IEnumerable<ValidationError>? errors = null)
    {
        return new ApiResponse<T> { Success = false, Message = message, Errors = errors ?? Enumerable.Empty<ValidationError>() };
    }
}

public class ValidationError
{
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
