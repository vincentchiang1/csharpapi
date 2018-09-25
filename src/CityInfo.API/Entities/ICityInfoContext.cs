using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Entities
{
    public interface ICityInfoContext
    {
        DbSet<City> Cities { get; set; }
        DbSet<PointOfInterest> PointsOfInterest { get; set; }
        int SaveChanges();
    }
}
