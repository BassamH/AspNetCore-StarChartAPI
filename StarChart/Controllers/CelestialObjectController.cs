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
        public CelestialObjectController(ApplicationDbContext  context)
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

            for(int i=0; i< celestialObjects.Count(); i++)
            {
                celestialObjects[i].Satellites  = 
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
    }
}
