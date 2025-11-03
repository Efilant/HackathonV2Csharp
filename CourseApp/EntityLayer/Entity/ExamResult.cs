namespace CourseApp.EntityLayer.Entity;

public class ExamResult:BaseEntity
{
    public byte Grade { get; set; }
    public string ExamID { get; set; } = string.Empty;
    public string StudentID { get; set; } = string.Empty;
    public Student? Student { get; set; }
    public Exam? Exam { get; set; }
}
