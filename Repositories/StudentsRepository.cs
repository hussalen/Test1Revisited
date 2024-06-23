using System.ComponentModel;
using System.Data.SqlClient;
using GakkoHorizontalSlice.Model;

namespace GakkoHorizontalSlice.Repositories;

public class StudentsRepository : IStudentsRepository
{
    private IConfiguration _configuration;

    public StudentsRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<IEnumerable<TeamMember>> GetTeamMembers()
    {
        using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();

        using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "SELECT * FROM TeamMember ORDER BY FirstName, LastName";

        var dr = await cmd.ExecuteReaderAsync();
        var teamMembers = new List<TeamMember>();
        while (await dr.ReadAsync())
        {
            var member = new TeamMember
            {
                IdTeamMember = (int)dr["IdTeamMember"],
                FirstName = dr["FirstName"].ToString() ?? throw new InvalidOperationException(),
                LastName = dr["LastName"].ToString() ?? throw new InvalidOperationException(),
                Email = dr["Email"].ToString() ?? throw new InvalidOperationException(),
            };
            teamMembers.Add(member);
        }

        return teamMembers;
    }

    public async Task<TeamMemberAndTasks> GetTeamMember(int idTeamMember)
    {
        //EXPERIMENTAL: DON"T UST MULTIPLE SQL QUERIES, JUST SEPARATE THEM INTO SEPARATE METHODS
        using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();

        using var cmd = new SqlCommand();
        cmd.Connection = con;

        string assignedTasksQuery =
            "SELECT Task.Name AS taskName, Task.Description, Task.Deadline, TeamMember.IdTeamMember, TeamMember.FirstName, TeamMember.LastName, TeamMember.Email, TaskType.Name AS taskType, Project.Name AS projectName FROM Task " +
            "INNER JOIN TeamMember ON Task.IdAssignedTo = TeamMember.IdTeamMember " +
            "INNER JOIN Project ON Task.IdProject = Project.IdProject " +
            "INNER JOIN TaskType ON Task.IdTaskType = TaskType.IdTaskType " +
            "WHERE Task.IdAssignedTo = @IdTeamMember " +
            "ORDER BY Task.Deadline DESC; ";

        string createdTasksQuery = "SELECT Task.Name AS taskName, Task.Description, Task.Deadline, " +
                                   "TeamMember.IdTeamMember, TeamMember.FirstName, TeamMember.LastName, TeamMember.Email, " +
                                   "TaskType.Name AS taskType, Project.Name AS projectName FROM Task " +
                                   "INNER JOIN TeamMember ON Task.IdCreator = TeamMember.IdTeamMember " +
                                   "INNER JOIN Project ON Task.IdProject = Project.IdProject " +
                                   "INNER JOIN TaskType ON Task.IdTaskType = TaskType.IdTaskType " +
                                   "WHERE Task.IdCreator = @IdTeamMember " +
                                   "ORDER BY Task.Deadline DESC; ";
        
        cmd.CommandText = "SELECT IdTeamMember, FirstName, LastName, Email FROM TeamMember WHERE IdTeamMember = @IdTeamMember;"+assignedTasksQuery+createdTasksQuery;
        cmd.Parameters.AddWithValue("@IdTeamMember", idTeamMember);
        var dataReader = await cmd.ExecuteReaderAsync();
        
        if (!dataReader.Read()) return null;
        
        var teamMember = new TeamMember
        {
            IdTeamMember = (int)dataReader["IdTeamMember"],
            FirstName = dataReader["FirstName"].ToString() ?? throw new InvalidOperationException(),
            LastName = dataReader["LastName"].ToString() ?? throw new InvalidOperationException(),
            Email = dataReader["Email"].ToString() ?? throw new InvalidOperationException(),
        };
        
        await dataReader.NextResultAsync();
        var tasksAssignedList = new List<object>();
        
        while (await dataReader.ReadAsync())
        {
            var task = new
            {
                Name = dataReader["taskName"].ToString(), Description = dataReader["Description"].ToString(),
                Deadline = dataReader["Deadline"].ToString(), ProjectName = dataReader["projectName"].ToString(),
                Type = dataReader["taskType"].ToString()
            };
            tasksAssignedList.Add(task);
        }
        
        await dataReader.NextResultAsync();
        
        var tasksCreatedList = new List<object>();
        while (await dataReader.ReadAsync())
        {
            var task = new
            {
                Name = dataReader["taskName"].ToString(), Description = dataReader["Description"].ToString(),
                Deadline = dataReader["Deadline"].ToString(), ProjectName = dataReader["projectName"].ToString(),
                Type = dataReader["taskType"].ToString()
            };
            tasksCreatedList.Add(task);
        }


        return new TeamMemberAndTasks {TeamMember = teamMember, AssignedToTasks = tasksAssignedList, CreatedTasks = tasksCreatedList};
    }

