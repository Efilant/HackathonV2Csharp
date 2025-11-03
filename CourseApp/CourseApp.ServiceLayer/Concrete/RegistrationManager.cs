using AutoMapper;
using CourseApp.DataAccessLayer.UnitOfWork;
using CourseApp.EntityLayer.Dto.RegistrationDto;
using CourseApp.EntityLayer.Entity;
using CourseApp.ServiceLayer.Abstract;
using CourseApp.ServiceLayer.Utilities.Constants;
using CourseApp.ServiceLayer.Utilities.Result;
using Microsoft.EntityFrameworkCore;

namespace CourseApp.ServiceLayer.Concrete;

public class RegistrationManager : IRegistrationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public RegistrationManager(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IDataResult<IEnumerable<GetAllRegistrationDto>>> GetAllAsync(bool track = true)
    {
        var registrationList = await _unitOfWork.Registrations.GetAll(false).ToListAsync();
        var registrationListMapping = _mapper.Map<IEnumerable<GetAllRegistrationDto>>(registrationList);
        if (!registrationList.Any())
        {
            return new ErrorDataResult<IEnumerable<GetAllRegistrationDto>>(null, ConstantsMessages.RegistrationListFailedMessage);
        }
        return new SuccessDataResult<IEnumerable<GetAllRegistrationDto>>(registrationListMapping, ConstantsMessages.RegistrationListSuccessMessage);
    }

    public async Task<IDataResult<GetByIdRegistrationDto>> GetByIdAsync(string id, bool track = true)
    {
        var hasRegistration = await _unitOfWork.Registrations.GetByIdAsync(id, false);
        var hasRegistrationMapping = _mapper.Map<GetByIdRegistrationDto>(hasRegistration);
        return new SuccessDataResult<GetByIdRegistrationDto>(hasRegistrationMapping, ConstantsMessages.RegistrationGetByIdSuccessMessage);
    }

    public async Task<IResult> CreateAsync(CreateRegistrationDto entity)
    {
        if (entity == null)
        {
            return new ErrorResult("Entity cannot be null");
        }
        
        var createdRegistration = _mapper.Map<Registration>(entity);
        if (createdRegistration == null)
        {
            return new ErrorResult("Failed to map entity");
        }
        
        await _unitOfWork.Registrations.CreateAsync(createdRegistration);
        var result = await _unitOfWork.CommitAsync();
        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.RegistrationCreateSuccessMessage);
        }

        return new ErrorResult(ConstantsMessages.RegistrationCreateFailedMessage);
    }

    public async Task<IResult> Remove(DeleteRegistrationDto entity)
    {
        if (entity == null || string.IsNullOrEmpty(entity.Id))
        {
            return new ErrorResult("Entity ID is required");
        }
        
        var deletedRegistration = _mapper.Map<Registration>(entity);
        if (deletedRegistration == null)
        {
            return new ErrorResult("Failed to map entity");
        }
        
        _unitOfWork.Registrations.Remove(deletedRegistration);
        var result = await _unitOfWork.CommitAsync();
        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.RegistrationDeleteSuccessMessage);
        }
        return new ErrorResult(ConstantsMessages.RegistrationDeleteFailedMessage);
    }

    public async Task<IResult> Update(UpdatedRegistrationDto entity)
    {
        if (entity == null)
        {
            return new ErrorResult("Entity cannot be null");
        }
        
        var updatedRegistration = _mapper.Map<Registration>(entity);
        if (updatedRegistration == null)
        {
            return new ErrorResult("Failed to map entity");
        }
        
        _unitOfWork.Registrations.Update(updatedRegistration);
        var result = await _unitOfWork.CommitAsync();
        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.RegistrationUpdateSuccessMessage);
        }
        return new ErrorResult(ConstantsMessages.RegistrationUpdateFailedMessage);
    }

    public async Task<IDataResult<IEnumerable<GetAllRegistrationDetailDto>>> GetAllRegistrationDetailAsync(bool track = true)
    {
        var registrationData = await _unitOfWork.Registrations.GetAllRegistrationDetail(track).ToListAsync();
        
        if(!registrationData.Any())
        {
            return new ErrorDataResult<IEnumerable<GetAllRegistrationDetailDto>>(null,ConstantsMessages.RegistrationListFailedMessage);
        }

        var registrationDataMapping = _mapper.Map<IEnumerable<GetAllRegistrationDetailDto>>(registrationData);
        
        return new SuccessDataResult<IEnumerable<GetAllRegistrationDetailDto>>(registrationDataMapping, ConstantsMessages.RegistrationListSuccessMessage);  
    }

    public async Task<IDataResult<GetByIdRegistrationDetailDto>> GetByIdRegistrationDetailAsync(string id, bool track = true)
    {
        throw new NotImplementedException();
    }
}
