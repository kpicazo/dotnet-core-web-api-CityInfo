using CityInfo.API.Models;
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
        [HttpGet]
        public ActionResult<IEnumerable<CityDto>> GetCities()
        {
            return Ok(CitiesDataStore.Current.Cities);
        }

        [HttpGet("{id}")] // curly brackets used for parameters in routing templates
        public ActionResult<CityDto> GetCity(int id)
        {
            // find city
            var cityToReturn = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == id);

            if (cityToReturn == null)
            {
                return NotFound();
            }

            return Ok(cityToReturn);
        }
    }
}
