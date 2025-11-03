namespace CourseApp.EntityLayer.Entity;

public class Student : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Fullname => $"{Name} {Surname}";
    public DateTime BirthDate { get; set; }
    public string TC { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"{TC}-{Fullname}";
    }
}
