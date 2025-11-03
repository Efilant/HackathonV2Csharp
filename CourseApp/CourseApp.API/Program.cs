using CourseApp.DataAccessLayer.Concrete;
using CourseApp.DataAccessLayer.UnitOfWork;
using CourseApp.ServiceLayer.Abstract;
using CourseApp.ServiceLayer.Concrete;
using CourseApp.ServiceLayer.Mapping;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext Configuration
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// UnitOfWork Configuration
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Service Configuration
builder.Services.AddScoped<IStudentService, StudentManager>();
builder.Services.AddScoped<ICourseService, CourseManager>();
builder.Services.AddScoped<IExamService, ExamManager>();
builder.Services.AddScoped<IExamResultService, ExamResultManager>();
builder.Services.AddScoped<IInstructorService, InstructorManager>();
builder.Services.AddScoped<ILessonService, LessonsManager>();
builder.Services.AddScoped<IRegistrationService, RegistrationManager>();

// AutoMapper Configuration
builder.Services.AddAutoMapper(typeof(StudentMapping).Assembly);

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Veritabanını otomatik oluştur (Development'ta)
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HTTPS redirection sadece Production'da (Mac'te sorun çıkarmasın)
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();