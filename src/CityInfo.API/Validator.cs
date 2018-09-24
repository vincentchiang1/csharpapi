using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API
{
    public class Validator:Controller, IValidator
    {
        public bool ValidateGetPoints(int cityId, ICityInfoRepository repo)
        {
            if (!repo.CityExists(cityId)){
                return false;
            }

            return true;
        }

        public bool ValidateGetPoint(int cityId, int id, ICityInfoRepository repo)
        {
            if (!repo.CityExists(cityId) || repo.GetPointOfInterestForCity(cityId, id) == null)
            {
                return false;
            }

            return true;
        }

        public bool ValidateCreate(int cityId, 
            PointOfInterestForCreationDto point, 
            ICityInfoRepository repo, 
            IActionResult error)
        {
            if(point == null)
            {
                error = BadRequest();
                return false;
            }

            if(point.Description == point.Name)
            {
                ModelState.AddModelError("Description", "The provided description should be different from the name.");
                error = BadRequest(ModelState);
                return false;
            }

            if (!repo.CityExists(cityId))
            {
                error = NotFound();
                return false;
            }

            var finalPointOfInterest = Mapper.Map<Entities.PointOfInterest>(point);

            repo.AddPointOfInterestForCity(cityId, finalPointOfInterest);

            if (!repo.Save())
            {
                error = StatusCode(500, "A problem happened while handling your request.");
                return false;
            }

            return true;
        }

        public bool ValidateUpdate(int cityId, 
            int id, 
            PointOfInterestForUpdateDto point, 
            ICityInfoRepository repo,
            IActionResult error)
        {
            if (point == null)
            {
                error = BadRequest();
                return false;
            }

            if (point.Description == point.Name)
            {
                ModelState.AddModelError("Description", "The provided description should be different from the name.");
                error = BadRequest(ModelState);
                return false;
            }

            if (!repo.CityExists(cityId))
            {
                error = NotFound();
                return false;
            }

            if (repo.GetPointOfInterestForCity(cityId, id) == null)
            {
                error = NotFound();
                return false;
            }

            return true;
        }

        public bool ValidatePartially(int cityId, 
            int id, 
            JsonPatchDocument<PointOfInterestForUpdateDto> patchDoc,
            ICityInfoRepository repo,
            IActionResult error)
        {
            if (patchDoc == null)
            {
                error = BadRequest();
                return false;
            }

            if (!repo.CityExists(cityId))
            {
                error = NotFound();
                return false;
            }

            if(repo.GetPointOfInterestForCity(cityId,id) == null)
            {
                error = NotFound();
                return false;
            }

            var pointOfInterestEntity = repo.GetPointOfInterestForCity(cityId, id);
            var pointOfInterestToPatch = Mapper.Map<PointOfInterestForUpdateDto>(pointOfInterestEntity);

            patchDoc.ApplyTo(pointOfInterestToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                error = BadRequest(ModelState);
                return false;
            }

            if (pointOfInterestToPatch.Description == pointOfInterestToPatch.Name)
            {
                ModelState.AddModelError("Description", "The provided description should be different from the name.");
            }

            TryValidateModel(pointOfInterestToPatch);

            if (!ModelState.IsValid)
            {
                error = BadRequest(ModelState);
                return false;
            }

            return true;
        }

        public bool ValidateDelete(int cityId, int id, ICityInfoRepository repo, IActionResult error)
        {
            if (!repo.CityExists(cityId))
            {
                error = NotFound();
                return false;
            }

            if (repo.GetPointOfInterestForCity(cityId, id) == null)
            {
                error = NotFound();
                return false;
            }

            return true;
        }

        public bool ValidateGetCity(int id, ICityInfoRepository repo, bool includePointsOfInterest, ref IActionResult error)
        {
            if(repo.GetCity(id,includePointsOfInterest) == null)
            {
                error = NotFound();
                return false;
            }

            return true;
        }
    }
}
