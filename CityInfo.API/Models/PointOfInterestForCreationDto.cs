using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.Models
{
    // This is the DTO used to create a new point of interest.
    // We want to keep this separate from the DTO used to retrieve a point of interest from the database.
    public class PointOfInterestForCreationDto
    {
        [Required(ErrorMessage = "You should provide a name value.")]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Description { get; set; }
    }
}