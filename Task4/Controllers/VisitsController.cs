using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Task4.models;

namespace Task4.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VisitsController : ControllerBase
    {
        // List of visits
        public static List<Visit> Visits = new List<Visit>();

        //Retrieve all visits
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(Visits);
        }

        //Retrieve visists with specific animal by the Id
        [HttpGet("{animalId}")]
        public IActionResult Get(Guid animalId)
        {
            var visit = Visits.FirstOrDefault(a => a.animalId == animalId);
            if (visit == null) return NotFound();
            return Ok(visit);
        }

        //Add a new visit
        [HttpPost]
        public IActionResult Post(Visit visit)
        {
            Visits.Add(visit);
            return Ok(Visits);
        }

        //Edit visit
        [HttpPut]
        public IActionResult Put(Guid id, Visit updatedVisit) {
            var index = Visits.FindIndex(a => a.id == id);
            if (index == -1) return NotFound();
            Visits[index] = updatedVisit;
            return NoContent();
        }

        //Delte visit
        [HttpDelete]
        public IActionResult Delete(Guid id)
        {
            var index = Visits.FindIndex(a => a.id == id);
            if (index == -1) return NotFound();
            Visits.RemoveAt(index);
            return NoContent();
        }
    }
}