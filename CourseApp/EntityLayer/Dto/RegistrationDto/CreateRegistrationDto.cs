namespace CourseApp.EntityLayer.Dto.RegistrationDto;

public class CreateRegistrationDto
{
    public DateTime RegistrationDate { get; set; } = DateTime.Now;
    public decimal Price { get; set; }
    public string StudentID { get; set; } = string.Empty;
    public string CourseID { get; set; } = string.Empty;
}
