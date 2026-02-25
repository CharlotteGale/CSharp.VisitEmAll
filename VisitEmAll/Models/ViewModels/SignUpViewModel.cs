using System.ComponentModel.DataAnnotations;

namespace VisitEmAll.ViewModels;

public class SignUpViewModel
{
    [Required(ErrorMessage = "Name is required")]
    [Display(Name = "Full Name")]
    public string Name { get; set; } = null!;
    
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
    public string Password { get; set; } = null!;

    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    [Compare("Password", ErrorMessage = "Passwords must match")]
    public string ConfirmPassword { get; set; } = null!;

    [Display(Name = "Home Town")]
    public string? HomeTown { get; set; }

    [Display(Name = "Profile Image URL")]
    public string? ProfileImg { get; set; }
}