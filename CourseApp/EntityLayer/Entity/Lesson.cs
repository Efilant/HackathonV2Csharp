namespace CourseApp.EntityLayer.Entity;

public class Lesson : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public byte Duration { get; set; }
    public string? Content { get; set; }
    public string CourseID { get; set; } = string.Empty;
    public string? Time { get; set; }

    //navigation property
    public Course? Course { get; set; }
}
