namespace CourseApp.EntityLayer.Dto.StudentDto;

public class CreateStudentDto
{
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Fullname => $"{Name} {Surname}";
    public DateTime BirthDate { get; set; }
    public string TC { get; set; } = string.Empty;
}
