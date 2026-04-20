using EventNexus.Models;
using EventNexus.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace EventNexus.Controllers
{
    public class ManagerController : Controller
    {

        private readonly EventNexusContext Dbcontext;
        private readonly JwtService _jwtService;

        public ManagerController(EventNexusContext context, JwtService jwtService)
        {
            Dbcontext = context;
            _jwtService = jwtService;
        }
        string connectionstring = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=EventNexus;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

        [Authorize(Roles = "Manager")]
        [HttpGet]
        public async Task<IActionResult> ManagerDashboard()
        {
            var data = await Dbcontext.Events.Include(e => e.Venue).ToListAsync();
            return View(data);
        }

        [Authorize(Roles = "Manager")]
        [HttpGet]
        public IActionResult CreateEvent()
        {
            return View();
        }
        [Authorize(Roles = "Manager")]
        [HttpPost]
        public IActionResult CreateEvent(Event temp,Venue venu)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            try
            {
                var newvenu = new Venue
                {
                    Name = venu.Name,
                    Location = venu.Location,
                    Capacity = venu.Capacity
                };
                Dbcontext.Venues.Add(newvenu);
                Dbcontext.SaveChanges();
                temp.VenueId = newvenu.VenueId;

                var newevent = new Event
                {
                    Title=temp.Title,
                    Description=temp.Description,
                    EventDate=temp.EventDate,
                    Status=temp.Status,
                    VenueId=temp.VenueId
                   
                };
                Dbcontext.Events.Add(newevent);
                Dbcontext.SaveChanges();

                return RedirectToAction("ManagerDashboard", "Manager");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Database Error: " + ex.Message);
                return View();
            }
        }

        public async Task<IActionResult> Events()
        {
            var data = await Dbcontext.Events
                .Include(e => e.Venue)
                .ToListAsync();

            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetEvent(int id)
        {
            var data = await Dbcontext.Events
                .Include(e => e.Venue)   // 🔥 REQUIRED
                .Where(e => e.EventId == id)
                .Select(e => new
                {
                    e.EventId,
                    e.Title,
                    e.Description,
                    e.EventDate,
                    e.Status,
                    Venue = new
                    {
                        e.Venue.Name,
                        e.Venue.Location,
                        e.Venue.Capacity
                    }
                })
                .FirstOrDefaultAsync();

            if (data == null)
                return NotFound();
            
            return Json(data);   // 🔥 IMPORTANT
        }
        [HttpPost]
        public async Task<IActionResult> UpdateEvent(Event model)
        {
            var eventData = await Dbcontext.Events
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(e => e.EventId == model.EventId);

            if (eventData == null)
                return NotFound();

            // EVENT UPDATE
            eventData.Title = model.Title;
            eventData.Description = model.Description;
            eventData.EventDate = model.EventDate;
            eventData.Status = model.Status;

            // VENUE UPDATE
            if (eventData.Venue != null)
            {
                eventData.Venue.Name = Request.Form["VenueName"];
                eventData.Venue.Location = Request.Form["VenueLocation"];
                eventData.Venue.Capacity = int.Parse(Request.Form["VenueCapacity"]);
            }

            await Dbcontext.SaveChangesAsync();

            return Ok();
        }
    }
}
