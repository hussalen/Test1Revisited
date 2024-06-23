using System.ComponentModel.DataAnnotations;
namespace GakkoHorizontalSlice.Model;

public class TheTask
{
    [Required]
    public int IdTask { get; set; }
    [Required]
    [MaxLength(200)]
    public string Name { get; set; }
    [Required]
    [MaxLength(200)]
    public string Description { get; set; }
    [Required]
    public DateTime Deadline { get; set; }
    [Required]
    public int IdProject { get; set; }
    [Required]
    public int IdTaskType { get; set; }
    [Required]
    public int IdAssignedTo { get; set; }
    [Required]
    public int IdCreator { get; set; }
}