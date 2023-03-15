using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.DbContexts
{
    public class CityInfoContext : DbContext
    {
        // Deriving the class from DbContext ensures that the properties here
        // are initialized as non-null after leaving the base constructor.
        // Use null-forgiving operator to suppress null warnings.
        public DbSet<City> Cities { get; set; } = null!;
        public DbSet<PointOfInterest> PointsOfInterest { get; set; } = null!;

        // We need to configure the DbContext and provide it connection strings 
        // and other information about the database we want to connect to.

        // one way:
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlite("connectionstring");
        //    SQLitePCL.raw.SetProvider();
        //    base.OnConfiguring(optionsBuilder);
        //}

        // another way - through constructor
        public CityInfoContext(DbContextOptions<CityInfoContext> options)
            : base(options)
        {
            
        }

    }
}
