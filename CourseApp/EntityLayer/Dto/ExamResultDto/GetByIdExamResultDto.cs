namespace CourseApp.EntityLayer.Dto.ExamResultDto;

public class GetByIdExamResultDto
{
    public string Id { get; set; } = null!;
    public byte Grade { get; set; }
    public string Name { get; set; } = null!;   
    public string Surname { get; set; } = null!;
    public string? ExamID { get; set; }
    public string? StudentID { get; set; }
}
