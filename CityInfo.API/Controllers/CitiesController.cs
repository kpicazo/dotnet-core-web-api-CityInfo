using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    // ApiController attribute configures the controller with features and behavior
    // aimed at improving the development experience when building APIs.
    // e.g. requiring a certain type of routing, automatically returning a 400 bad request on bad input,
    // returning problem details on errors, etc.
    [ApiController]
    //[Route("api/[controller]")] // [controller] refers to the controller's name; in this case "Cities"
    [Route("api/cities")]
    public class CitiesController : ControllerBase
    {
        private readonly ICityInfoRepository _cityInfoRepository;

        public CitiesController(ICityInfoRepository cityInfoRepository)
        {
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDto>>> GetCities()
        {
            // This returns an IEnumerable of cities without points of interest
            // so we need to create a new DTO for this and map the repository results to
            // a list of this new DTO.
            var cityEntities = await _cityInfoRepository.GetCitiesAsync();
            var results = new List<CityWithoutPointsOfInterestDto>();
            foreach (var cityEntity in cityEntities)
            {
                results.Add(new CityWithoutPointsOfInterestDto()
                {
                    Id = cityEntity.Id,
                    Description = cityEntity.Description,
                    Name = cityEntity.Name
                });
            }

            //return Ok(_citiesDataStore.Cities);
            return Ok(results);
        }

        [HttpGet("{id}")] // curly brackets used for parameters in routing templates
        public ActionResult<CityDto> GetCity(int id)
        {
            // find city
            //var cityToReturn = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == id);

            //if (cityToReturn == null)
            //{
            //    return NotFound();
            //}

            //return Ok(cityToReturn);

            return Ok();
        }
    }
}
