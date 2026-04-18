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
    }
}
