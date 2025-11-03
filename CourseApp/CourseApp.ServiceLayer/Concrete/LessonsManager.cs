using AutoMapper;
using CourseApp.DataAccessLayer.UnitOfWork;
using CourseApp.EntityLayer.Dto.LessonDto;
using CourseApp.EntityLayer.Entity;
using CourseApp.ServiceLayer.Abstract;
using CourseApp.ServiceLayer.Utilities.Constants;
using CourseApp.ServiceLayer.Utilities.Result;
using Microsoft.EntityFrameworkCore;

namespace CourseApp.ServiceLayer.Concrete;

public class LessonsManager : ILessonService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public LessonsManager(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<IDataResult<IEnumerable<GetAllLessonDto>>> GetAllAsync(bool track = true)
    {
        var lessonList = await _unitOfWork.Lessons.GetAll(false).ToListAsync();
        var lessonListMapping = _mapper.Map<IEnumerable<GetAllLessonDto>>(lessonList);
        
        // Boş liste normal bir durum, hata değil
        return new SuccessDataResult<IEnumerable<GetAllLessonDto>>(lessonListMapping, 
            lessonList.Any() ? ConstantsMessages.LessonListSuccessMessage : "Henüz ders bulunmamaktadır.");
    }

    public async Task<IDataResult<GetByIdLessonDto>> GetByIdAsync(string id, bool track = true)
    {
        if (string.IsNullOrEmpty(id))
        {
            return new ErrorDataResult<GetByIdLessonDto>(null, "Id is required");
        }
        
        var hasLesson = await _unitOfWork.Lessons.GetByIdAsync(id, false);
        if (hasLesson == null)
        {
            return new ErrorDataResult<GetByIdLessonDto>(null, "Lesson not found");
        }
        
        var hasLessonMapping = _mapper.Map<GetByIdLessonDto>(hasLesson);
        return new SuccessDataResult<GetByIdLessonDto>(hasLessonMapping, ConstantsMessages.LessonGetByIdSuccessMessage);
    }

    public async Task<IResult> CreateAsync(CreateLessonDto entity)
    {
        if (string.IsNullOrWhiteSpace(entity.Title))
        {
            return new ErrorResult("Title is required");
        }
        if (string.IsNullOrWhiteSpace(entity.CourseID))
        {
            return new ErrorResult("CourseID is required");
        }
        
        var createdLesson = _mapper.Map<Lesson>(entity);
        if (createdLesson == null)
        {
            return new ErrorResult("Failed to map entity");
        }
        
        await _unitOfWork.Lessons.CreateAsync(createdLesson);
        var result = await _unitOfWork.CommitAsync();
        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.LessonCreateSuccessMessage);
        }

        return new ErrorResult(ConstantsMessages.LessonCreateFailedMessage);
    }

    public async Task<IResult> Remove(DeleteLessonDto entity)
    {
        if (entity == null || string.IsNullOrEmpty(entity.Id))
        {
            return new ErrorResult("Entity ID is required");
        }
        
        var deletedLesson = _mapper.Map<Lesson>(entity);
        if (deletedLesson == null)
        {
            return new ErrorResult("Failed to map entity");
        }
        
        _unitOfWork.Lessons.Remove(deletedLesson);
        var result = await _unitOfWork.CommitAsync();
        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.LessonDeleteSuccessMessage);
        }
        return new ErrorResult(ConstantsMessages.LessonDeleteFailedMessage);
    }

    public async Task<IResult> Update(UpdateLessonDto entity)
    {
        var updatedLesson = _mapper.Map<Lesson>(entity);
        if (updatedLesson == null)
        {
            return new ErrorResult("Failed to map entity");
        }
        
        _unitOfWork.Lessons.Update(updatedLesson);
        var result = await _unitOfWork.CommitAsync();
        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.LessonUpdateSuccessMessage);
        }
        return new ErrorResult(ConstantsMessages.LessonUpdateFailedMessage);
    }

    public async Task<IDataResult<IEnumerable<GetAllLessonDetailDto>>> GetAllLessonDetailAsync(bool track = true)
    {
        var lessonList = await _unitOfWork.Lessons.GetAllLessonDetails(false).ToListAsync();
        
        // Boş liste normal bir durum, hata değil
        if (!lessonList.Any())
        {
            return new SuccessDataResult<IEnumerable<GetAllLessonDetailDto>>(Enumerable.Empty<GetAllLessonDetailDto>(), "Henüz ders bulunmamaktadır.");
        }
        
        var lessonsListMapping = _mapper.Map<IEnumerable<GetAllLessonDetailDto>>(lessonList);
   
        return new SuccessDataResult<IEnumerable<GetAllLessonDetailDto>>(lessonsListMapping, ConstantsMessages.LessonListSuccessMessage);
    }

    public async Task<IDataResult<GetByIdLessonDetailDto>> GetByIdLessonDetailAsync(string id, bool track = true)
    {
        if (string.IsNullOrEmpty(id))
        {
            return new ErrorDataResult<GetByIdLessonDetailDto>(null, "Id is required");
        }
        
        var lesson = await _unitOfWork.Lessons.GetByIdLessonDetailsAsync(id, false);
        if (lesson == null)
        {
            return new ErrorDataResult<GetByIdLessonDetailDto>(null, "Lesson not found");
        }
        
        var lessonMapping = _mapper.Map<GetByIdLessonDetailDto>(lesson);
        return new SuccessDataResult<GetByIdLessonDetailDto>(lessonMapping, ConstantsMessages.LessonGetByIdSuccessMessage);
    }
}
