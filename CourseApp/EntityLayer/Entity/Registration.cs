namespace CourseApp.EntityLayer.Entity;

public class Registration : BaseEntity
{
    public DateTime RegistrationDate { get; set; } = DateTime.Now;
    public decimal Price { get; set; }
    public string StudentID { get; set; } = string.Empty;
    public string CourseID { get; set; } = string.Empty;
    public Course? Course { get; set; }
    public Student? Student { get; set; }
}
