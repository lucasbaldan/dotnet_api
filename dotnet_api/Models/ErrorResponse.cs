namespace dotnet_api.Models;
public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string? Message { get; set; }
    public IEnumerable<string>? Errors { get; set; }
    public string? StackTrace { get; set; }

}
