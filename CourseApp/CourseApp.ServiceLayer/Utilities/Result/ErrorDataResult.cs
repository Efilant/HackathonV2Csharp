namespace CourseApp.ServiceLayer.Utilities.Result;

public class ErrorDataResult<T>:DataResult<T>
{
    public ErrorDataResult(T? data,string message):base(data!,false,message ?? string.Empty)
    {
        
    }
    public ErrorDataResult(T? data) : base(data!,false,string.Empty)
    {
        
    }

}
