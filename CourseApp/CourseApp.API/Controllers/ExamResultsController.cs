using CourseApp.EntityLayer.Dto.ExamResultDto;
using CourseApp.ServiceLayer.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace CourseApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExamResultsController : ControllerBase
{
    private readonly IExamResultService _examResultService;

    public ExamResultsController(IExamResultService examResultService)
    {
        _examResultService = examResultService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _examResultService.GetAllAsync();
        if (result.IsSuccess)
            {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest("Id is required");
        }
        
        var result = await _examResultService.GetByIdAsync(id);
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpGet("detail")]
    public async Task<IActionResult> GetAllDetail()
    {
        var result = await _examResultService.GetAllExamResultDetailAsync();
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpGet("detail/{id}")]
    public async Task<IActionResult> GetByIdDetail(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest("Id is required");
        }
        
        var result = await _examResultService.GetByIdExamResultDetailAsync(id);
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateExamResultDto createExamResultDto)
    {
        if (createExamResultDto == null)
        {
            return BadRequest("Request body cannot be null");
        }
        
        var result = await _examResultService.CreateAsync(createExamResultDto);
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateExamResultDto updateExamResultDto)
    {
        if (updateExamResultDto == null)
        {
            return BadRequest("Request body cannot be null");
        }
        
        var result = await _examResultService.Update(updateExamResultDto);
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest("Id is required");
        }
        
        var deleteDto = new DeleteExamResultDto { Id = id };
        var result = await _examResultService.Remove(deleteDto);
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }
}
