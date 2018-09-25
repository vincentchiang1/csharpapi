using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CityInfo.API;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CityInfoUnitTests
{
    public class TestValidator
    {
        public TestValidator()
        {
            Mapper.Reset();
            Startup start = new Startup(null);
            start.InitializeMapper();
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
        [Trait("Category", "City.Validator")]
        // Validate GetCity where city does not exist 
        public void TestValidateGetCityWrongId()
        {
            var cities = new List<City> { }.AsQueryable();

            var points = new List<PointOfInterest> { }.AsQueryable();

            var mockContext = CreateCityInfoContext(cities, points);

            ICityInfoRepository repo = new CityInfoRepository(mockContext);

            IValidator validator = new Validator();

            IActionResult error = null;

            Assert.False(validator.ValidateGetCity(1, repo, false, ref error));
        }

        [Fact]
        [Trait("Category", "City.Validator")]
        // Validate GetCity 
        public void TestValidateGetCity()
        {
            var cities = new List<City>
            {
                new City{ Id = 1, Name = "Richmond", Description = "Small" },
                new City{ Id = 2, Name = "Vancouver", Description = "Big" }
            }.AsQueryable();

            var points = new List<PointOfInterest> { poi1, poi2, poi3, poi4 }.AsQueryable();

            var mockContext = CreateCityInfoContext(cities, points);

            ICityInfoRepository repo = new CityInfoRepository(mockContext);

            IValidator validator = new Validator();

            IActionResult error = null;

            Assert.True(validator.ValidateGetCity(1, repo, false, ref error));
        }

        [Fact]
        [Trait("Category", "PointOfInterest.Validator")]
        // Validate GetPoints with wrong city
        public void TestValidateGetPointsWrongCity()
        {
            var cities = new List<City>
            {
                new City{ Id = 1, Name = "Richmond", Description = "Small" },
                new City{ Id = 2, Name = "Vancouver", Description = "Big" }
            }.AsQueryable();

            var points = new List<PointOfInterest> { poi1, poi2, poi3, poi4 }.AsQueryable();

            var mockContext = CreateCityInfoContext(cities, points);

            ICityInfoRepository repo = new CityInfoRepository(mockContext);

            IValidator validator = new Validator();

            Assert.False(validator.ValidateGetPoints(3, repo));
        }

        [Fact]
        [Trait("Category", "PointOfInterest.Validator")]
        // Validate GetPoints
        public void TestValidateGetPoints()
        {
            var cities = new List<City>
            {
                new City{ Id = 1, Name = "Richmond", Description = "Small" },
                new City{ Id = 2, Name = "Vancouver", Description = "Big" }
            }.AsQueryable();

            var points = new List<PointOfInterest> { poi1, poi2, poi3, poi4 }.AsQueryable();

            var mockContext = CreateCityInfoContext(cities, points);

            ICityInfoRepository repo = new CityInfoRepository(mockContext);

            IValidator validator = new Validator();

            Assert.True(validator.ValidateGetPoints(1, repo));
        }

        [Fact]
        [Trait("Category", "PointOfInterest.Validator")]
        // Validate GetPoint wrong city
        public void TestValidateGetPointWrongCity()
        {
            var cities = new List<City>
            {
                new City{ Id = 1, Name = "Richmond", Description = "Small" },
                new City{ Id = 2, Name = "Vancouver", Description = "Big" }
            }.AsQueryable();

            var points = new List<PointOfInterest> { poi1, poi2, poi3, poi4 }.AsQueryable();

            var mockContext = CreateCityInfoContext(cities, points);

            ICityInfoRepository repo = new CityInfoRepository(mockContext);

            IValidator validator = new Validator();

            Assert.False(validator.ValidateGetPoint(3, 1, repo));
        }

        [Fact]
        [Trait("Category", "PointOfInterest.Validator")]
        // Validate GetPoint wrong point
        public void TestValidateGetPointWrongPoint()
        {
            var cities = new List<City>
            {
                new City{ Id = 1, Name = "Richmond", Description = "Small" },
                new City{ Id = 2, Name = "Vancouver", Description = "Big" }
            }.AsQueryable();

            var points = new List<PointOfInterest> { poi1, poi2, poi3, poi4 }.AsQueryable();

            var mockContext = CreateCityInfoContext(cities, points);

            ICityInfoRepository repo = new CityInfoRepository(mockContext);

            IValidator validator = new Validator();

            Assert.False(validator.ValidateGetPoint(1, 3, repo));
        }

        [Fact]
        [Trait("Category", "PointOfInterest.Validator")]
        // Validate GetPoint
        public void TestValidateGetPoint()
        {
            var cities = new List<City>
            {
                new City{ Id = 1, Name = "Richmond", Description = "Small" },
                new City{ Id = 2, Name = "Vancouver", Description = "Big" }
            }.AsQueryable();

            var points = new List<PointOfInterest> { poi1, poi2, poi3, poi4 }.AsQueryable();

            var mockContext = CreateCityInfoContext(cities, points);

            ICityInfoRepository repo = new CityInfoRepository(mockContext);

            IValidator validator = new Validator();

            Assert.True(validator.ValidateGetPoint(1, 1, repo));
        }

        [Fact]
        [Trait("Category", "PointOfInterest.Validator")]
        // ValidateCreate null point
        public void TestValidateCreateNull()
        {
            var cities = new List<City>
            {
                new City{ Id = 1, Name = "Richmond", Description = "Small" },
                new City{ Id = 2, Name = "Vancouver", Description = "Big" }
            }.AsQueryable();

            var points = new List<PointOfInterest> { poi1, poi2, poi3, poi4 }.AsQueryable();

            var mockContext = CreateCityInfoContext(cities, points);

            ICityInfoRepository repo = new CityInfoRepository(mockContext);

            IValidator validator = new Validator();

            PointOfInterestForCreationDto point = null;

            IActionResult error = null;

            Assert.False(validator.ValidateCreate(1, point, repo, ref error));
        }

        [Fact]
        [Trait("Category", "PointOfInterest.Validator")]
        // ValidateCreate description = name
        public void TestValidateCreateDescName()
        {
            var cities = new List<City>
            {
                new City{ Id = 1, Name = "Richmond", Description = "Small" },
                new City{ Id = 2, Name = "Vancouver", Description = "Big" }
            }.AsQueryable();

            var points = new List<PointOfInterest> { poi1, poi2, poi3, poi4 }.AsQueryable();

            var mockContext = CreateCityInfoContext(cities, points);

            ICityInfoRepository repo = new CityInfoRepository(mockContext);

            IValidator validator = new Validator();

            PointOfInterestForCreationDto point = new PointOfInterestForCreationDto
            {
                Name = "hello",
                Description = "hello"
            };

            IActionResult error = null;

            Assert.False(validator.ValidateCreate(1, point, repo, ref error));
        }

        [Fact]
        [Trait("Category", "PointOfInterest.Validator")]
        // ValidateCreate wrong city
        public void TestValidateCreateWrongCity()
        {
            var cities = new List<City>
            {
                new City{ Id = 1, Name = "Richmond", Description = "Small" },
                new City{ Id = 2, Name = "Vancouver", Description = "Big" }
            }.AsQueryable();

            var points = new List<PointOfInterest> { poi1, poi2, poi3, poi4 }.AsQueryable();

            var mockContext = CreateCityInfoContext(cities, points);

            ICityInfoRepository repo = new CityInfoRepository(mockContext);

            IValidator validator = new Validator();

            PointOfInterestForCreationDto point = new PointOfInterestForCreationDto
            {
                Name = "hello",
                Description = "bye"
            };

            IActionResult error = null;

            Assert.False(validator.ValidateCreate(3, point, repo, ref error));
        }

        [Fact]
        [Trait("Category", "PointOfInterest.Validator")]
        // ValidateCreate 
        public void TestValidateCreate()
        {
            var cities = new List<City>
            {
                new City{ Id = 1, Name = "Richmond", Description = "Small" },
                new City{ Id = 2, Name = "Vancouver", Description = "Big" }
            }.AsQueryable();

            var points = new List<PointOfInterest> { poi1, poi2, poi3, poi4 }.AsQueryable();

            var mockContext = CreateCityInfoContext(cities, points);

            ICityInfoRepository repo = new CityInfoRepository(mockContext);

            IValidator validator = new Validator();

            PointOfInterestForCreationDto point = new PointOfInterestForCreationDto
            {
                Name = "hello",
                Description = "bye"
            };

            IActionResult error = null;

            Assert.True(validator.ValidateCreate(1, point, repo, ref error));
        }

        [Fact]
        [Trait("Category", "PointOfInterest.Validator")]
        // ValidateUpdate null point
        public void TestValidateUpdateNullPoint()
        {
            var cities = new List<City>
            {
                new City{ Id = 1, Name = "Richmond", Description = "Small" },
                new City{ Id = 2, Name = "Vancouver", Description = "Big" }
            }.AsQueryable();

            var points = new List<PointOfInterest> { poi1, poi2, poi3, poi4 }.AsQueryable();

            var mockContext = CreateCityInfoContext(cities, points);

            ICityInfoRepository repo = new CityInfoRepository(mockContext);

            IValidator validator = new Validator();

            PointOfInterestForUpdateDto point = null;

            IActionResult error = null;

            Assert.False(validator.ValidateUpdate(1, 1, point, repo, ref error));
        }

        [Fact]
        [Trait("Category", "PointOfInterest.Validator")]
        // ValidateUpdate name = desc
        public void TestValidateUpdateNameDesc()
        {
            var cities = new List<City>
            {
                new City{ Id = 1, Name = "Richmond", Description = "Small" },
                new City{ Id = 2, Name = "Vancouver", Description = "Big" }
            }.AsQueryable();

            var points = new List<PointOfInterest> { poi1, poi2, poi3, poi4 }.AsQueryable();

            var mockContext = CreateCityInfoContext(cities, points);

            ICityInfoRepository repo = new CityInfoRepository(mockContext);

            IValidator validator = new Validator();

            PointOfInterestForUpdateDto point = new PointOfInterestForUpdateDto
            {
                Name = "hello",
                Description = "hello"
            };
            IActionResult error = null;

            Assert.False(validator.ValidateUpdate(1, 1, point, repo, ref error));
        }

        [Fact]
        [Trait("Category", "PointOfInterest.Validator")]
        // ValidateUpdate wrong city
        public void TestValidateUpdateWrongCity()
        {
            var cities = new List<City>
            {
                new City{ Id = 1, Name = "Richmond", Description = "Small" },
                new City{ Id = 2, Name = "Vancouver", Description = "Big" }
            }.AsQueryable();

            var points = new List<PointOfInterest> { poi1, poi2, poi3, poi4 }.AsQueryable();

            var mockContext = CreateCityInfoContext(cities, points);

            ICityInfoRepository repo = new CityInfoRepository(mockContext);

            IValidator validator = new Validator();

            PointOfInterestForUpdateDto point = new PointOfInterestForUpdateDto
            {
                Name = "hello",
                Description = "bye"
            };
            IActionResult error = null;

            Assert.False(validator.ValidateUpdate(3, 1, point, repo, ref error));
        }

        [Fact]
        [Trait("Category", "PointOfInterest.Validator")]
        // ValidateUpdate wrong point
        public void TestValidateUpdateWrongPoint()
        {
            var cities = new List<City>
            {
                new City{ Id = 1, Name = "Richmond", Description = "Small" },
                new City{ Id = 2, Name = "Vancouver", Description = "Big" }
            }.AsQueryable();

            var points = new List<PointOfInterest> { poi1, poi2, poi3, poi4 }.AsQueryable();

            var mockContext = CreateCityInfoContext(cities, points);

            ICityInfoRepository repo = new CityInfoRepository(mockContext);

            IValidator validator = new Validator();

            PointOfInterestForUpdateDto point = new PointOfInterestForUpdateDto
            {
                Name = "hello",
                Description = "bye"
            };
            IActionResult error = null;

            Assert.False(validator.ValidateUpdate(1, 3, point, repo, ref error));
        }

        [Fact]
        [Trait("Category", "PointOfInterest.Validator")]
        // ValidateUpdate 
        public void TestValidateUpdate()
        {
            var cities = new List<City>
            {
                new City{ Id = 1, Name = "Richmond", Description = "Small" },
                new City{ Id = 2, Name = "Vancouver", Description = "Big" }
            }.AsQueryable();

            var points = new List<PointOfInterest> { poi1, poi2, poi3, poi4 }.AsQueryable();

            var mockContext = CreateCityInfoContext(cities, points);

            ICityInfoRepository repo = new CityInfoRepository(mockContext);

            IValidator validator = new Validator();

            PointOfInterestForUpdateDto point = new PointOfInterestForUpdateDto
            {
                Name = "hello",
                Description = "bye"
            };
            IActionResult error = null;

            Assert.True(validator.ValidateUpdate(1, 2, point, repo, ref error));
        }

        [Fact]
        [Trait("Category", "PointOfInterest.Validator")]
        // ValidatePartially null patchDoc
        public void TestValidatePartiallyNullPatchDoc()
        {
            var cities = new List<City>
            {
                new City{ Id = 1, Name = "Richmond", Description = "Small" },
                new City{ Id = 2, Name = "Vancouver", Description = "Big" }
            }.AsQueryable();

            var points = new List<PointOfInterest> { poi1, poi2, poi3, poi4 }.AsQueryable();

            var mockContext = CreateCityInfoContext(cities, points);

            ICityInfoRepository repo = new CityInfoRepository(mockContext);

            IValidator validator = new Validator();

            JsonPatchDocument<PointOfInterestForUpdateDto> patchDoc = null;

            IActionResult error = null;

            Assert.False(validator.ValidatePartially(1, 1, patchDoc, repo, ref error));
        }

        [Fact]
        [Trait("Category", "PointOfInterest.Validator")]
        // ValidatePartially wrong city
        public void TestValidatePartiallyWrongCity()
        {

        }

        [Fact]
        [Trait("Category", "PointOfInterest.Validator")]
        // ValidatePartially wrong point
        public void TestValidatePartiallyWrongPoint()
        {

        }

        [Fact]
        [Trait("Category", "PointOfInterest.Validator")]
        // ValidatePartially Model Not Valid
        public void TestValidatePartiallyInvalidModel()
        {

        }

        [Fact]
        [Trait("Category", "PointOfInterest.Validator")]
        // ValidatePartially name = desc
        public void TestValidatePartiallyNameDesc()
        {

        }

        [Fact]
        [Trait("Category", "PointOfInterest.Validator")]
        // ValidatePartially
        public void TestValidatePartially()
        {

        }

        [Fact]
        [Trait("Category", "PointOfInterest.Validator")]
        // ValidateDelete wrong city
        public void TestValidateDeleteWrongCity()
        {
            var cities = new List<City>
            {
                new City{ Id = 1, Name = "Richmond", Description = "Small" },
                new City{ Id = 2, Name = "Vancouver", Description = "Big" }
            }.AsQueryable();

            var points = new List<PointOfInterest> { poi1, poi2, poi3, poi4 }.AsQueryable();

            var mockContext = CreateCityInfoContext(cities, points);

            ICityInfoRepository repo = new CityInfoRepository(mockContext);

            IValidator validator = new Validator();

            IActionResult error = null;

            Assert.False(validator.ValidateDelete(3, 1, repo, ref error));
        }

        [Fact]
        [Trait("Category", "PointOfInterest.Validator")]
        // ValidateDelete wrong point
        public void TestValidateDeleteWrongPoint()
        {
            var cities = new List<City>
            {
                new City{ Id = 1, Name = "Richmond", Description = "Small" },
                new City{ Id = 2, Name = "Vancouver", Description = "Big" }
            }.AsQueryable();

            var points = new List<PointOfInterest> { poi1, poi2, poi3, poi4 }.AsQueryable();

            var mockContext = CreateCityInfoContext(cities, points);

            ICityInfoRepository repo = new CityInfoRepository(mockContext);

            IValidator validator = new Validator();

            IActionResult error = null;

            Assert.False(validator.ValidateDelete(1, 5, repo, ref error));
        }

        [Fact]
        [Trait("Category", "PointOfInterest.Validator")]
        // ValidateDelete 
        public void TestValidateDelete()
        {
            var cities = new List<City>
            {
                new City{ Id = 1, Name = "Richmond", Description = "Small" },
                new City{ Id = 2, Name = "Vancouver", Description = "Big" }
            }.AsQueryable();

            var points = new List<PointOfInterest> { poi1, poi2, poi3, poi4 }.AsQueryable();

            var mockContext = CreateCityInfoContext(cities, points);

            ICityInfoRepository repo = new CityInfoRepository(mockContext);

            IValidator validator = new Validator();

            IActionResult error = null;

            Assert.True(validator.ValidateDelete(1, 1, repo, ref error));
        }
    }
}
