using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{

    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.FirstOrDefault(c => c.Id == id);
            if (celestialObject == null)
                return NotFound();

            celestialObject.Satellites
                = _context.CelestialObjects.Where(c => c.OrbitedObjectId == celestialObject.Id).ToList();

            return Ok(celestialObject);
        }
        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects.Where(c => c.Name == name).ToList();
            if (celestialObjects == null || !celestialObjects.Any())
                return NotFound();

            for (int i = 0; i < celestialObjects.Count(); i++)
            {
                celestialObjects[i].Satellites =
                    _context.CelestialObjects.Where(c => c.OrbitedObjectId == celestialObjects[i].Id).ToList();
            }
            return Ok(celestialObjects);
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects.ToList();
            for (int i = 0; i < celestialObjects.Count(); i++)
            {
                celestialObjects[i].Satellites =
                    _context.CelestialObjects.Where(c => c.OrbitedObjectId == celestialObjects[i].Id).ToList();
            }
            return Ok(celestialObjects);
        }
        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();
            return CreatedAtRoute(nameof(GetById), new { id = celestialObject.Id }, celestialObject);
        }
        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject updatedCelestialObject)
        {
            var currentCelestialObject = _context.CelestialObjects.FirstOrDefault(c => c.Id == id);
            if (currentCelestialObject == null)
                return NotFound();

            currentCelestialObject.Name             = updatedCelestialObject.Name;
            currentCelestialObject.OrbitalPeriod    = updatedCelestialObject.OrbitalPeriod;
            currentCelestialObject.OrbitedObjectId  = updatedCelestialObject.OrbitedObjectId;

            _context.CelestialObjects.Update(currentCelestialObject);
            _context.SaveChanges();
            return NoContent();
        }
        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var currentCelestialObject = _context.CelestialObjects.FirstOrDefault(c => c.Id == id);
            if (currentCelestialObject == null)
                return NotFound();

            currentCelestialObject.Name = name;

            _context.CelestialObjects.Update(currentCelestialObject);
            _context.SaveChanges();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var celestialObjects = _context.CelestialObjects.Where(c => 
            c.Id == id || c.OrbitedObjectId == id).ToArray();
            if (celestialObjects == null || !celestialObjects.Any())
                return NotFound();

            _context.CelestialObjects.RemoveRange(celestialObjects);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
