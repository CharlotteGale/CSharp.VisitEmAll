using System.ComponentModel.DataAnnotations;

namespace VisitEmAll.Models;

public class Country
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(2)]
    public string Iso2 { get; set; } = string.Empty;

    [MaxLength(30)]
    public string? Continent { get; set; }
}