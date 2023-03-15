namespace CityInfo.API.Models
{
    // This is the DTO used to create a new point of interest.
    // We want to keep this separate from the DTO used to retrieve a point of interest from the database.
    public class PointOfInterestForCreationDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}