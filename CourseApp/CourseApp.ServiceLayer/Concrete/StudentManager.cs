using AutoMapper;
using CourseApp.DataAccessLayer.UnitOfWork;
using CourseApp.EntityLayer.Dto.RegistrationDto;
using CourseApp.EntityLayer.Dto.StudentDto;
using CourseApp.EntityLayer.Entity;
using CourseApp.ServiceLayer.Abstract;
using CourseApp.ServiceLayer.Utilities.Constants;
using CourseApp.ServiceLayer.Utilities.Result;
using Microsoft.EntityFrameworkCore;

namespace CourseApp.ServiceLayer.Concrete;

public class StudentManager : IStudentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public StudentManager(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IDataResult<IEnumerable<GetAllStudentDto>>> GetAllAsync(bool track = true)
    {
        var studentList = await _unitOfWork.Students.GetAll(track).ToListAsync();
        var studentListMapping = _mapper.Map<IEnumerable<GetAllStudentDto>>(studentList);
        
        // Boş liste normal bir durum, hata değil
        return new SuccessDataResult<IEnumerable<GetAllStudentDto>>(studentListMapping, 
            studentList.Any() ? ConstantsMessages.StudentListSuccessMessage : "Henüz öğrenci bulunmamaktadır.");
    }

    public async Task<IDataResult<GetByIdStudentDto>> GetByIdAsync(string id, bool track = true)
    {
        if (string.IsNullOrEmpty(id))
        {
            return new ErrorDataResult<GetByIdStudentDto>(null, "Id is required");
        }
        
        var hasStudent = await _unitOfWork.Students.GetByIdAsync(id, false);
        if (hasStudent == null)
        {
            return new ErrorDataResult<GetByIdStudentDto>(null, "Student not found");
        }
        
        var hasStudentMapping = _mapper.Map<GetByIdStudentDto>(hasStudent);
        return new SuccessDataResult<GetByIdStudentDto>(hasStudentMapping, ConstantsMessages.StudentGetByIdSuccessMessage);
    }

    public async Task<IResult> CreateAsync(CreateStudentDto entity)
    {
        if(entity == null)
        {
            return new ErrorResult("Entity cannot be null");
        }
        
        var createdStudent = _mapper.Map<Student>(entity);
        if (createdStudent == null)
        {
            return new ErrorResult("Failed to map entity");
        }
        
        await _unitOfWork.Students.CreateAsync(createdStudent);
        var result = await _unitOfWork.CommitAsync();
        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.StudentCreateSuccessMessage);
        }

        return new ErrorResult(ConstantsMessages.StudentCreateFailedMessage);
    }

    public async Task<IResult> Remove(DeleteStudentDto entity)
    {
        if (entity == null || string.IsNullOrEmpty(entity.Id))
        {
            return new ErrorResult("Entity ID is required");
        }
        
        var deletedStudent = _mapper.Map<Student>(entity);
        if (deletedStudent == null)
        {
            return new ErrorResult("Failed to map entity");
        }
        
        _unitOfWork.Students.Remove(deletedStudent);
        var result = await _unitOfWork.CommitAsync();
        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.StudentDeleteSuccessMessage);
        }
        return new ErrorResult(ConstantsMessages.StudentDeleteFailedMessage);
    }

    public async Task<IResult> Update(UpdateStudentDto entity)
    {
        if (entity == null)
        {
            return new ErrorResult("Entity cannot be null");
        }
        
        var updatedStudent = _mapper.Map<Student>(entity);
        if (updatedStudent == null)
        {
            return new ErrorResult("Failed to map entity");
        }
        
        _unitOfWork.Students.Update(updatedStudent);
        var result = await _unitOfWork.CommitAsync();
        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.StudentUpdateSuccessMessage);
        }
        return new ErrorResult(ConstantsMessages.StudentUpdateFailedMessage);
    }

}
