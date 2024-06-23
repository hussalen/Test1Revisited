
using System.ComponentModel.DataAnnotations;
namespace GakkoHorizontalSlice.Model;

public class UpdateTeamMemberRequest
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; }
    [Required]
    [MaxLength(100)]
    public string LastName { get; set; }
    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public string Email { get; set; }
}