using System.ComponentModel.DataAnnotations;

namespace VisitEmAll.ViewModels;

public class CreateHolidayViewModel
{
    public int? Id { get; set; }
    [Required, MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Location { get; set; }
    public int? CountryId { get; set; }
    public string? CountryName { get; set; }

    [Required, DataType(DataType.Date)]
    public DateOnly? StartDate { get; set; }

    [DataType(DataType.Date)]
    public DateOnly? EndDate { get; set; }

    public decimal? TotalCost { get; set; }

    public string? ThumbnailUrl { get; set; }

    public List<ActivityInput> Activities { get; set; } = new();

    public class ActivityInput
    {
        [MaxLength(150)]
        public string? Name { get; set; }
    }
    public IFormFile? HeroImageFile { get; set; }
    public string? ExistingHeroImage { get; set; }
}