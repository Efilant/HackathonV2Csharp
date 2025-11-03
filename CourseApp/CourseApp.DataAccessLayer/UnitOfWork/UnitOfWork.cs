using CourseApp.DataAccessLayer.Abstract;
using CourseApp.DataAccessLayer.Concrete;

namespace CourseApp.DataAccessLayer.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private readonly Lazy<StudentRepository> _studentRepository;
    private readonly Lazy<LessonRepository> _lessonRepository;
    private readonly Lazy<CourseRepository> _courseRepository;
    private readonly Lazy<RegistrationRepository> _registrationRepository;
    private readonly Lazy<ExamRepository> _examRepository;
    private readonly Lazy<ExamResultRepository> _examResultRepository;
    private readonly Lazy<InstructorRepository> _instructorRepository;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        _studentRepository = new Lazy<StudentRepository>(() => new StudentRepository(_context));
        _lessonRepository = new Lazy<LessonRepository>(() => new LessonRepository(_context));
        _courseRepository = new Lazy<CourseRepository>(() => new CourseRepository(_context));
        _registrationRepository = new Lazy<RegistrationRepository>(() => new RegistrationRepository(_context));
        _examRepository = new Lazy<ExamRepository>(() => new ExamRepository(_context));
        _examResultRepository = new Lazy<ExamResultRepository>(() => new ExamResultRepository(_context));
        _instructorRepository = new Lazy<InstructorRepository>(() => new InstructorRepository(_context));
    }

    public IStudentRepository Students => _studentRepository.Value;

    public ILessonRepository Lessons => _lessonRepository.Value;

    public ICourseRepository Courses => _courseRepository.Value;

    public IExamRepository Exams => _examRepository.Value;

    public IExamResultRepository ExamResults => _examResultRepository.Value;

    public IInstructorRepository Instructors => _instructorRepository.Value;

    public IRegistrationRepository Registrations => _registrationRepository.Value;

    public async Task<int> CommitAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
    }
}
