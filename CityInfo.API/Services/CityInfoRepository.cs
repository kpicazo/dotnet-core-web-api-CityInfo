using CityInfo.API.DbContexts;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services
{
    // This is where we provide persistence logic.
    // We might need only CityInfoContext, or a mix of EF Core methods and services, etc.
    public class CityInfoRepository : ICityInfoRepository
    {
        private readonly CityInfoContext _context;

        public CityInfoRepository(CityInfoContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<City>> GetCitiesAsync()
        {
            return await _context.Cities.OrderBy(c => c.Name).ToListAsync();
        }

        public async Task<IEnumerable<City>> GetCitiesAsync(string? name, string? searchQuery)
        {
            // if no name or search query was passed in, just get all cities
            if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(searchQuery))
            {
                return await GetCitiesAsync();
            }

            // we need to cover all the following scenarios:
            // 1. apply only the name filter
            // 2. apply only the search query
            // 3. apply both the name filter and search query

            // We want to build the query statement by statement and only execute what we need.
            // First, initialize collection variable as IQueryable.
            // Note: This query variable stores COMMANDS and not results.
            // Execution of this query is deferred until the query is iterated over.
            var collection = _context.Cities as IQueryable<City>;

            // apply name filter
            if (!string.IsNullOrWhiteSpace(name))
            {
                name = name.Trim();
                collection = collection.Where(c => c.Name == name);
            }

            // apply searchQuery
            if (!string.IsNullOrEmpty(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                collection = collection.Where(a => a.Name.Contains(searchQuery) 
                                || (a.Description != null && a.Description.Contains(searchQuery)));
            }

            // This is where the query is executed as we are calling ToListAsync() which iterates over the collection
            return await collection.OrderBy(c => c.Name).ToListAsync();
        }

        public async Task<City?> GetCityAsync(int cityId, bool includePointsOfInterest)
        {
            if (includePointsOfInterest)
            {
                return await _context.Cities.Include(c => c.PointsOfInterest)
                    .Where(c => c.Id == cityId).FirstOrDefaultAsync();
            }

            return await _context.Cities
                .Where(c => c.Id == cityId).FirstOrDefaultAsync();
        }

        public async Task<bool> CityExistsAsync(int cityId)
        {
            return await _context.Cities.AnyAsync(c => c.Id == cityId);
        }

        public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId)
        {
            return await _context.PointsOfInterest
                .Where(p => p.CityId == cityId)
                .ToListAsync();
        }

        public async Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId)
        {
            return await _context.PointsOfInterest
                .Where(p => p.CityId == cityId && p.Id == pointOfInterestId)
                .FirstOrDefaultAsync();
        }

        public async Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest)
        {
            var city = await GetCityAsync(cityId, false);
            if (city != null)
            {
                // this only adds to object context (i.e. in-memory representation of the objects); it does not persist to the database yet.
                // we need to call a save method on the context in order to save to database.
                city.PointsOfInterest.Add(pointOfInterest);
            }
        }

        // This is an in-memory operation so it does not need to be asynchronous.
        public void DeletePointOfInterest(PointOfInterest pointOfInterest)
        {
            _context.PointsOfInterest.Remove(pointOfInterest);
        }

        // This will save data to the actual database
        public async Task<bool> SaveChangesAsync()
        {
            // SaveChangesAsync() will return the amount of entities that have been changed
            return (await _context.SaveChangesAsync() >= 0);
        }
    }
}
