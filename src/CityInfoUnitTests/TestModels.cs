using CityInfo.API.Models;
using Xunit;



namespace CityInfoUnitTests
{
    public class TestModels
    {
            
        [Fact]
        public void TestNoPoints()
        {
            CityDto city1 = new CityDto
            {
                Id = 1,
                Name = "Vancouver",
                Description = "Home"
            };

            Assert.Equal(0, city1.PointsOfInterest.Count);
        }

        [Fact]
        public void TestMultiplePoints()
        {

            PointOfInterestDto poi1 = new PointOfInterestDto
            {
                Id = 1,
                Name = "Downtown",
                Description = "Lots of food"
            };

            PointOfInterestDto poi2 = new PointOfInterestDto
            {
                Id = 2,
                Name = "Stanley Park",
                Description = "Lots to do"
            };

            PointOfInterestDto poi3 = new PointOfInterestDto
            {
                Id = 3,
                Name = "Science World",
                Description = "Lots of science"
            };

            CityDto city2 = new CityDto
            {
                Id = 2,
                Name = "Van",
                Description = "City of Vancouver",
                PointsOfInterest = { poi1,poi2,poi3 }
            };

            Assert.Equal(3, city2.PointsOfInterest.Count);
        }
    }
}

