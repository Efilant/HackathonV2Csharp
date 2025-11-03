namespace CourseApp.EntityLayer.Entity;

public class Exam:BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public DateTime Date { get; set; }
}
