using GakkoHorizontalSlice.Model;

namespace GakkoHorizontalSlice.Services;

public interface ITasksService
{
    Task<IEnumerable<TeamMember>> GetTeamMembers();
    Task<int> CreateTask(TaskRequest taskReq);
    Task<TeamMemberAndTasks?> GetTeamMember(int idTeamMember);
    Task<int> UpdateTeamMember(int id, UpdateTeamMemberRequest teamMember);
    Task<int> DeleteTeamMember(int idTeamMember);
}