using System.Collections.Generic;
using System.Linq;
using CityInfo.API.Entities;
using CityInfo.API.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CityInfoUnitTests
{
    public class TestRepository
    {        
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
        [Trait("GetCity", "Repository.City")]
        // No data, thus no id match
        public void TestGetCityNoCities()
        {
            var cities = new List<City>();
            IEnumerable<PointOfInterest> pointsOfInterest = null;
            var mockContext = CreateCityInfoContext(cities, pointsOfInterest);
            var repo = new CityInfoRepository(mockContext);
            Assert.Null(repo.GetCity(3, false));
        }

        [Fact]
        [Trait("GetCity", "Repository.City")]
        // Data, no id match
        public void TestGetCityWrongId()
        {
            var cities = new List<City>
            {
                new City{ Id = 1, Name = "Richmond", Description = "Small", PointsOfInterest = { poi1, poi2 } },
                new City{ Id = 2, Name = "Vancouver", Description = "Big", PointsOfInterest = { poi3, poi4 } },
            }.AsQueryable();

            var points = new List<PointOfInterest> { }.AsQueryable();

            var mockContext = CreateCityInfoContext(cities, points);

            var repo = new CityInfoRepository(mockContext);

            Assert.Null(repo.GetCity(3, false));
        }

        [Fact]
        [Trait("GetCity", "Repository.City")]
        // Data with POI(include = false)
        public void TestGetCityIncludeFalse()
        {
           var cities = new List<City>
            {
                new City{ Id = 1, Name = "Richmond", Description = "Small", PointsOfInterest = { poi1, poi2 } },
                new City{ Id = 2, Name = "Vancouver", Description = "Big", PointsOfInterest = { poi3, poi4 } },
            }.AsQueryable();

            var points = new List<PointOfInterest> { }.AsQueryable();

            var mockContext = CreateCityInfoContext(cities, points);

            var repo = new CityInfoRepository(mockContext);

            Assert.Equal(1, repo.GetCity(1, false).Id);
            Assert.Equal("Richmond", repo.GetCity(1, false).Name);
            Assert.Equal("Small", repo.GetCity(1, false).Description);
            Assert.Equal(2, repo.GetCity(1, false).PointsOfInterest.Count); 
            Assert.True(repo.GetCity(1, false).PointsOfInterest.Contains(poi1)); 
            Assert.True(repo.GetCity(1, false).PointsOfInterest.Contains(poi2)); 
        }

        [Fact]
        [Trait("GetCity", "Repository.City")]
        // Data with POI (include = true)
        public void TestGetCityIncludeTrue()
        {
            var cities = new List<City>
            {
                new City{ Id = 1, Name = "Richmond", Description = "Small", PointsOfInterest = { poi1, poi2 } },
                new City{ Id = 2, Name = "Vancouver", Description = "Big", PointsOfInterest = { poi3, poi4 } },
            }.AsQueryable();

            var points = new List<PointOfInterest> { }.AsQueryable();

            var mockContext = CreateCityInfoContext(cities, points);

            var repo = new CityInfoRepository(mockContext);

            Assert.Equal(1, repo.GetCity(1, true).Id);
            Assert.Equal("Richmond", repo.GetCity(1, true).Name);
            Assert.Equal("Small", repo.GetCity(1, true).Description);
            Assert.Equal(2, repo.GetCity(1, true).PointsOfInterest.Count); 
            Assert.True(repo.GetCity(1, true).PointsOfInterest.Contains(poi1)); 
            Assert.True(repo.GetCity(1, true).PointsOfInterest.Contains(poi2)); 
        }

        [Fact]
        [Trait("GetPoint", "Repository.City")]
        // No cities, thus no point of interest
        public void TestGetPointNoCities()
        {
            var cities = new List<City>{ }.AsQueryable();

            var points = new List<PointOfInterest> { }.AsQueryable();

            var mockContext = CreateCityInfoContext(cities, points);

            var repo = new CityInfoRepository(mockContext);

            Assert.Null(repo.GetPointOfInterestForCity(1, 1));
        }

        [Fact]
        [Trait("GetPoint", "Repository.City")]
        // Cities with wrong city id
        public void TestGetPointWrongCityId()
        {
            var cities = new List<City>
            {
                new City{ Id = 1, Name = "Richmond", Description = "Small" },
                new City{ Id = 2, Name = "Vancouver", Description = "Big" }
            }.AsQueryable();

            var points = new List<PointOfInterest> { poi1, poi2, poi3, poi4 }.AsQueryable();

            var mockContext = CreateCityInfoContext(cities, points);

            var repo = new CityInfoRepository(mockContext);

            Assert.Null(repo.GetPointOfInterestForCity(5, 1));
        }

        [Fact]
        [Trait("GetPoint", "Repository.City")]
        // Cities with existing city id, no points of interest
        public void TestGetPointNoPoints()
        {
            var cities = new List<City>
            {
                new City{ Id = 1, Name = "Richmond", Description = "Small" },
                new City{ Id = 2, Name = "Vancouver", Description = "Big" }
            }.AsQueryable();

            var points = new List<PointOfInterest> { }.AsQueryable();

            var mockContext = CreateCityInfoContext(cities, points);

            var repo = new CityInfoRepository(mockContext);

            Assert.Null(repo.GetPointOfInterestForCity(1, 1));
        }

        [Fact]
        [Trait("GetPoint", "Repository.City")]
        // Cities with exisiting city id, wrong poi id
        public void TestGetPointWrongId()
        {
            var cities = new List<City>
            {
                new City{ Id = 1, Name = "Richmond", Description = "Small" },
                new City{ Id = 2, Name = "Vancouver", Description = "Big" }
            }.AsQueryable();

            var points = new List<PointOfInterest> { poi1, poi2, poi3, poi4 }.AsQueryable();

            var mockContext = CreateCityInfoContext(cities, points);

            var repo = new CityInfoRepository(mockContext);

            Assert.Null(repo.GetPointOfInterestForCity(1, 3));
        }

        [Fact]
        [Trait("GetPoint", "Repository.City")]
        // Cities with existing city id, correct poi id
        public void TestGetPoint()
        {
            var cities = new List<City>
            {
                new City{ Id = 1, Name = "Richmond", Description = "Small" },
                new City{ Id = 2, Name = "Vancouver", Description = "Big" }
            }.AsQueryable();

            var points = new List<PointOfInterest> { poi1, poi2, poi3, poi4 }.AsQueryable();

            var mockContext = CreateCityInfoContext(cities, points);

            var repo = new CityInfoRepository(mockContext);

            Assert.Equal(2, repo.GetPointOfInterestForCity(1, 2).Id);
            Assert.Equal("Stanley Park", repo.GetPointOfInterestForCity(1, 2).Name);
            Assert.Equal("Lots to do", repo.GetPointOfInterestForCity(1, 2).Description);
            Assert.Equal(1, repo.GetPointOfInterestForCity(1, 2).CityId);
        }

        [Fact]
        [Trait("CityExists", "Repository.City")]
        // No cities thus does not exist
        public void TestCityExistsNoCities()
        {
            var cities = new List<City> { }.AsQueryable();

            var points = new List<PointOfInterest> { }.AsQueryable();

            var mockContext = CreateCityInfoContext(cities, points);

            var repo = new CityInfoRepository(mockContext);

            Assert.False(repo.CityExists(1));
        }

        [Fact]
        [Trait("CityExists", "Repository.City")]
        // Cities with wrong id thus does not exist
        public void TestCityExistsWrongId()
        {
            var cities = new List<City>
            {
                new City{ Id = 1, Name = "Richmond", Description = "Small" },
                new City{ Id = 2, Name = "Vancouver", Description = "Big" }
            }.AsQueryable();

            var points = new List<PointOfInterest> { }.AsQueryable();

            var mockContext = CreateCityInfoContext(cities, points);

            var repo = new CityInfoRepository(mockContext);

            Assert.False(repo.CityExists(3));
        }

        [Fact]
        [Trait("CityExists", "Repository.City")]
        // Cities with correct id thus does exist
        public void TestCityExists()
        {
            var cities = new List<City>
            {
                new City{ Id = 1, Name = "Richmond", Description = "Small" },
                new City{ Id = 2, Name = "Vancouver", Description = "Big" }
            }.AsQueryable();

            var points = new List<PointOfInterest> { }.AsQueryable();

            var mockContext = CreateCityInfoContext(cities, points);

            var repo = new CityInfoRepository(mockContext);

            Assert.True(repo.CityExists(1));
        }

        [Fact]
        [Trait("GetCities", "Repository.City")]
        // No cities thus empty list
        public void TestGetCitiesNoCities()
        {
            var cities = new List<City> { }.AsQueryable();

            var points = new List<PointOfInterest> { }.AsQueryable();

            var mockContext = CreateCityInfoContext(cities, points);

            var repo = new CityInfoRepository(mockContext);

            Assert.Empty(repo.GetCities());
        }

        [Fact]
        [Trait("GetCities", "Repository.City")]
        // Cities exist thus list of cities
        public void TestGetCities()
        {
            var cities = new List<City>
            {
                new City{ Id = 1, Name = "Richmond", Description = "Small" },
                new City{ Id = 2, Name = "Vancouver", Description = "Big" }
            }.AsQueryable();

            var points = new List<PointOfInterest> { }.AsQueryable();

            var mockContext = CreateCityInfoContext(cities, points);

            var repo = new CityInfoRepository(mockContext);

            var cities2 = repo.GetCities();
            Assert.Equal(2, cities2.Count());
            Assert.Equal(1, cities2.First().Id);
            Assert.Equal("Richmond", cities2.First().Name);
            Assert.Equal("Small", cities2.First().Description);
            Assert.Equal(2, cities2.Last().Id);
            Assert.Equal("Vancouver", cities2.Last().Name);
            Assert.Equal("Big", cities2.Last().Description);
        }

        [Fact]
        [Trait("GetPoints", "Repository.City")]
        // No cities thus no points
        public void TestGetPointsNoCities()
        {
            var cities = new List<City> { }.AsQueryable();

            var points = new List<PointOfInterest> { }.AsQueryable();

            var mockContext = CreateCityInfoContext(cities, points);

            var repo = new CityInfoRepository(mockContext);

            Assert.Empty(repo.GetPointsOfInterestForCity(1));
        }

        [Fact]
        [Trait("GetPoints", "Repository.City")]
        // Cities with wrong city id
        public void TestGetPointsWrongCityId()
        {
            var cities = new List<City>
            {
                new City{ Id = 1, Name = "Richmond", Description = "Small" },
                new City{ Id = 2, Name = "Vancouver", Description = "Big" }
            }.AsQueryable();

            var points = new List<PointOfInterest> { poi1, poi2, poi3, poi4 }.AsQueryable();

            var mockContext = CreateCityInfoContext(cities, points);

            var repo = new CityInfoRepository(mockContext);

            Assert.Empty(repo.GetPointsOfInterestForCity(3));
        }

        [Fact]
        [Trait("GetPoints", "Repository.City")]
        // City with no points
        public void TestGetPointsNoPoints()
        {
            var cities = new List<City>
            {
                new City{ Id = 1, Name = "Richmond", Description = "Small" },
                new City{ Id = 2, Name = "Vancouver", Description = "Big" },
                new City{ Id = 3, Name = "Burnaby", Description = "Far"}
            }.AsQueryable();

            var points = new List<PointOfInterest> { poi1, poi2, poi3, poi4 }.AsQueryable();

            var mockContext = CreateCityInfoContext(cities, points);

            var repo = new CityInfoRepository(mockContext);

            Assert.Empty(repo.GetPointsOfInterestForCity(3));
        }

        [Fact]
        [Trait("GetPoints", "Repository.City")]
        // Correct city id with points of interest
        public void TestGetPoints()
        {
            var cities = new List<City>
            {
                new City{ Id = 1, Name = "Richmond", Description = "Small" },
                new City{ Id = 2, Name = "Vancouver", Description = "Big" }
            }.AsQueryable();

            var points = new List<PointOfInterest> { poi1, poi2, poi3, poi4 }.AsQueryable();

            var mockContext = CreateCityInfoContext(cities, points);

            var repo = new CityInfoRepository(mockContext);

            Assert.Equal(2, repo.GetPointsOfInterestForCity(1).Count());
            Assert.Equal(1, repo.GetPointsOfInterestForCity(1).ElementAt(0).Id);
            Assert.Equal("Downtown", repo.GetPointsOfInterestForCity(1).ElementAt(0).Name);
            Assert.Equal("Lots of food", repo.GetPointsOfInterestForCity(1).ElementAt(0).Description);
            Assert.Equal(1, repo.GetPointsOfInterestForCity(1).ElementAt(0).CityId);
            Assert.Equal(2, repo.GetPointsOfInterestForCity(1).ElementAt(1).Id);
            Assert.Equal("Stanley Park", repo.GetPointsOfInterestForCity(1).ElementAt(1).Name);
            Assert.Equal("Lots to do", repo.GetPointsOfInterestForCity(1).ElementAt(1).Description);
        }        
    }
}
