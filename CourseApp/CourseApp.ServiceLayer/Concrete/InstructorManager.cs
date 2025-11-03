using AutoMapper;
using CourseApp.DataAccessLayer.UnitOfWork;
using CourseApp.EntityLayer.Dto.InstructorDto;
using CourseApp.EntityLayer.Entity;
using CourseApp.ServiceLayer.Abstract;
using CourseApp.ServiceLayer.Utilities.Constants;
using CourseApp.ServiceLayer.Utilities.Result;
using Microsoft.EntityFrameworkCore;

namespace CourseApp.ServiceLayer.Concrete;

public class InstructorManager : IInstructorService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public InstructorManager(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IDataResult<IEnumerable<GetAllInstructorDto>>> GetAllAsync(bool track = true)
    {
        try
        {
            var instructorList = await _unitOfWork.Instructors.GetAll(false).ToListAsync();
            var instructorListMapping = _mapper.Map<IEnumerable<GetAllInstructorDto>>(instructorList);
            
            // Boş liste normal bir durum, hata değil
            return new SuccessDataResult<IEnumerable<GetAllInstructorDto>>(instructorListMapping, 
                instructorList.Any() ? ConstantsMessages.InstructorListSuccessMessage : "Henüz eğitmen bulunmamaktadır.");
        }
        catch (Exception ex)
        {
            return new ErrorDataResult<IEnumerable<GetAllInstructorDto>>(Enumerable.Empty<GetAllInstructorDto>(), $"Eğitmenler listelenirken bir hata oluştu: {ex.Message}");
        }
    }

    public async Task<IDataResult<GetByIdInstructorDto>> GetByIdAsync(string id, bool track = true)
    {
        if (string.IsNullOrEmpty(id))
        {
            return new ErrorDataResult<GetByIdInstructorDto>(null, "Id is required");
        }
        
        var hasInstructor = await _unitOfWork.Instructors.GetByIdAsync(id, false);
        if (hasInstructor == null)
        {
            return new ErrorDataResult<GetByIdInstructorDto>(null, "Instructor not found");
        }
        
        var hasInstructorMapping = _mapper.Map<GetByIdInstructorDto>(hasInstructor);
        return new SuccessDataResult<GetByIdInstructorDto>(hasInstructorMapping, ConstantsMessages.InstructorGetByIdSuccessMessage);
    }

    public async Task<IResult> CreateAsync(CreatedInstructorDto entity)
    {
        if (string.IsNullOrWhiteSpace(entity.Name))
        {
            return new ErrorResult("Name is required");
        }
        if (string.IsNullOrWhiteSpace(entity.Surname))
        {
            return new ErrorResult("Surname is required");
        }
        
        var createdInstructor = _mapper.Map<Instructor>(entity);
        if (createdInstructor == null)
        {
            return new ErrorResult("Failed to map entity");
        }
        
        await _unitOfWork.Instructors.CreateAsync(createdInstructor);
        var result = await _unitOfWork.CommitAsync();
        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.InstructorCreateSuccessMessage);
        }
        return new ErrorResult(ConstantsMessages.InstructorCreateFailedMessage);
    }

    public async Task<IResult> Remove(DeletedInstructorDto entity)
    {
        if (entity == null || string.IsNullOrEmpty(entity.Id))
        {
            return new ErrorResult("Entity ID is required");
        }
        
        var deletedInstructor = _mapper.Map<Instructor>(entity);
        if (deletedInstructor == null)
        {
            return new ErrorResult("Failed to map entity");
        }
        
        _unitOfWork.Instructors.Remove(deletedInstructor);
        var result = await _unitOfWork.CommitAsync();
        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.InstructorDeleteSuccessMessage);
        }
        return new ErrorResult(ConstantsMessages.InstructorDeleteFailedMessage);
    }

    public async Task<IResult> Update(UpdatedInstructorDto entity)
    {
        if (string.IsNullOrEmpty(entity.Id))
        {
            return new ErrorResult("Entity ID is required");
        }
        
        var existingInstructor = await _unitOfWork.Instructors.GetByIdAsync(entity.Id, true);
        if (existingInstructor == null)
        {
            return new ErrorResult("Instructor not found");
        }
        
        // Sadece değişen property'leri güncelle (partial update)
        if (!string.IsNullOrWhiteSpace(entity.Name))
            existingInstructor.Name = entity.Name;
        if (!string.IsNullOrWhiteSpace(entity.Surname))
            existingInstructor.Surname = entity.Surname;
        if (entity.Email != null)
            existingInstructor.Email = entity.Email;
        if (entity.Professions != null)
            existingInstructor.Professions = entity.Professions;
        if (entity.PhoneNumber != null)
            existingInstructor.PhoneNumber = entity.PhoneNumber;
        
        _unitOfWork.Instructors.Update(existingInstructor);
        var result = await _unitOfWork.CommitAsync();
        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.InstructorUpdateSuccessMessage);
        }
        return new ErrorResult(ConstantsMessages.InstructorUpdateFailedMessage);
    }
}
