using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheWorld.Models;
using TheWorld.Services;
using TheWorld.ViewModels;

namespace TheWorld.Controllers.Api
{
    [Route("/api/trips/{tripName}/stops")]
    public class StopsController: Controller
    {
        private GeoCoordsService _coords;
        private ILogger _logger;
        private IWorldRepository _repository;

        public StopsController(IWorldRepository repository, ILogger<StopsController> logger, GeoCoordsService coords)
        {
            _repository = repository;
            _logger = logger;
            _coords = coords;
        }

        [HttpGet]
        public IActionResult Get(string tripName)
        {
            try
            {
                var trip = _repository.GetTripByName(tripName);
                var stops = trip.Stops.OrderBy(s => s.Order).ToList();
                return Ok(Mapper.Map<IEnumerable<StopViewModel>>(stops));
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get stops {0}", ex);
            }

            return BadRequest("Failed to get stops");
        }

        [HttpPost]
        public async Task<IActionResult> Post(string tripName, [FromBody]StopViewModel vm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var newStop = Mapper.Map<Stop>(vm);
                    var result = await _coords.GetCoordsAsync(newStop.Name);
                    if (!result.Success)
                    {
                        _logger.LogError(result.Message);
                    }
                    else
                    {
                        newStop.Latitude = result.Latitude;
                        newStop.Longitude = result.Longitude;
                        _repository.AddStop(tripName, newStop);

                        if (await _repository.SaveChangesAsync())
                        {
                            return Created($"api/trips/{tripName}/stops/{newStop.Name}", Mapper.Map<StopViewModel>(newStop));
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                _logger.LogError("Failed to save new stop {0}", ex);
            }

            return BadRequest("Failed to save new stop");
        }
    }
}
