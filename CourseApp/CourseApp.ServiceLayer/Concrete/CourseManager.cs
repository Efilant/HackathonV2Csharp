using CourseApp.DataAccessLayer.UnitOfWork;
using CourseApp.EntityLayer.Dto.CourseDto;
using CourseApp.EntityLayer.Entity;
using CourseApp.ServiceLayer.Abstract;
using CourseApp.ServiceLayer.Utilities.Constants;
using CourseApp.ServiceLayer.Utilities.Result;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Linq;

namespace CourseApp.ServiceLayer.Concrete;

public class CourseManager : ICourseService
{
    private readonly IUnitOfWork _unitOfWork;

    public CourseManager(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IDataResult<IEnumerable<GetAllCourseDto>>> GetAllAsync(bool track = true)
    {
        try
        {
            // Projection database seviyesinde yapılıyor - daha performanslı
            var courseList = await _unitOfWork.Courses.GetAll(false)
                .Select(course => new GetAllCourseDto
                {
                    CourseName = course.CourseName,
                    CreatedDate = course.CreatedDate,
                    EndDate = course.EndDate,
                    Id = course.ID,
                    InstructorID = course.InstructorID,
                    IsActive = course.IsActive,
                    StartDate = course.StartDate
                })
                .ToListAsync();

            // Boş liste normal bir durum, hata değil
            return new SuccessDataResult<IEnumerable<GetAllCourseDto>>(courseList, 
                courseList.Any() ? ConstantsMessages.CourseListSuccessMessage : "Henüz kurs bulunmamaktadır.");
        }
        catch (Exception ex)
        {
            return new ErrorDataResult<IEnumerable<GetAllCourseDto>>(Enumerable.Empty<GetAllCourseDto>(), $"Kurslar listelenirken bir hata oluştu: {ex.Message}");
        }
    }

    public async Task<IDataResult<GetByIdCourseDto>> GetByIdAsync(string id, bool track = true)
    {
        if (string.IsNullOrEmpty(id))
        {
            return new ErrorDataResult<GetByIdCourseDto>(null, "Id is required");
        }
        
        var hasCourse = await _unitOfWork.Courses.GetByIdAsync(id, track);
        if (hasCourse == null)
        {
            return new ErrorDataResult<GetByIdCourseDto>(null, "Course not found");
        }

        var course = new GetByIdCourseDto
        {
            CourseName = hasCourse.CourseName,
            CreatedDate = hasCourse.CreatedDate,
            EndDate = hasCourse.EndDate,
            InstructorID = hasCourse.InstructorID,
            IsActive = hasCourse.IsActive,
            StartDate = hasCourse.StartDate,
            Id = hasCourse.ID
        };

        return new SuccessDataResult<GetByIdCourseDto>(course, ConstantsMessages.CourseGetByIdSuccessMessage);
    }
    public async Task<IResult> CreateAsync(CreateCourseDto entity)
    {
        if (string.IsNullOrWhiteSpace(entity.CourseName))
        {
            return new ErrorResult("CourseName is required");
        }
        
        var createdCourse = new Course
        {
            CourseName = entity.CourseName,
            CreatedDate = entity.CreatedDate,
            EndDate = entity.EndDate,
            InstructorID = entity.InstructorID,
            IsActive = entity.IsActive,
            StartDate = entity.StartDate,
        };

        await _unitOfWork.Courses.CreateAsync(createdCourse);

        var result = await _unitOfWork.CommitAsync();

        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.CourseCreateSuccessMessage);
        }

