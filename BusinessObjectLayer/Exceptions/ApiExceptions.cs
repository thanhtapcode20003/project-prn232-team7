namespace BusinessObjectLayer.Exceptions;

/// <summary>
/// Standard API Error Response
/// </summary>
public class ApiError
{
    public int StatusCode { get; set; }
    public string ErrorCode { get; set; }
    public string Message { get; set; }
    public string? Details { get; set; }
    public DateTime Timestamp { get; set; }

    public ApiError(int statusCode, string errorCode, string message, string? details = null)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
        Message = message;
        Details = details;
        Timestamp = DateTime.UtcNow;
    }

    // Factory methods cho các l?i th??ng g?p
    public static ApiError BadRequest(string message, string? details = null)
        => new(400, "HB40001", message, details);

    public static ApiError Unauthorized(string message = "Token missing or invalid", string? details = null)
        => new(401, "HB40101", message, details);

    public static ApiError Forbidden(string message = "Permission denied", string? details = null)
        => new(403, "HB40301", message, details);

    public static ApiError NotFound(string resource, string? details = null)
        => new(404, "HB40401", $"{resource} not found", details);

    public static ApiError InternalServerError(string message = "Internal server error", string? details = null)
        => new(500, "HB50001", message, details);

    public static ApiError Conflict(string message, string? details = null)
        => new(409, "HB40901", message, details);

    public static ApiError ValidationError(string message, string? details = null)
        => new(422, "HB42201", message, details);

    // Get error by status code
    public static ApiError FromStatusCode(int statusCode, string? customMessage = null)
    {
        return statusCode switch
        {
            400 => BadRequest(customMessage ?? "Missing or invalid input"),
            401 => Unauthorized(customMessage),
            403 => Forbidden(customMessage),
            404 => NotFound(customMessage ?? "Resource", null),
            409 => Conflict(customMessage ?? "Resource already exists"),
            422 => ValidationError(customMessage ?? "Validation failed"),
            500 => InternalServerError(customMessage),
            _ => new ApiError(statusCode, "HB00000", customMessage ?? "Unknown error")
        };
    }
}

/// <summary>
/// Standard API Success Response
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }

    public ApiResponse() { }

    public ApiResponse(T data)
    {
        Success = true;
        Data = data;
    }

    public static ApiResponse<T> Ok(T data, string message = "Success")
        => new() { Success = true, Message = message, Data = data };
}

/// <summary>
/// Paginated Response
/// </summary>
public class PagedResponse<T> : ApiResponse<IEnumerable<T>>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }

    public PagedResponse(IEnumerable<T> data, int page, int pageSize, int totalItems)
        : base(data)
    {
        Page = page;
        PageSize = pageSize;
        TotalItems = totalItems;
        TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
    }
}

/// <summary>
/// Custom Exception v?i ApiError
/// </summary>
public class ApiException : Exception
{
    public ApiError Error { get; }

    public ApiException(ApiError error) : base(error.Message)
    {
        Error = error;
    }

    public ApiException(int statusCode, string errorCode, string message, string? details = null)
        : base(message)
    {
        Error = new ApiError(statusCode, errorCode, message, details);
    }
}

/// <summary>
/// Validation Exception
/// </summary>
public class ValidationException : ApiException
{
    public Dictionary<string, string[]> Errors { get; }

    public ValidationException(Dictionary<string, string[]> errors)
        : base(ApiError.ValidationError("Validation failed", string.Join("; ", errors.SelectMany(e => e.Value))))
    {
        Errors = errors;
    }
}

/// <summary>
/// Not Found Exception
/// </summary>
public class NotFoundException : ApiException
{
    public NotFoundException(string resource, string? id = null)
        : base(ApiError.NotFound(resource, id != null ? $"ID: {id}" : null))
    {
    }
}

/// <summary>
/// Unauthorized Exception
/// </summary>
public class UnauthorizedException : ApiException
{
    public UnauthorizedException(string message = "Unauthorized access")
        : base(ApiError.Unauthorized(message))
    {
    }
}
