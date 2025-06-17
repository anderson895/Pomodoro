using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;
using StudentPlannerApp.Models;

namespace StudentPlannerApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return View(new List<PomodoroSchedule>()); // empty model if not logged in

            int userId = Convert.ToInt32(Session["UserId"]);
            List<PomodoroSchedule> userSchedules = new List<PomodoroSchedule>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM PomodoroSchedule WHERE UserId = @UserId ORDER BY StartTime ASC";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    userSchedules.Add(new PomodoroSchedule
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        StartTime = Convert.ToDateTime(reader["StartTime"]),
                        EndTime = Convert.ToDateTime(reader["EndTime"]),
                        BreakDuration = Convert.ToInt32(reader["BreakDuration"]),
                        PomodoroDuration = Convert.ToInt32(reader["PomodoroDuration"]),
                        Activity = reader["Activity"].ToString(),
                        CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                    });
                }
            }

            return View(userSchedules); // ✅ pass model directly
        }

    }
}
