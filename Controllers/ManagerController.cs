using EventNexus.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EventNexus.Controllers
{
    public class ManagerController : Controller
    {
        string connectionstring = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=EventNexus;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

        [Authorize(Roles = "Manager")]
        [HttpGet]
        public IActionResult ManagerDashboard()
        {
            return View();
        }

        [Authorize(Roles = "Manager")]
        [HttpGet]
        public IActionResult CreateEvent()
        {
            return View();
        }
        [Authorize(Roles = "Manager")]
        [HttpPost]
        public IActionResult CreateEvent(Event temp, string VenueName, string Location, int Capacity)
        {
            if (!ModelState.IsValid)
            {
                return View(temp);
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionstring))
                {
                    conn.Open();

                    // 🔹 1. Insert Venue first
                    string venueQuery = @"INSERT INTO Venues (Name, Location, Capacity)
                                 OUTPUT INSERTED.VenueId
                                 VALUES (@Name, @Location, @Capacity)";

                    int venueId;

                    using (SqlCommand cmd = new SqlCommand(venueQuery, conn))
                    {
                        cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = VenueName;
                        cmd.Parameters.Add("@Location", SqlDbType.NVarChar).Value = Location;
                        cmd.Parameters.Add("@Capacity", SqlDbType.Int).Value = Capacity;

                        venueId = (int)cmd.ExecuteScalar(); // get new VenueId
                    }

                    // 🔹 2. Insert Event using that VenueId
                    string eventQuery = @"INSERT INTO Events 
                                 (Title, Description, EventDate, VenueId, Status)
                                 VALUES (@Title, @Description, @EventDate, @VenueId, @Status)";

                    using (SqlCommand cmd = new SqlCommand(eventQuery, conn))
                    {
                        cmd.Parameters.Add("@Title", SqlDbType.NVarChar).Value = temp.Title;
                        cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value =
                            string.IsNullOrEmpty(temp.Description) ? DBNull.Value : temp.Description;

                        cmd.Parameters.Add("@EventDate", SqlDbType.DateTime).Value = temp.EventDate;
                        cmd.Parameters.Add("@VenueId", SqlDbType.Int).Value = venueId;
                        cmd.Parameters.Add("@Status", SqlDbType.NVarChar).Value = temp.Status;

                        cmd.ExecuteNonQuery();
                    }
                }

                return RedirectToAction("ManagerDashboard", "Manager");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Database Error: " + ex.Message);
                return View(temp);
            }
        }
    }
}
