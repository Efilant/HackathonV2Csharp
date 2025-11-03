namespace CourseApp.ServiceLayer.Utilities.Result;

public class Result : IResult
{
    public bool IsSuccess {  get; }

    public string Message { get; } = string.Empty;

    public Result(bool isSuccess)
    {
        IsSuccess = isSuccess;
        Message = string.Empty;
    }
    public Result(bool isSuccess,string message):this(isSuccess)
    {
        Message = message ?? string.Empty;
    }
}
