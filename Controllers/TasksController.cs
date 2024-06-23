using GakkoHorizontalSlice.Exceptions;
using GakkoHorizontalSlice.Model;
using GakkoHorizontalSlice.Services;
using Microsoft.AspNetCore.Mvc;

namespace GakkoHorizontalSlice.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TasksController : ControllerBase
{
    private ITasksService _tasksService;
    
    public TasksController(ITasksService tasksService)
    {
        _tasksService = tasksService;
    }
    
    /// <summary>
    /// Endpoints used to return list of team members.
    /// </summary>
    /// <returns>List of team members</returns>
    [HttpGet]
    public async Task<IActionResult> GetTeamMembers()
    {
        var students = await _tasksService.GetTeamMembers();
        return Ok(students);
    }
    
    /// <summary>
    /// Endpoint used to return a single student.
    /// </summary>
    /// <param name="id">Id of a student</param>
    /// <returns>Student</returns>
    [HttpGet("/api/tasks/members/{id:int}")]
    public async Task<IActionResult> GetTeamMember(int id)
    {
        var student = await _tasksService.GetTeamMember(id);

        if (student==null)
        {
            return NotFound("Student not found");
        }
        
        return Ok(student);
    }
    
    /// <summary>
    /// Endpoint used to create a student.
    /// </summary>
    /// <param name="teamMember">New student data</param>
    /// <returns>201 Created</returns>
    [HttpPost]
    public async Task<IActionResult> CreateTask(TaskRequest task)
    {
        try
        {
            var createdTaskID = await _tasksService.CreateTask(task);
            return Created("api/tasks/id", new
            {
                Id = createdTaskID
            });
        }
        catch (NotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    /// <summary>
    /// Endpoint used to update a student.
    /// </summary>
    /// <param name="id">Id of a student</param>
    /// <param name="teamMember">204 No Content</param>
    /// <returns></returns>
    [HttpPut("/api/tasks/members/{id:int}")]
    public async Task<IActionResult> UpdateTeamMember(int id, UpdateTeamMemberRequest teamMemberRequest)
    {
        try
        {
            await _tasksService.UpdateTeamMember(id, teamMemberRequest);
            return NoContent();
        }
        catch (NotFoundException e)
        {
            return BadRequest(e.Message);
        }
    }
    
    /// <summary>
    /// Endpoint used to delete a team member.
    /// </summary>
    /// <param name="id">Id of a member</param>
    /// <returns>204 No Content</returns>
    [HttpDelete("/api/tasks/members/{id:int}")]
    public async Task<IActionResult> DeleteTeamMember(int id)
    {
        try
        {
            await _tasksService.DeleteTeamMember(id);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}