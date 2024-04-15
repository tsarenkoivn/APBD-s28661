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
        public IActionResult Get => Ok(Visits);

        //Retrieve visists with specific animal by the Id
        [HttpGet("{animalId}")]
        public IActionResult Get(Guid id)
        {
            var visit = Visits.FirstOrDefault(a => a.animalId == animalID);
            if (visit == null) return NotFound();
            return Ok(Visit);
        }

        //Add a new visit
        [HttpPost]
        public IActionResult Post(Visit visit)
        {
            Visits.Add(visit);
            return CreatedAtAction(nameof(Get), new { id == visit.id }, visit)
        }

        //Edit visit
        [HttpPut]
        public IActionResult Put(Guid id, Visit updatedVisit) {
            var index = animals.FindIndex(a => a.Id == id);
            if (index == -1) return NotFound();
            animals[index] = updatedAnimal;
            return NoContent();
        }

        //Delte visit
        [HttpDelete]
        public IActionResult Delete(Guid id)
        {
            var index = animals.FindIndex(a => a.Id == id);
            if (index == -1) return NotFound();
            animals.RemoveAt(index);
            return NoContent();
        }
    }
}