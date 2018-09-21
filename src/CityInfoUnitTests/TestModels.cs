using System.Linq;
using CityInfo.API.Models;
using Xunit;

namespace CityInfoUnitTests
{
    public class TestModels
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

        [Fact]
        [Trait("City", "CityDto.City")]
        // CityDto with no points of interest
        public void TestCityDtoNoPoints()
        {
            CityDto city1 = new CityDto
            {
                Id = 1,
                Name = "Vancouver",
                Description = "Big"
            };

            Assert.Equal(1, city1.Id);
            Assert.Equal("Vancouver", city1.Name);
            Assert.Equal("Big", city1.Description);
            Assert.Equal(0, city1.PointsOfInterest.Count);
        }

        [Fact]
        [Trait("City", "CityDto.City")]
        // CityDto with multiple points of interest
        public void TestCityDtoMultiplePoints()
        {
            CityDto city2 = new CityDto
            {
                Id = 2,
                Name = "Van",
                Description = "City of Vancouver",
                PointsOfInterest = { poi1, poi2 }
            };

            Assert.Equal(2, city2.Id);
            Assert.Equal("Van", city2.Name);
            Assert.Equal("City of Vancouver", city2.Description);
            Assert.Equal(2, city2.PointsOfInterest.Count);
            Assert.Equal(1, city2.PointsOfInterest.ElementAt(0).Id);
            Assert.Equal("Downtown", city2.PointsOfInterest.ElementAt(0).Name);
            Assert.Equal("Lots of food", city2.PointsOfInterest.ElementAt(0).Description);
            Assert.Equal(2, city2.PointsOfInterest.ElementAt(1).Id);
            Assert.Equal("Stanley Park", city2.PointsOfInterest.ElementAt(1).Name);
            Assert.Equal("Lots to do", city2.PointsOfInterest.ElementAt(1).Description);
        }

        [Fact]
        [Trait("City", "CityDto.City")]
        // CityDto with no id, name, description, or points of interest
        public void TestCityDtoEmpty()
        {
            CityDto city = new CityDto { };

            Assert.Equal(0, city.Id);
            Assert.Null(city.Name);
            Assert.Null(city.Description);
            Assert.Empty(city.PointsOfInterest);
        }

        [Fact]
        [Trait("City", "CityWithoutPointsOfInterestDto.City")]
        // CityWithoutPointsOfInterestDto 
        public void TestCityWithoutPointsDto()
        {
            CityWithoutPointsOfInterestDto city3 = new CityWithoutPointsOfInterestDto
            {
                Id = 3,
                Name = "Richmond",
                Description = "Small"
            };

            Assert.Equal(3, city3.Id);
            Assert.Equal("Richmond", city3.Name);
            Assert.Equal("Small", city3.Description);
        }

        [Fact]
        [Trait("City", "CityWithoutPointsOfInterestDto.City")]
        // CityWithoutPointsOfInterestDto with no id, name, or description
        public void TestCityWIthoutPointsDtoEmpty()
        {
            CityWithoutPointsOfInterestDto city = new CityWithoutPointsOfInterestDto { };

            Assert.Equal(0, city.Id);
            Assert.Null(city.Name);
            Assert.Null(city.Description);
        }

        [Fact]
        [Trait("Point", "PointOfInterestDto.City")]
        // PointOfInterestDto
        public void TestPointDto()
        {
            Assert.Equal(1, poi1.Id);
            Assert.Equal("Downtown", poi1.Name);
            Assert.Equal("Lots of food", poi1.Description);
        }

        [Fact]
        [Trait("Point", "PointOfInterestDto.City")]
        // PointOfInterestDto with no id, name, or description
        public void TestPointDtoEmpty()
        {
            PointOfInterestDto point = new PointOfInterestDto { };

            Assert.Equal(0, point.Id);
            Assert.Null(point.Name);
            Assert.Null(point.Description);
        }

        [Fact]
        [Trait("Point", "PointOfInterestForCreationDto.City")]
        // PointOfInterestForCreationDto
        public void TestPointForCreation()
        {
            PointOfInterestForCreationDto pointC = new PointOfInterestForCreationDto
            {
                Name = "Aquarium",
                Description = "Fish"
            };

            Assert.Equal("Aquarium", pointC.Name);
            Assert.Equal("Fish", pointC.Description);
        }

        [Fact]
        [Trait("Point", "PointOfInterestForCreationDto.City")]
        // PointOfInterestForCreationDto without Description
        public void TestPointForCreationNoDesc()
        {
            PointOfInterestForCreationDto pointC = new PointOfInterestForCreationDto
            {
                Name = "Aquarium"
            };

            Assert.Equal("Aquarium", pointC.Name);
            Assert.Null(pointC.Description);
        }

        [Fact]
        [Trait("Point", "PointOfInterestForUpdateDto.City")]
        // PointOfInterestForUpdateDto
        public void TestPointForUpdate()
        {
            PointOfInterestForUpdateDto pointU = new PointOfInterestForUpdateDto
            {
                Name = "Aquarium",
                Description = "Fish"
            };
            
            Assert.Equal("Aquarium", pointU.Name);
            Assert.Equal("Fish", pointU.Description);
        }

        [Fact]
        [Trait("Point", "PointOfInterestForUpdateDto.City")]
        // PointOfInterestForUpdateDto without Description
        public void TestPointForUpdateNoDesc()
        {
            PointOfInterestForUpdateDto pointU = new PointOfInterestForUpdateDto
            {
                Name = "Aquarium"
            };

            Assert.Equal("Aquarium", pointU.Name);
            Assert.Null(pointU.Description);
        }
    }
}

