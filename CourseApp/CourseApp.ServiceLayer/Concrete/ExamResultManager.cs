using AutoMapper;
using CourseApp.DataAccessLayer.UnitOfWork;
using CourseApp.EntityLayer.Dto.ExamResultDto;
using CourseApp.EntityLayer.Entity;
using CourseApp.ServiceLayer.Abstract;
using CourseApp.ServiceLayer.Utilities.Constants;
using CourseApp.ServiceLayer.Utilities.Result;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace CourseApp.ServiceLayer.Concrete;

public class ExamResultManager : IExamResultService
{

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ExamResultManager(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IDataResult<IEnumerable<GetAllExamResultDto>>> GetAllAsync(bool track = true)
    {
        var examResultList = await _unitOfWork.ExamResults.GetAll(false).ToListAsync();
        var examResultListMapping = _mapper.Map<IEnumerable<GetAllExamResultDto>>(examResultList);
        
        // Boş liste normal bir durum, hata değil
        return new SuccessDataResult<IEnumerable<GetAllExamResultDto>>(examResultListMapping, 
            examResultList.Any() ? ConstantsMessages.ExamResultListSuccessMessage : "Henüz sınav sonucu bulunmamaktadır.");
    }

    public async Task<IDataResult<GetByIdExamResultDto>> GetByIdAsync(string id, bool track = true)
    {
        if (string.IsNullOrEmpty(id))
        {
            return new ErrorDataResult<GetByIdExamResultDto>(null, "Id is required");
        }
        
        var hasExamResult = await _unitOfWork.ExamResults.GetByIdAsync(id, false);
        if (hasExamResult == null)
        {
            return new ErrorDataResult<GetByIdExamResultDto>(null, "Exam result not found");
        }
        
        var examResultMapping = _mapper.Map<GetByIdExamResultDto>(hasExamResult);
        return new SuccessDataResult<GetByIdExamResultDto>(examResultMapping, ConstantsMessages.ExamResultGetByIdSuccessMessage);
    }

    public async Task<IResult> CreateAsync(CreateExamResultDto entity)
    {
        if (string.IsNullOrWhiteSpace(entity.ExamID))
        {
            return new ErrorResult("ExamID is required");
        }
        if (string.IsNullOrWhiteSpace(entity.StudentID))
        {
            return new ErrorResult("StudentID is required");
        }
        
        var addedExamResultMapping = _mapper.Map<ExamResult>(entity);
        if (addedExamResultMapping == null)
        {
            return new ErrorResult("Failed to map entity");
        }
        
        await _unitOfWork.ExamResults.CreateAsync(addedExamResultMapping);
        var result = await _unitOfWork.CommitAsync();
        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.ExamResultCreateSuccessMessage);
        }
        return new ErrorResult(ConstantsMessages.ExamResultCreateFailedMessage);
    }

    public async Task<IResult> Remove(DeleteExamResultDto entity)
    {
        if (entity == null || string.IsNullOrEmpty(entity.Id))
        {
            return new ErrorResult("Entity ID is required");
        }
        
        var deletedExamResultMapping = _mapper.Map<ExamResult>(entity);
        if (deletedExamResultMapping == null)
        {
            return new ErrorResult("Failed to map entity");
        }
        
        _unitOfWork.ExamResults.Remove(deletedExamResultMapping);
        var result = await _unitOfWork.CommitAsync();
        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.ExamResultDeleteSuccessMessage);
        }
        return new ErrorResult(ConstantsMessages.ExamResultDeleteFailedMessage);
    }

    public async Task<IResult> Update(UpdateExamResultDto entity)
    {
        if (string.IsNullOrEmpty(entity.Id))
        {
            return new ErrorResult("Entity ID is required");
        }
        
        var existingExamResult = await _unitOfWork.ExamResults.GetByIdAsync(entity.Id, true);
        if (existingExamResult == null)
        {
            return new ErrorResult("Exam result not found");
        }
        
        // Sadece değişen property'leri güncelle (partial update)
        existingExamResult.Grade = entity.Grade;
        if (!string.IsNullOrWhiteSpace(entity.ExamID))
            existingExamResult.ExamID = entity.ExamID;
        if (!string.IsNullOrWhiteSpace(entity.StudentID))
            existingExamResult.StudentID = entity.StudentID;
        
        _unitOfWork.ExamResults.Update(existingExamResult);
        var result = await _unitOfWork.CommitAsync();
        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.ExamResultUpdateSuccessMessage);
        }
        return new ErrorResult(ConstantsMessages.ExamResultUpdateFailedMessage);
    }

    public async Task<IDataResult<IEnumerable<GetAllExamResultDetailDto>>> GetAllExamResultDetailAsync(bool track = true)
    {
        var examResultList = await _unitOfWork.ExamResults.GetAllExamResultDetail(false).ToListAsync();
        var examResultListMapping = _mapper.Map<IEnumerable<GetAllExamResultDetailDto>>(examResultList);
        
        // Boş liste normal bir durum, hata değil
        return new SuccessDataResult<IEnumerable<GetAllExamResultDetailDto>>(examResultListMapping, 
            examResultList.Any() ? ConstantsMessages.ExamResultListSuccessMessage : "Henüz sınav sonucu bulunmamaktadır.");
    }

    public async Task<IDataResult<GetByIdExamResultDetailDto>> GetByIdExamResultDetailAsync(string id, bool track = true)
    {
        if (string.IsNullOrEmpty(id))
        {
            return new ErrorDataResult<GetByIdExamResultDetailDto>(null, "Id is required");
        }
        
        var examResult = await _unitOfWork.ExamResults.GetByIdExamResultDetailAsync(id, track);
        if (examResult == null)
        {
            return new ErrorDataResult<GetByIdExamResultDetailDto>(null, "Exam result not found");
        }
        
        var examResultMapping = _mapper.Map<GetByIdExamResultDetailDto>(examResult);
        return new SuccessDataResult<GetByIdExamResultDetailDto>(examResultMapping, ConstantsMessages.ExamResultGetByIdSuccessMessage);
    }
}
