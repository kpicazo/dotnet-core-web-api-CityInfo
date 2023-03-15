using CityInfo.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointsofinterest")]
    [ApiController]
    public class PointsOfInterestController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterest(int cityId)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            return Ok(city.PointsOfInterest);
        }

        [HttpGet("{pointofinterestid}", Name = "GetPointOfInterest")]
        public ActionResult<PointOfInterestDto> GetPointOfInterest(int cityId, int pointOfInterestId)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterest = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);

            if (pointOfInterest == null)
            {
                return NotFound();
            }

            return Ok(pointOfInterest);
        }

        [HttpPost]
        public ActionResult<PointOfInterestDto> CreatePointOfInterest(
            int cityId,  
            PointOfInterestForCreationDto pointOfInterest)
        {
            // First, check to see if user is trying to add a point of interest to a city that does not exist.
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null )
            {
                return NotFound();
            }

            // Calculate the ID of the new point of interest.
            // For now, just get the highest ID of all existing points of interest, and add 1 to it.
            // Note: iterating through all the cities and their points just to get an ID is not ideal for performance reasons,
            // and also this code does not take possible errors into account when multiple consumers try to get an ID at the same time.
            // We'll improve this later.
            var maxPointOfInterestId = CitiesDataStore.Current.Cities.SelectMany(c => c.PointsOfInterest).Max(p => p.Id);

            // Map PointOfInterestForCreationDto to PointOfInterestDto since that's the object type in the data store.
            // Note: this is not ideal as mapping can lead to errors.
            var finalPointOfInterest = new PointOfInterestDto()
            {
                Id = ++maxPointOfInterestId,
                Name = pointOfInterest.Name,
                Description = pointOfInterest.Description,
            };

            // Add point of interest to city.
            city.PointsOfInterest.Add(finalPointOfInterest);

            // Finally, return something.
            // The advised response for a POST is "201 Created". Can do this with helper method CreatedAtRoute.
            // This allows us to return a response with the location header which contains the URI where the 
            // newly created point of interest can be found.
            return CreatedAtRoute("GetPointOfInterest", 
                new
                {
                    // these are the values that the specified route template needs
                    cityId = cityId,
                    pointOfInterestId = finalPointOfInterest.Id
                },
                finalPointOfInterest
            );
        }
    }
}
