using GakkoHorizontalSlice.Exceptions;
using GakkoHorizontalSlice.Model;
using GakkoHorizontalSlice.Repositories;

namespace GakkoHorizontalSlice.Services;

public class TasksService : ITasksService
{
    private readonly IStudentsRepository _studentsRepository;

    public TasksService(IStudentsRepository studentsRepository)
    {
        _studentsRepository = studentsRepository;
    }

    public async Task<IEnumerable<TeamMember>> GetTeamMembers()
    {
        //Business logic
        return await _studentsRepository.GetTeamMembers();
    }

    public async Task<int> CreateTask(TaskRequest taskReq)
    {
        if (!await _studentsRepository.IdProjectExist(taskReq))
        {
            throw new ProjectNotFound(taskReq.IdProject);
        }

        if (!await _studentsRepository.IdTaskTypeExists(taskReq))
        {
            throw new TaskTypeNotFound(taskReq.IdTaskType);
        }

        if (!await _studentsRepository.CreatorExists(taskReq))
        {
            throw new TeamMemberNotFound(taskReq.IdCreator);
        }

        if (!await _studentsRepository.AssignedToExists(taskReq))
        {
            throw new TeamMemberNotFound(taskReq.IdAssignedTo);
        }

        return await _studentsRepository.CreateTask(taskReq);
    }

    public async Task<TeamMemberAndTasks?> GetTeamMember(int idTeamMember)
    {
        //Business logic
        return await _studentsRepository.GetTeamMember(idTeamMember);
    }

    public async Task<int> UpdateTeamMember(int id, UpdateTeamMemberRequest teamMember)
    {
        //Business logic
        if (!await _studentsRepository.TeamMemberInTeamExists(id))
        {
            throw new TeamMemberNotFound(id);
        }
        return await _studentsRepository.UpdateTeamMember(id, teamMember);
    }

    public async Task<int> DeleteTeamMember(int idTeamMember)
    {
        //Business logic
        if (!await _studentsRepository.TeamMemberInTeamExists(idTeamMember))
        {
            throw new TeamMemberNotFound(idTeamMember);
        }

        if (await _studentsRepository.CreatorExists(new TaskRequest { IdCreator = idTeamMember }) ||
            await _studentsRepository.AssignedToExists(new TaskRequest { IdAssignedTo = idTeamMember }))
        {
            await _studentsRepository.DeleteTask(idTeamMember);
        }

        return await _studentsRepository.DeleteTeamMember(idTeamMember);
    }
}