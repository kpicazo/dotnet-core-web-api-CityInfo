using AutoMapper;

namespace CityInfo.API.Profiles
{
    public class CityProfile : Profile
    {
        
        public CityProfile()
        {
            // We want to create a map from the City entity to the CityWithoutPointsOfInterestDTO.
            // By convention, AutoMapper will map property names on the source object to the same
            // property names on the destination object. If the property doesn't exist, it will be ignored.
            // Occasionally we might have to provide our own property mappings but for a lot of objects,
            // this is sufficient.
            CreateMap<Entities.City, Models.CityWithoutPointsOfInterestDto>();
            CreateMap<Entities.City, Models.CityDto>();
        }
    }
}
