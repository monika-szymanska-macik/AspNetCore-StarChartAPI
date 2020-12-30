using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route(""), ApiController]
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
            var celestialObject = _context.CelestialObjects.Find(id);
            if (celestialObject == null)
                return NotFound();
            celestialObject.Satellites = _context.CelestialObjects.Where(e => e.OrbitedObjectId == id).ToList();
            return Ok(celestialObject);
        }
        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {

            var celestialObjects = _context.CelestialObjects.Where(e => e.Name == name).ToList();
            if (!celestialObjects.Any())
                return NotFound();
            foreach (var c in celestialObjects)
            {
                c.Satellites = _context.CelestialObjects.Where(e => e.OrbitedObjectId == c.Id).ToList();
            }
            return Ok(celestialObjects);
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects.ToList();
            foreach (var c in celestialObjects)
            {
                c.Satellites = _context.CelestialObjects.Where(e => e.OrbitedObjectId == c.Id).ToList();
            }
            return Ok(celestialObjects);
        }
        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject a)
        {
            _context.CelestialObjects.Add(a);
            _context.SaveChanges();

            return CreatedAtRoute("GetById", new { id = a.Id }, a);
        }
        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            var existingObject = _context.CelestialObjects.Find(id);
            if (existingObject == null)
                return NotFound();
            existingObject.Name = celestialObject.Name;
            existingObject.OrbitalPeriod = existingObject.OrbitalPeriod;
            existingObject.OrbitedObjectId = existingObject.OrbitedObjectId;
            _context.CelestialObjects.Update(existingObject);
            _context.SaveChanges();
            return NoContent();

        }
        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var namedObject = _context.CelestialObjects.Find(id);
            if (namedObject == null)
                return NotFound();
            namedObject.Name = name;
            _context.CelestialObjects.Update(namedObject);
            _context.SaveChanges();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var listObjects = _context.CelestialObjects.Where(o => o.Id == id || o.OrbitedObjectId == id).ToList();
            if (!listObjects.Any())
                return NotFound();
            _context.CelestialObjects.RemoveRange(listObjects);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