        return new ErrorResult(ConstantsMessages.CourseCreateFailedMessage);
    }
    public async Task<IResult> Remove(DeleteCourseDto entity)
    {
        if (entity == null || string.IsNullOrEmpty(entity.Id))
        {
            return new ErrorResult("Entity ID is required");
        }
        
        var deletedCourse = new Course
        {
            ID = entity.Id,
        };
        _unitOfWork.Courses.Remove(deletedCourse);
        var result = await _unitOfWork.CommitAsync();
        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.CourseDeleteSuccessMessage);
        }

        return new ErrorResult(ConstantsMessages.CourseDeleteFailedMessage);
    }

    public async Task<IResult> Update(UpdateCourseDto entity)
    {
        if (string.IsNullOrEmpty(entity.Id))
        {
            return new ErrorResult("Entity ID is required");
        }
        
        var updatedCourse = await _unitOfWork.Courses.GetByIdAsync(entity.Id, true);
        if (updatedCourse == null)
        {
            return new ErrorResult("Course not found");
        }

        updatedCourse.CourseName = entity.CourseName ?? string.Empty;
        updatedCourse.StartDate = entity.StartDate;
        updatedCourse.EndDate = entity.EndDate;
        updatedCourse.InstructorID = entity.InstructorID;
        updatedCourse.IsActive = entity.IsActive;

        _unitOfWork.Courses.Update(updatedCourse);
        var result = await _unitOfWork.CommitAsync();
        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.CourseUpdateSuccessMessage);
        }
        return new ErrorResult(ConstantsMessages.CourseUpdateFailedMessage);
    }

    public async Task<IDataResult<IEnumerable<GetAllCourseDetailDto>>> GetAllCourseDetail(bool track = true)
    {
        // Projection database seviyesinde yapılıyor - daha performanslı
        var courseDetailDtoList = await _unitOfWork.Courses.GetAllCourseDetail(false)
            .Select(x => new GetAllCourseDetailDto
            {
                CourseName = x.CourseName,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                CreatedDate = x.CreatedDate,
                Id = x.ID,
                InstructorID = x.InstructorID ?? string.Empty,
                InstructorName = x.Instructor != null ? x.Instructor.Name ?? string.Empty : string.Empty,
                IsActive = x.IsActive,
            })
            .ToListAsync();

        // Boş liste normal bir durum, hata değil
        return new SuccessDataResult<IEnumerable<GetAllCourseDetailDto>>(courseDetailDtoList, 
            courseDetailDtoList.Any() ? ConstantsMessages.CourseDetailsFetchedSuccessfully : "Henüz kurs detayı bulunmamaktadır.");
    }

    private IResult CourseNameIsNullOrEmpty(string courseName)
    {
        if(string.IsNullOrWhiteSpace(courseName))
        {
            return new ErrorResult("Kurs Adı Boş Olamaz");
        }
        return new SuccessResult();
    }

    private async Task<IResult> CourseNameUniqeCheck(string id, string courseName)
    {
        // Aynı isimde başka bir kurs var mı kontrol et (kendi ID'sini hariç tut)
        var courseNameCheck = await _unitOfWork.Courses.GetAll(false)
            .AnyAsync(c => c.CourseName == courseName && c.ID != id);
        
        if(courseNameCheck)
        {
            return new ErrorResult("Bu kurs adı ile zaten bir kurs var");
        }
        return new SuccessResult();
    }

    private IResult CourseNameLenghtCehck(string courseName)
    {
        if(string.IsNullOrWhiteSpace(courseName) || courseName.Length < 2 || courseName.Length > 50)
        {
            return new ErrorResult("Kurs Adı Uzunluğu 2 - 50 Karakter Arasında Olmalı");
        }
        return new SuccessResult();
    }

    private IResult IsValidDateFormat(string date)
    {
        DateTime tempDate;
        bool isValid = DateTime.TryParse(date, out tempDate);

        if (!isValid)
        {
            return new ErrorResult("Geçersiz tarih formatı.");
        }
        return new SuccessResult();
    }
    private IResult CheckCourseDates(DateTime startDate, DateTime endDate)
    {
        if (endDate <= startDate)
        {
            return new ErrorResult("Bitiş tarihi, başlangıç tarihinden sonra olmalıdır.");
        }
        return new SuccessResult();
    }
    
    private IResult CheckInstructorNameIsNullOrEmpty(string instructorName)
    {
        if (string.IsNullOrEmpty(instructorName))
        {
            return new ErrorResult("Eğitmen alanı boş olamaz");
        }

        return new SuccessResult();
    }
}
