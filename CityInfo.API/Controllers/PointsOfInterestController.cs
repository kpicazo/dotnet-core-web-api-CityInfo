using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointsofinterest")]
    [ApiController]
    public class PointsOfInterestController : ControllerBase
    {
        private readonly ILogger<PointsOfInterestController> _logger;
        private readonly IMailService _mailService;
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger,
            IMailService mailService,
            ICityInfoRepository cityInfoRepository,
            IMapper mapper)
        {
            // Good idea to add null check for any type that is injected 
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

            // when constructor injection is not feasible, can request a service from the container directly
            // e.g. HttpContext.RequestServices.GetService
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterest(int cityId)
        {
            // First, check if city exists or not.
            // We want to return NotFound if it does not, to differentiate returning an empty list
            // if the city does exist, but no points of interest exist.
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation(
                    $"City with id {cityId} wasn't found when accessing points of interest.");
                return NotFound();
            }

            var pointsOfInterestForCity = await _cityInfoRepository.GetPointsOfInterestForCityAsync(cityId);

            return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterestForCity));
        }

        [HttpGet("{pointofinterestid}", Name = "GetPointOfInterest")]
        public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterest(int cityId, int pointOfInterestId)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation(
                    $"City with id {cityId} wasn't found when accessing points of interest.");
                return NotFound();
            }

            var pointOfInterest = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);

            if (pointOfInterest == null)
            {
                _logger.LogInformation(
                    $"Point of interest with id {pointOfInterestId} wasn't found when accessing points of interest.");
                return NotFound();
            }

            return Ok(_mapper.Map<PointOfInterestDto>(pointOfInterest));
        }

        //[HttpPost]
        //public ActionResult<PointOfInterestDto> CreatePointOfInterest(
        //    int cityId,  
        //    PointOfInterestForCreationDto pointOfInterest)
        //{
        //    // First, check to see if user is trying to add a point of interest to a city that does not exist.
        //    var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
        //    if (city == null )
        //    {
        //        return NotFound();
        //    }

        //    // Calculate the ID of the new point of interest.
        //    // For now, just get the highest ID of all existing points of interest, and add 1 to it.
        //    // Note: iterating through all the cities and their points just to get an ID is not ideal for performance reasons,
        //    // and also this code does not take possible errors into account when multiple consumers try to get an ID at the same time.
        //    // We'll improve this later.
        //    var maxPointOfInterestId = _citiesDataStore.Cities.SelectMany(c => c.PointsOfInterest).Max(p => p.Id);

        //    // Map PointOfInterestForCreationDto to PointOfInterestDto since that's the object type in the data store.
        //    // Note: this is not ideal as mapping can lead to errors.
        //    var finalPointOfInterest = new PointOfInterestDto()
        //    {
        //        Id = ++maxPointOfInterestId,
        //        Name = pointOfInterest.Name,
        //        Description = pointOfInterest.Description,
        //    };

        //    // Add point of interest to city.
        //    city.PointsOfInterest.Add(finalPointOfInterest);

        //    // Finally, return something.
        //    // The advised response for a POST is "201 Created". Can do this with helper method CreatedAtRoute.
        //    // This allows us to return a response with the location header which contains the URI where the 
        //    // newly created point of interest can be found.
        //    return CreatedAtRoute("GetPointOfInterest", 
        //        new
        //        {
        //            // these are the values that the specified route template needs
        //            cityId = cityId,
        //            pointOfInterestId = finalPointOfInterest.Id
        //        },
        //        finalPointOfInterest
        //    );
        //}

        //[HttpPut("{pointofinterestid}")]
        //public ActionResult UpdatePointOfInterest(int cityId, int pointOfInterestId,
        //    PointOfInterestForUpdateDto pointOfInterest)
        //{
        //    // find city
        //    var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
        //    if (city == null)
        //    {
        //        return NotFound();
        //    }

        //    // find point of interest
        //    var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(c => c.Id == pointOfInterestId);
        //    if (pointOfInterestFromStore == null)
        //    {
        //        return NotFound();
        //    }

        //    pointOfInterestFromStore.Name = pointOfInterest.Name;
        //    pointOfInterestFromStore.Description = pointOfInterest.Description;

        //    return NoContent();
        //}

        //[HttpPatch("{pointofinterestid}")]
        //public ActionResult PartiallyUpdatePointOfInterest(
        //    int cityId, int pointOfInterestId,
        //    JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
        //{
        //    // find city
        //    var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
        //    if (city == null)
        //    {
        //        return NotFound();
        //    }

        //    // find point of interest
        //    var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(c => c.Id == pointOfInterestId);
        //    if (pointOfInterestFromStore == null)
        //    {
        //        return NotFound();
        //    }

        //    // Map the point of interest we got from the data store above to 
        //    // match PointOfInterestForUpdateDto which is the object on which
        //    // we'll apply the patch document.
        //    var pointOfInterestToPatch = 
        //        new PointOfInterestForUpdateDto()
        //        {
        //            Name = pointOfInterestFromStore.Name,
        //            Description = pointOfInterestFromStore.Description
        //        };

        //    // Pass in ModelState to check for potential errors in patch document creation
        //    // e.g. if we created the patch document with the wrong model
        //    patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    // We still need to validate the model again after the patch has been applied
        //    if (!TryValidateModel(pointOfInterestToPatch))
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    // Finally, update the point of interest and return 
        //    pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
        //    pointOfInterestFromStore.Description = pointOfInterestToPatch.Description;
        //    return NoContent();
        //}

        //[HttpDelete("{pointOfInterestId}")]
        //public ActionResult DeletePointOfInterest(int cityId, int pointOfInterestId)
        //{
        //    // find city
        //    var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
        //    if (city == null)
        //    {
        //        return NotFound();
        //    }

        //    // find point of interest
        //    var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(c => c.Id == pointOfInterestId);
        //    if (pointOfInterestFromStore == null)
        //    {
        //        return NotFound();
        //    }

        //    city.PointsOfInterest.Remove(pointOfInterestFromStore);
        //    _mailService.Send("Point of interest deleted.",
        //        $"Point of interest {pointOfInterestFromStore.Name} with id {pointOfInterestFromStore.Id} was deleted.");
        //    return NoContent();
        //}
    }
}
