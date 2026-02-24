using System.ComponentModel.DataAnnotations;
namespace VisitEmAll.Models;

public class User
{
    [Key]
    public int Id { get; set; }
    [Required]
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public string? HomeTown { get; set; } = null!;
    public string? ProfileImg {get; set; } = null!;

}