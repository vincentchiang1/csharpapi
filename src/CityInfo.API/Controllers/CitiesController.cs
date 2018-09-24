using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Controllers
{
    [Route("api/cities")]
    public class CitiesController : Controller
    {
        private ICityInfoRepository _cityInfoRepository;
        private IValidator _validator;

        public CitiesController(ICityInfoRepository cityInfoRepository, IValidator validator)
        {
            _cityInfoRepository = cityInfoRepository;
            _validator = validator;
        }

        [HttpGet()]
        public IActionResult GetCities()
        {
            var cityEntities = _cityInfoRepository.GetCities();
            var results = Mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities); 

            return Ok(results);
        }

        [HttpGet("{id}")]
        public IActionResult GetCity(int id, bool includePointsOfInterest = false)
        {
            IActionResult error = null;
            var city = _cityInfoRepository.GetCity(id, includePointsOfInterest);

            if (!_validator.ValidateGetCity(id, _cityInfoRepository, includePointsOfInterest, ref error))
            {
                return error;
            }
          
            if (includePointsOfInterest)
            {
                var cityResult = Mapper.Map<CityDto>(city); 
                return Ok(cityResult);
            }

            var cityWithoutPointsOfInterestResult = Mapper.Map<CityWithoutPointsOfInterestDto>(city);
            return Ok(cityWithoutPointsOfInterestResult);
        }
    }
}
