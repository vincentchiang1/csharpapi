using System;
using Xunit;

namespace CityInfoUnitTests
{
    public class Class1
    {

        [Fact]
        public void PassingTest()
            {
            
            Assert.Equal(4, Add(2,2));
            }

        //[Fact]
        //public void FailingTest()
        //    {
        //   Assert.Equal(5, Add(2,2));
        //    }

        int Add(int x, int y)
        {
            return x + y;
        }


        //[Theory]
        //[InlineData(3)] // Theory is a parameterised test that is true for a subset of data denoted by [InlineData] 
        //[InlineData(5)] // Each theory with its data set is a separate test, thus 3 pieces of data is 3 tests for this theory
        //[InlineData(6)]

        //public void MyFirstTheory(int value)
        //    {

        //    Assert.True(IsOdd(value));
        //    }

        //bool IsOdd(int value)
        //    {

        //    return value % 2 == 1;
        //    }

        //[Fact]
        //public void TestNoPoints()
        //{
        //    CityDto city0 = new CityDto
        //    {
        //        Name = "Vancouver",
        //        Id = 1,
        //        Description = "Home"
        //    };
        //    Assert.Equal(0, city0.PointsOfInterest.Count);



        //}

        //[Fact]
        //public void TestPoints()
        //{
        //    PointOfInterestDto poi1 = new PointOfInterestDto
        //    {
        //        Id = 1,
        //        Name = "Downtown",
        //        Description = "Lots of food!"
        //    };

        //    PointOfInterestDto poi2 = new PointOfInterestDto
        //    {
        //        Id = 2,
        //        Name = "Stanley Park",
        //        Description = "Lots to do!"
        //    };

        //    PointOfInterestDto poi3 = new PointOfInterestDto
        //    {
        //        Id = 3,
        //        Name = "Science World",
        //        Description = "Lots of science!"
        //    };

        //    CityDto city1 = new CityDto
        //    {
        //        Name = "Van",
        //        Id = 2,
        //        Description = "home",
        //        PointsOfInterest = { poi1, poi2, poi3 }

        //    };

        //    Assert.Equal(3, city1.PointsOfInterest.Count);

        //}
    }
}
