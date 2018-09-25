using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CityInfo.API;
using CityInfo.API.Controllers;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CityInfoUnitTests
{
    public class TestControllers
    {
        public TestControllers()
        {
            Mapper.Reset();
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<City, CityWithoutPointsOfInterestDto>();
                cfg.CreateMap<City, CityDto>();
                cfg.CreateMap<PointOfInterest, PointOfInterestDto>();
                cfg.CreateMap<PointOfInterestForCreationDto, PointOfInterest>();
                cfg.CreateMap<PointOfInterestForUpdateDto, PointOfInterest>();
                cfg.CreateMap<PointOfInterest, PointOfInterestForUpdateDto>();
            });
        }

        readonly PointOfInterest poi1 = new PointOfInterest
        {
            Id = 1,
            Name = "Downtown",
            Description = "Lots of food",
            CityId = 1
        };
        readonly PointOfInterest poi2 = new PointOfInterest
        {
            Id = 2,
            Name = "Stanley Park",
            Description = "Lots to do",
            CityId = 1
        };
        readonly PointOfInterest poi3 = new PointOfInterest
        {
            Id = 3,
            Name = "Science World",
            Description = "Lots of science",
            CityId = 2
        };
        readonly PointOfInterest poi4 = new PointOfInterest
        {
            Id = 4,
            Name = "UBC",
            Description = "School",
            CityId = 2
        };

        // Helper function for mock database of any entities
        private void MockSetupEntity<T>(Mock<DbSet<T>> mock, IQueryable<T> entities) where T : class
        {
            mock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(entities.Provider);
            mock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(entities.Expression);
            mock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(entities.ElementType);
        }

        // Helper function for creating context
        private ICityInfoContext CreateCityInfoContext(IEnumerable<City> cities, IEnumerable<PointOfInterest> points)
        {
            var mockSet = new Mock<DbSet<City>>();
            MockSetupEntity<City>(mockSet, cities.AsQueryable());

            var mockSetP = new Mock<DbSet<PointOfInterest>>();
            MockSetupEntity<PointOfInterest>(mockSetP, points.AsQueryable());

            var mockContext = new Mock<ICityInfoContext>();
            mockContext.Setup(m => m.Cities).Returns(mockSet.Object);
            mockContext.Setup(m => m.PointsOfInterest).Returns(mockSetP.Object);

            return mockContext.Object;
        }

        [Fact]
        [Trait("Category", "City.Controllers")]
        // CitiesController GetCities when empty
        public void TestCitiesControlGetCitiesEmpty()
        {
            // Arrange
            var cities = new List<City> { }.AsQueryable();

            var points = new List<PointOfInterest> { }.AsQueryable();

            var mockContext = CreateCityInfoContext(cities, points);

            ICityInfoRepository repo = new CityInfoRepository(mockContext);
            IValidator validator = new Validator();

            CitiesController controller = new CitiesController(repo, validator);

            // Act
            var result = controller.GetCities();
            var okResult = result as OkObjectResult;

            // Assert
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        [Trait("Category", "City.Controllers")]
        // CitiesController GetCities non-empty
        public void TestCitiesControlGetCities()
        {
            // Arrange
            var cities = new List<City>
            {
                new City{ Id = 1, Name = "Richmond", Description = "Small" },
                new City{ Id = 2, Name = "Vancouver", Description = "Big" }
            }.AsQueryable();

            var points = new List<PointOfInterest> { poi1, poi2, poi3, poi4 }.AsQueryable();

            var mockContext = CreateCityInfoContext(cities, points);

            ICityInfoRepository repo = new CityInfoRepository(mockContext);
            IValidator validator = new Validator();

            CitiesController controller = new CitiesController(repo, validator);

            // Act
            var result = controller.GetCities();
            var okResult = result as OkObjectResult;

            // Assert
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        [Trait("Category", "City.Controllers")]
        // CitiesController GetCity empty
        public void TestCitiesControlGetCityEmpty()
        {
            // Arrange
            var cities = new List<City> { }.AsQueryable();

            var points = new List<PointOfInterest> { }.AsQueryable();

            var mockContext = CreateCityInfoContext(cities, points);

            ICityInfoRepository repo = new CityInfoRepository(mockContext);
            IValidator validator = new Validator();

            CitiesController controller = new CitiesController(repo, validator);

            // Act
            var result = controller.GetCity(1);
            var notFoundResult = result as NotFoundResult;

            // Assert
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        [Trait("Category", "City.Controllers")]
        // CitiesController GetCity wrong id
        public void TestCitiesControlGetCityWrongId()
        {
            // Arrange
            var cities = new List<City>
            {
                new City{ Id = 1, Name = "Richmond", Description = "Small" },
                new City{ Id = 2, Name = "Vancouver", Description = "Big" }
            }.AsQueryable();

            var points = new List<PointOfInterest> { poi1, poi2, poi3, poi4 }.AsQueryable();

            var mockContext = CreateCityInfoContext(cities, points);

            ICityInfoRepository repo = new CityInfoRepository(mockContext);
            IValidator validator = new Validator();

            CitiesController controller = new CitiesController(repo, validator);

            // Act
            var result = controller.GetCity(3);
            var notFoundResult = result as NotFoundResult;

            // Assert
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        [Trait("Category", "City.Controllers")]
        // CitiesController GetCity
        public void TestCitiesControlGetCity()
        {
            // Arrange
            var cities = new List<City>
            {
                new City{ Id = 1, Name = "Richmond", Description = "Small" },
                new City{ Id = 2, Name = "Vancouver", Description = "Big" }
            }.AsQueryable();

            var points = new List<PointOfInterest> { poi1, poi2, poi3, poi4 }.AsQueryable();

            var mockContext = CreateCityInfoContext(cities, points);

            ICityInfoRepository repo = new CityInfoRepository(mockContext);
            IValidator validator = new Validator();

            CitiesController controller = new CitiesController(repo, validator);

            // Act
            var result = controller.GetCity(1);
            var okResult = result as OkObjectResult;

            // Assert
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        [Trait("Category", "PointOfInterest.Controllers")]
        // Points Controller GetPoints Wrong id
        public void TestPointsControlWrongId()
        {
            // Arrange
            var cities = new List<City>
            {
                new City{ Id = 1, Name = "Richmond", Description = "Small" },
                new City{ Id = 2, Name = "Vancouver", Description = "Big" }
            }.AsQueryable();

            var points = new List<PointOfInterest> { poi1, poi2, poi3, poi4 }.AsQueryable();

            var mockContext = CreateCityInfoContext(cities, points);

            ICityInfoRepository repo = new CityInfoRepository(mockContext);
            ILoggerFactory factory = new LoggerFactory();
            ILogger<PointsOfInterestController> logger = factory.CreateLogger<PointsOfInterestController>();
            IMailService service = new LocalMailService();
            IValidator validator = new Validator();
            PointsOfInterestController controller = new PointsOfInterestController(logger, service, repo, validator);

            // Act
            var result = controller.GetPointsOfInterest(3);
            var notFoundResult = result as NotFoundResult;

            // Assert
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }
    }
}
