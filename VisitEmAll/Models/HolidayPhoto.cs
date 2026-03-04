namespace VisitEmAll.Models
{
    public class HolidayPhoto
    {
        public int Id { get; set; }

        // FK to Holiday (not wired yet)
        public int HolidayId { get; set; }

        // URL or file path to the uploaded image
        public string ImageUrl { get; set; } = string.Empty;

        // Short caption under the photo
        public string Caption { get; set; } = string.Empty;

        // Optional: ordering inside the carousel
        public int SortOrder { get; set; }
    }
}
