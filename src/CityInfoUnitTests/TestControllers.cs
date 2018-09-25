using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CityInfo.API;
using CityInfo.API.Controllers;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
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
            Startup s = new Startup(null);
            Mapper.Reset();
            s.InitializeMapper();

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
        public void TestPointsControlGetPointsWrongId()
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
            IMailService service = new LocalMailService("vincent@moj.io", "vincent@gmail.com");
            IValidator validator = new Validator();
            PointsOfInterestController controller = new PointsOfInterestController(logger, service, repo, validator);

            // Act
            var result = controller.GetPointsOfInterest(3);
            var notFoundResult = result as NotFoundResult;

            // Assert
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        [Trait("Category", "PointOfInterest.Controllers")]
        // Points Controller GetPoints 
        public void TestPointsControlGetPoints()
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
            IMailService service = new LocalMailService("vincent@moj.io", "vincent@gmail.com");
            IValidator validator = new Validator();
            PointsOfInterestController controller = new PointsOfInterestController(logger, service, repo, validator);

            // Act
            var result = controller.GetPointsOfInterest(1);
            var okResult = result as OkObjectResult;

            // Assert
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        [Trait("Category", "PointOfInterest.Controllers")]
        // Points Controller GetPoint wrong city
        public void TestPointsControlGetPointWrongCity()
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
            IMailService service = new LocalMailService("vincent@moj.io", "vincent@gmail.com");
            IValidator validator = new Validator();
            PointsOfInterestController controller = new PointsOfInterestController(logger, service, repo, validator);

            // Act
            var result = controller.GetPointOfInterest(3,1);
            var notFoundResult = result as NotFoundResult;

            // Assert
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        [Trait("Category", "PointOfInterest.Controllers")]
        // Points Controller GetPoint wrong point
        public void TestPointsControlGetPointWrongPoint()
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
            IMailService service = new LocalMailService("vincent@moj.io", "vincent@gmail.com");
            IValidator validator = new Validator();
            PointsOfInterestController controller = new PointsOfInterestController(logger, service, repo, validator);

            // Act
            var result = controller.GetPointOfInterest(1, 3);
            var notFoundResult = result as NotFoundResult;

            // Assert
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        [Trait("Category", "PointOfInterest.Controllers")]
        // Points Controller GetPoint 
        public void TestPointsControlGetPoint()
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
            IMailService service = new LocalMailService("vincent@moj.io", "vincent@gmail.com");
            IValidator validator = new Validator();
            PointsOfInterestController controller = new PointsOfInterestController(logger, service, repo, validator);

            // Act
            var result = controller.GetPointOfInterest(1, 1);
            var okResult = result as OkObjectResult;

            // Assert
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        [Trait("Category", "PointOfInterest.Controllers")]
        // Points Controller CreatePoint null point
        public void TestPointsControlCreateNullPoint()
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
            IMailService service = new LocalMailService("vincent@moj.io", "vincent@gmail.com");
            IValidator validator = new Validator();
            PointOfInterestForCreationDto point = null;
            PointsOfInterestController controller = new PointsOfInterestController(logger, service, repo, validator);

            // Act
            var result = controller.CreatePointOfInterest(1, point);
            var badRequestResult = result as BadRequestResult;

            // Assert
            Assert.NotNull(badRequestResult);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        [Trait("Category", "PointOfInterest.Controllers")]
        // PointsController CreatePoint name = desc
        public void TestPointsControlCreateNameDesc()
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
            IMailService service = new LocalMailService("vincent@moj.io", "vincent@gmail.com");
            IValidator validator = new Validator();
            PointOfInterestForCreationDto point = new PointOfInterestForCreationDto
            {
                Name = "hi",
                Description = "hi"
            };

            PointsOfInterestController controller = new PointsOfInterestController(logger, service, repo, validator);

            // Act
            var result = controller.CreatePointOfInterest(1, point);
            var badRequestResult = result as BadRequestObjectResult;

            // Assert
            Assert.NotNull(badRequestResult);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        [Trait("Category", "PointOfInterest.Controllers")]
        // PointsController CreatePoint wrong city
        public void TestPointsControlCreateWrongCity()
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
            IMailService service = new LocalMailService("vincent@moj.io", "vincent@gmail.com");
            IValidator validator = new Validator();
            PointOfInterestForCreationDto point = new PointOfInterestForCreationDto
            {
                Name = "hi",
                Description = "bye"
            };

            PointsOfInterestController controller = new PointsOfInterestController(logger, service, repo, validator);

            // Act
            var result = controller.CreatePointOfInterest(5, point);
            var notFoundResult = result as NotFoundResult;

            // Assert
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        [Trait("Category", "PointOfInterest.Controllers")]
        // PointsController CreatePoint 
        public void TestPointsControlCreate()
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
            IMailService service = new LocalMailService("vincent@moj.io", "vincent@gmail.com");
            IValidator validator = new Validator();
            PointOfInterestForCreationDto point = new PointOfInterestForCreationDto
            {
                Name = "hi",
                Description = "bye"
            };

            PointsOfInterestController controller = new PointsOfInterestController(logger, service, repo, validator);

            // Act
            var result = controller.CreatePointOfInterest(2, point);
            var createdAtRouteResult = result as CreatedAtRouteResult;

            // Assert
            Assert.NotNull(createdAtRouteResult);
            Assert.Equal(201, createdAtRouteResult.StatusCode);
        }

        [Fact]
        [Trait("Category", "PointOfInterest.Controllers")]
        // PointsController UpdatePoint null point
        public void TestPointsControlUpdateNullPoint()
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
            IMailService service = new LocalMailService("vincent@moj.io", "vincent@gmail.com");
            IValidator validator = new Validator();
            PointOfInterestForUpdateDto point = null;
            PointsOfInterestController controller = new PointsOfInterestController(logger, service, repo, validator);

            // Act
            var result = controller.UpdatePointOfInterest(1, 1, point);
            var badRequestResult = result as BadRequestResult;

            // Assert
            Assert.NotNull(badRequestResult);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        [Trait("Category", "PointOfInterest.Controllers")]
        // PointsController UpdatePoint name = desc
        public void TestPointsControlUpdateNameDesc()
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
            IMailService service = new LocalMailService("vincent@moj.io", "vincent@gmail.com");
            IValidator validator = new Validator();
            PointOfInterestForUpdateDto point = new PointOfInterestForUpdateDto()
            {
                Name = "x",
                Description = "x"
            };

            PointsOfInterestController controller = new PointsOfInterestController(logger, service, repo, validator);

            // Act
            var result = controller.UpdatePointOfInterest(1, 1, point);
            var badRequestResult = result as BadRequestObjectResult;

            // Assert
            Assert.NotNull(badRequestResult);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        [Trait("Category", "PointOfInterest.Controllers")]
        // PointsController UpdatePoint Wrong City
        public void TestPointsControlUpdateWrongCity()
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
            IMailService service = new LocalMailService("vincent@moj.io", "vincent@gmail.com");
            IValidator validator = new Validator();
            PointOfInterestForUpdateDto point = new PointOfInterestForUpdateDto()
            {
                Name = "a",
                Description = "b"
            };

            PointsOfInterestController controller = new PointsOfInterestController(logger, service, repo, validator);

            // Act
            var result = controller.UpdatePointOfInterest(5, 1, point);
            var notFoundResult = result as NotFoundResult;

            // Assert
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        [Trait("Category", "PointOfInterest.Controllers")]
        // PointsController UpdatePoint Wrong City
        public void TestPointsControlUpdateWrongPoint()
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
            IMailService service = new LocalMailService("vincent@moj.io", "vincent@gmail.com");
            IValidator validator = new Validator();
            PointOfInterestForUpdateDto point = new PointOfInterestForUpdateDto()
            {
                Name = "a",
                Description = "b"
            };

            PointsOfInterestController controller = new PointsOfInterestController(logger, service, repo, validator);

            // Act
            var result = controller.UpdatePointOfInterest(1, 5, point);
            var notFoundResult = result as NotFoundResult;

            // Assert
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        [Trait("Category", "PointOfInterest.Controllers")]
        // PointsController UpdatePoint 
        public void TestPointsControlUpdate()
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
            IMailService service = new LocalMailService("vincent@moj.io", "vincent@gmail.com");
            IValidator validator = new Validator();
            PointOfInterestForUpdateDto point = new PointOfInterestForUpdateDto()
            {
                Name = "a",
                Description = "b"
            };

            PointsOfInterestController controller = new PointsOfInterestController(logger, service, repo, validator);

            // Act
            var result = controller.UpdatePointOfInterest(1, 1, point);
            var noContentResult = result as NoContentResult;

            // Assert
            Assert.NotNull(noContentResult);
            Assert.Equal(204, noContentResult.StatusCode);
        }

        [Fact]
        [Trait("Category", "PointOfInterest.Controllers")]
        // PointsController PartiallyUpdatePoint null patch doc
        public void TestPointsControlPartiallyUpdateNullDoc()
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
            IMailService service = new LocalMailService("vincent@moj.io", "vincent@gmail.com");
            IValidator validator = new Validator();
            JsonPatchDocument<PointOfInterestForUpdateDto> patch = null;

            PointsOfInterestController controller = new PointsOfInterestController(logger, service, repo, validator);

            // Act
            var result = controller.PartiallyUpdatePointOfInterest(1, 1, patch);
            var badRequestResult = result as BadRequestResult;

            // Assert
            Assert.NotNull(badRequestResult);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        [Trait("Category", "PointOfInterest.Controllers")]
        // PointsController PartiallyUpdatePoint wrong city
        public void TestPointsControlPartiallyUpdateWrongCity()
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
            IMailService service = new LocalMailService("vincent@moj.io", "vincent@gmail.com");
            IValidator validator = new Validator();
            JsonPatchDocument<PointOfInterestForUpdateDto> patch = new JsonPatchDocument<PointOfInterestForUpdateDto>();
            patch.Replace(n => n.Name, "Updated - Hello");

            PointsOfInterestController controller = new PointsOfInterestController(logger, service, repo, validator);

            // Act
            var result = controller.PartiallyUpdatePointOfInterest(3, 1, patch);
            var notFoundResult = result as NotFoundResult;

            // Assert
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        [Trait("Category", "PointOfInterest.Controllers")]
        // PointsController PartiallyUpdatePoint wrong point
        public void TestPointsControlPartiallyUpdateWrongPoint()
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
            IMailService service = new LocalMailService("vincent@moj.io", "vincent@gmail.com");
            IValidator validator = new Validator();
            JsonPatchDocument<PointOfInterestForUpdateDto> patch = new JsonPatchDocument<PointOfInterestForUpdateDto>();
            patch.Replace(n => n.Name, "Updated - Hello");

            PointsOfInterestController controller = new PointsOfInterestController(logger, service, repo, validator);

            // Act
            var result = controller.PartiallyUpdatePointOfInterest(1, 3, patch);
            var notFoundResult = result as NotFoundResult;

            // Assert
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        [Trait("Category", "PointOfInterest.Controllers")]
        // PointsController PartiallyUpdatePoint name = description
        public void TestPointsControlPartiallyUpdateNameDesc()
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
            IMailService service = new LocalMailService("vincent@moj.io", "vincent@gmail.com");
            IValidator validator = new Validator();
            JsonPatchDocument<PointOfInterestForUpdateDto> patch = new JsonPatchDocument<PointOfInterestForUpdateDto>();
            patch.Replace(n => n.Name, "Lots of food");

            PointsOfInterestController controller = new PointsOfInterestController(logger, service, repo, validator);

            // Act
            var result = controller.PartiallyUpdatePointOfInterest(1, 1, patch);
            var badRequestResult = result as BadRequestObjectResult;

            // Assert
            Assert.NotNull(badRequestResult);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        [Trait("Category", "PointOfInterest.Controllers")]
        // PointsController PartiallyUpdatePoint
        public void TestPointsControlPartiallyUpdate()
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
            IMailService service = new LocalMailService("vincent@moj.io", "vincent@gmail.com");
            IValidator validator = new Validator();
            JsonPatchDocument<PointOfInterestForUpdateDto> patch = new JsonPatchDocument<PointOfInterestForUpdateDto>();
            patch.Replace(n => n.Name, "Updated name");

            PointsOfInterestController controller = new PointsOfInterestController(logger, service, repo, validator);

            // Act
            var result = controller.PartiallyUpdatePointOfInterest(1, 1, patch);
            var noContentResult = result as NoContentResult;

            // Assert
            Assert.NotNull(noContentResult);
            Assert.Equal(204, noContentResult.StatusCode);
        }
    }
}