    public async Task<bool> IdProjectExist(TaskRequest taskReq)
    {
        using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();
        using var cmd = new SqlCommand();
        cmd.Connection = con;
        
        cmd.CommandText = "SELECT COUNT(1) FROM Project WHERE IdProject = @IdProject ";
        cmd.Parameters.AddWithValue("@IdProject", taskReq.IdProject);
        var count = await cmd.ExecuteScalarAsync() ?? throw new InvalidOperationException(); 
        
        await con.CloseAsync();
        return (int)count > 0;
    }

    public async Task<bool> IdTaskTypeExists(TaskRequest taskReq)
    {
        using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();
        using var cmd = new SqlCommand();
        cmd.Connection = con;
        
        cmd.CommandText = "SELECT COUNT(1) FROM TaskType WHERE IdTaskType = @IdTaskType ";
        cmd.Parameters.AddWithValue("@IdTaskType", taskReq.IdTaskType);

        var count = await cmd.ExecuteScalarAsync() ?? throw new InvalidOperationException();
        await con.CloseAsync();
        
        return (int) count > 0;
    }

    public async Task<bool> AssignedToExists(TaskRequest taskReq)
    {
        using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();
        using var cmd = new SqlCommand();
        cmd.Connection = con;
        
        cmd.CommandText = "SELECT COUNT(1) FROM Task WHERE IdAssignedTo = @IdAssignedTo ";
        cmd.Parameters.AddWithValue("@IdAssignedTo", taskReq.IdAssignedTo);
        
        var count = await cmd.ExecuteScalarAsync() ?? throw new InvalidOperationException();
        await con.CloseAsync();
        return (int)count > 0;
    }

    public async Task<bool> CreatorExists(TaskRequest taskReq)
    {
        using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();
        using var cmd = new SqlCommand();
        cmd.Connection = con;
        
        cmd.CommandText = "SELECT COUNT(1) FROM Task WHERE IdCreator = @IdCreator ";
        cmd.Parameters.AddWithValue("@IdCreator", taskReq.IdCreator);
        
        var count = (int)cmd.ExecuteScalar();
        await con.CloseAsync();
        return count > 0;
    }

    public async Task<int> CreateTask(TaskRequest taskReq)
    {
        using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();

        using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText =
            "INSERT INTO Task(Name, Description, Deadline, IdProject, IdTaskType, IdAssignedTo, IdCreator) OUTPUT INSERTED.IdTask " +
            "VALUES(@Name, @Description, @Deadline, @IdProject, @IdTaskType, @IdAssignedTo, @IdCreator)";
        cmd.Parameters.AddWithValue("@Name", taskReq.Name);
        cmd.Parameters.AddWithValue("@Description", taskReq.Description);
        cmd.Parameters.AddWithValue("@Deadline", taskReq.Deadline);
        cmd.Parameters.AddWithValue("@IdProject", taskReq.IdProject);
        cmd.Parameters.AddWithValue("@IdTaskType", taskReq.IdTaskType);
        cmd.Parameters.AddWithValue("@IdAssignedTo", taskReq.IdAssignedTo);
        cmd.Parameters.AddWithValue("@IdCreator", taskReq.IdCreator);

        var dataReader = cmd.ExecuteScalar();
        return (int)dataReader;
    }

    public async Task<bool> TeamMemberInTeamExists(int id)
    {
        using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();
        using var cmd = new SqlCommand();
        cmd.Connection = con;
        
        cmd.CommandText = "SELECT COUNT(1) FROM TeamMember WHERE IdTeamMember = @IdTeamMember ";
        cmd.Parameters.AddWithValue("@IdTeamMember", id);
        
        var count = (int)cmd.ExecuteScalar();
        await con.CloseAsync();
        return count > 0;
    }

    public async Task DeleteTask(int idTeamMember)
    {
        using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();

        using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "DELETE FROM Task WHERE IdCreator = @IdCreator OR IdAssignedTo = @IdAssignedTo";
        cmd.Parameters.AddWithValue("@IdCreator", idTeamMember);
        cmd.Parameters.AddWithValue("@IdAssignedTo", idTeamMember);

        cmd.ExecuteNonQuery();
        await con.CloseAsync();
    }

    public async Task<int> DeleteTeamMember(int id)
    {
        using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();

        using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "DELETE FROM TeamMember WHERE IdTeamMember = @IdTeamMember";
        cmd.Parameters.AddWithValue("@IdTeamMember", id);

        cmd.ExecuteNonQuery();
        await con.CloseAsync();
        return id;
    }

    public async Task<int> UpdateTeamMember(int id, UpdateTeamMemberRequest teamMember)
    {
        using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();

        using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText =
            "UPDATE TeamMember SET FirstName=@FirstName, LastName=@LastName, Email=@Email WHERE IdTeamMember = @IdTeamMember";
        cmd.Parameters.AddWithValue("@IdTeamMember", id);
        cmd.Parameters.AddWithValue("@FirstName", teamMember.FirstName);
        cmd.Parameters.AddWithValue("@LastName", teamMember.LastName);
        cmd.Parameters.AddWithValue("@Email", teamMember.Email);

        var affectedCount = cmd.ExecuteNonQuery();
        await con.CloseAsync();
        return affectedCount;
    }
}