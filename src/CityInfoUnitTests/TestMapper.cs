using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using Xunit;

namespace CityInfoUnitTests
{
    public class TestMapper
    {
        public TestMapper()
        {
            Mapper.Reset();
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<CityInfo.API.Entities.City, CityInfo.API.Models.CityWithoutPointsOfInterestDto>();
                cfg.CreateMap<CityInfo.API.Entities.City, CityInfo.API.Models.CityDto>();
                cfg.CreateMap<CityInfo.API.Entities.PointOfInterest, CityInfo.API.Models.PointOfInterestDto>();
                cfg.CreateMap<CityInfo.API.Models.PointOfInterestForCreationDto, CityInfo.API.Entities.PointOfInterest>();
                cfg.CreateMap<CityInfo.API.Models.PointOfInterestForUpdateDto, CityInfo.API.Entities.PointOfInterest>();
                cfg.CreateMap<CityInfo.API.Entities.PointOfInterest, CityInfo.API.Models.PointOfInterestForUpdateDto>();
            });
        }

        readonly City city = new City
        {
            Id = 1,
            Name = "Richmond",
            Description = "Small",
            PointsOfInterest = new List<PointOfInterest>()
            {
                new PointOfInterest
            {
                Id = 1,
                Name = "Cocoru",
                Description = "Chicken",
                CityId = 1
            },
                new PointOfInterest
            {
                Id = 2,
                Name = "Midam",
                Description = "Korean",
                CityId = 1
            }
            }
        };

        [Fact]
        // City Entity to CityWithoutPointsOfInterestDto
        public void TestFromCityToCityWithoutPointsDto()
        {    
            var result = Mapper.Map<CityWithoutPointsOfInterestDto>(city);

            Assert.Equal(1, result.Id);
            Assert.Equal("Richmond", result.Name);
            Assert.Equal("Small", result.Description);            
        }

        [Fact]
        // City Entity to CityDto
        public void TestFromCityToCityDto()
        {
            var result = Mapper.Map<CityDto>(city);

            Assert.Equal(1, result.Id);
            Assert.Equal("Richmond", result.Name);
            Assert.Equal("Small", result.Description);
            Assert.Equal(2, result.PointsOfInterest.Count);
            Assert.Equal(1, result.PointsOfInterest.ElementAt(0).Id);
            Assert.Equal("Cocoru", result.PointsOfInterest.ElementAt(0).Name);
            Assert.Equal("Chicken", result.PointsOfInterest.ElementAt(0).Description);
            Assert.Equal(2, result.PointsOfInterest.ElementAt(1).Id);
            Assert.Equal("Midam", result.PointsOfInterest.ElementAt(1).Name);
            Assert.Equal("Korean", result.PointsOfInterest.ElementAt(1).Description);
        }

        [Fact]
        // Point Entity to PointDto
        public void TestFromPointToPointDto()
        {
            var result = Mapper.Map<PointOfInterestDto>(city.PointsOfInterest.ElementAt(0));
            Assert.Equal(1, result.Id);
            Assert.Equal("Cocoru", result.Name);
            Assert.Equal("Chicken", result.Description);
        }

        [Fact]
        // PointForCreationDto to Point Entity
        public void TestFromPointForCreationDtoToPoint()
        {
            PointOfInterestForCreationDto point = new PointOfInterestForCreationDto
            {
                Name = "Steveston",
                Description = "Nice"
            };

            var result = Mapper.Map<PointOfInterest>(point);

            Assert.Equal("Steveston", result.Name);
            Assert.Equal("Nice", result.Description);
        }

        [Fact]
        // PointForUpdateDto to Point Entity
        public void TestFromPointForUpdateDtoToPoint()
        {
            PointOfInterestForUpdateDto point = new PointOfInterestForUpdateDto
            {
                Name = "Steveston",
                Description = "Nice"
            };

            var result = Mapper.Map<PointOfInterest>(point);

            Assert.Equal("Steveston", result.Name);
            Assert.Equal("Nice", result.Description);
        }

        [Fact]
        // Point Entity to PointForUpdateDto
        public void TestFromPointToPointForUpdate()
        {
            var result = Mapper.Map<PointOfInterestForUpdateDto>(city.PointsOfInterest.ElementAt(0));

            Assert.Equal("Cocoru", result.Name);
            Assert.Equal("Chicken", result.Description);
        }
    }
}
