using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API
{
    public interface IValidator
    {
         bool ValidateGetPoints(int cityId, ICityInfoRepository repo);
         bool ValidateGetPoint(int cityId, int id, ICityInfoRepository repo);
         bool ValidateCreate(int cityId,
            PointOfInterestForCreationDto point,
            ICityInfoRepository repo,
            IActionResult error);
         bool ValidateUpdate(int cityId,
            int id,
            PointOfInterestForUpdateDto point,
            ICityInfoRepository repo,
            IActionResult error);
         bool ValidatePartially(int cityId,
            int id,
            JsonPatchDocument<PointOfInterestForUpdateDto> patchDoc,
            ICityInfoRepository repo,
            IActionResult error);
         bool ValidateDelete(int cityId, int id, ICityInfoRepository repo, IActionResult error);
         bool ValidateGetCity(int id, ICityInfoRepository repo, bool includePointsOfInterest, IActionResult error);


    }
}
