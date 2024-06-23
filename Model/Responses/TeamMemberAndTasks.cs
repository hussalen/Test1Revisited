using System.ComponentModel.DataAnnotations;

namespace GakkoHorizontalSlice.Model;

public class TeamMemberAndTasks
{
    public TeamMember TeamMember { get; set; }
    public List<object> AssignedToTasks { get; set; }
    public List<object> CreatedTasks { get; set; }
}