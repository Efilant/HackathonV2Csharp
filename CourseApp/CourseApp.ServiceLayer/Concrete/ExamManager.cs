using AutoMapper;
using CourseApp.DataAccessLayer.UnitOfWork;
using CourseApp.EntityLayer.Dto.ExamDto;
using CourseApp.EntityLayer.Entity;
using CourseApp.ServiceLayer.Abstract;
using CourseApp.ServiceLayer.Utilities.Constants;
using CourseApp.ServiceLayer.Utilities.Result;
using Microsoft.EntityFrameworkCore;

namespace CourseApp.ServiceLayer.Concrete;

public class ExamManager : IExamService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ExamManager(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IDataResult<IEnumerable<GetAllExamDto>>> GetAllAsync(bool track = true)
    {
        var examList = await _unitOfWork.Exams.GetAll(false).ToListAsync();
        var examListMapping = _mapper.Map<IEnumerable<GetAllExamDto>>(examList);
        
        return new SuccessDataResult<IEnumerable<GetAllExamDto>>(examListMapping, ConstantsMessages.ExamListSuccessMessage);
    }

    public async Task<IDataResult<GetByIdExamDto>> GetByIdAsync(string id, bool track = true)
    {
        if (string.IsNullOrEmpty(id))
        {
            return new ErrorDataResult<GetByIdExamDto>(null, "Id is required");
        }
        
        var hasExam = await _unitOfWork.Exams.GetByIdAsync(id, false);
        if (hasExam == null)
        {
            return new ErrorDataResult<GetByIdExamDto>(null, "Exam not found");
        }
        
        var examResultMapping = _mapper.Map<GetByIdExamDto>(hasExam);
        return new SuccessDataResult<GetByIdExamDto>(examResultMapping, ConstantsMessages.ExamGetByIdSuccessMessage);
    }

    public async Task<IResult> CreateAsync(CreateExamDto entity)
    {
        if (string.IsNullOrWhiteSpace(entity.Name))
        {
            return new ErrorResult("Name is required");
        }
        
        var addedExamMapping = _mapper.Map<Exam>(entity);
        if (addedExamMapping == null)
        {
            return new ErrorResult("Failed to map entity");
        }
        
        await _unitOfWork.Exams.CreateAsync(addedExamMapping);
        var result = await _unitOfWork.CommitAsync();
        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.ExamCreateSuccessMessage);
        }
        return new ErrorResult(ConstantsMessages.ExamCreateFailedMessage);
    }

    public async Task<IResult> Remove(DeleteExamDto entity)
    {
        if (entity == null || string.IsNullOrEmpty(entity.Id))
        {
            return new ErrorResult("Entity ID is required");
        }
        
        var deletedExamMapping = _mapper.Map<Exam>(entity);
        if (deletedExamMapping == null)
        {
            return new ErrorResult("Failed to map entity");
        }
        
        _unitOfWork.Exams.Remove(deletedExamMapping);
        var result = await _unitOfWork.CommitAsync();
        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.ExamDeleteSuccessMessage);
        }
        return new ErrorResult(ConstantsMessages.ExamDeleteFailedMessage);
    }

    public async Task<IResult> Update(UpdateExamDto entity)
    {
        var updatedExamMapping = _mapper.Map<Exam>(entity);
        if (updatedExamMapping == null)
        {
            return new ErrorResult("Failed to map entity");
        }
        
        _unitOfWork.Exams.Update(updatedExamMapping);
        var result = await _unitOfWork.CommitAsync();
        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.ExamUpdateSuccessMessage);
        }
        return new ErrorResult(ConstantsMessages.ExamUpdateFailedMessage);
    }
}
