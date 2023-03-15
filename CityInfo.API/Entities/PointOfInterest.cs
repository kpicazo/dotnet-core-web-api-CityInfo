using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.Entities
{
    public class PointOfInterest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }

        // Navigation property creates a relationship between entities.
        // A property is considered a navigation property if the type it points to cannot be mapped
        // as a scalar type by the current database provider.
        [ForeignKey("CityId")]
        public City? City { get; set; }

        // For clarity, add the foreign key explicitly.
        // Alternatively, can add [ForeignKey("CityId")] annotation on the City property
        public int CityId { get; set; }

        public PointOfInterest(string name)
        {
            Name = name;
        }
    }
}
