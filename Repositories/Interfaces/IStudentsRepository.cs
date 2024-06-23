using GakkoHorizontalSlice.Model;

namespace GakkoHorizontalSlice.Repositories;

public interface IStudentsRepository
{
    Task<IEnumerable<TeamMember>> GetTeamMembers();
    Task<int> CreateTask(TaskRequest taskReq);
    Task<bool> IdProjectExist(TaskRequest taskReq);
    Task<bool> IdTaskTypeExists(TaskRequest taskReq);
    Task<bool> AssignedToExists(TaskRequest taskReq);
    Task<bool> CreatorExists(TaskRequest taskReq);
    Task<bool> TeamMemberInTeamExists(int id);
    Task<TeamMemberAndTasks> GetTeamMember(int idTeamMember);
    Task<int> UpdateTeamMember(int id, UpdateTeamMemberRequest teamMember);
    Task<int> DeleteTeamMember(int idStudent);
    Task DeleteTask(int idTeamMember);
}