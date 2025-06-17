using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;
using StudentPlannerApp.Models;

namespace StudentPlannerApp.Controllers
{
    public class TimerController : Controller
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;


        [HttpPost]
        public ActionResult DeletePomodoro(int id)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM PomodoroSchedule WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        return Json(new { success = true });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Schedule not found." });
                    }
                }
            }
        }


        [HttpPost]
        public ActionResult UpdatePomodoro(PomodoroSchedule schedule)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"UPDATE PomodoroSchedule
                         SET Activity = @Activity, 
                             PomodoroDuration = @PomodoroDuration, 
                             BreakDuration = @BreakDuration,
                             Status = @Status
                         WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Activity", schedule.Activity);
                    cmd.Parameters.AddWithValue("@PomodoroDuration", schedule.PomodoroDuration);
                    cmd.Parameters.AddWithValue("@BreakDuration", schedule.BreakDuration);
                    cmd.Parameters.AddWithValue("@Status", schedule.Status);
                    cmd.Parameters.AddWithValue("@Id", schedule.Id);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    conn.Close();

                    if (rowsAffected > 0)
                    {
                        return Json(new { success = true });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Schedule not found." });
                    }
                }
            }
        }





        [HttpPost]
        public ActionResult SetSchedule(DateTime startTime, DateTime endTime, string activity)
        {
            if (Session["UserId"] == null)
            {
                TempData["Message"] = "You must be logged in to set schedules.";
                return RedirectToAction("Login", "User");
            }

            int userId = Convert.ToInt32(Session["UserId"]);

            try
            {
                if (endTime <= startTime)
                {
                    TempData["Message"] = "End time must be after start time.";
                    return RedirectToAction("Index");
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string overlapQuery = @"
                        SELECT COUNT(*) FROM PomodoroSchedule
                        WHERE UserId = @UserId
                        AND (@NewStart < EndTime AND @NewEnd > StartTime)";

                    using (SqlCommand overlapCmd = new SqlCommand(overlapQuery, conn))
                    {
                        overlapCmd.Parameters.AddWithValue("@UserId", userId);
                        overlapCmd.Parameters.AddWithValue("@NewStart", startTime);
                        overlapCmd.Parameters.AddWithValue("@NewEnd", endTime);

                        int overlapCount = (int)overlapCmd.ExecuteScalar();
                        if (overlapCount > 0)
                        {
                            TempData["Message"] = "Schedule conflicts with an existing one.";
                            return RedirectToAction("Index");
                        }
                    }

                    string insertSql = @"
                        INSERT INTO PomodoroSchedule (StartTime, EndTime, Activity, CreatedAt, UserId)
                        VALUES (@StartTime, @EndTime, @Activity, @CreatedAt, @UserId)";

                    using (SqlCommand insertCmd = new SqlCommand(insertSql, conn))
                    {
                        insertCmd.Parameters.AddWithValue("@StartTime", startTime);
                        insertCmd.Parameters.AddWithValue("@EndTime", endTime);
                        insertCmd.Parameters.AddWithValue("@Activity", activity);
                        insertCmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                        insertCmd.Parameters.AddWithValue("@UserId", userId);

                        insertCmd.ExecuteNonQuery();
                    }
                }

                TempData["Message"] = "Schedule reminder saved successfully!";
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Failed to save schedule: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult SetPomodoroSchedule(DateTime startTime, int pomodoroDuration, int breakDuration, string activity)
        {
            if (Session["UserId"] == null)
            {
                TempData["Message"] = "You must be logged in to set Pomodoro schedules.";
                return RedirectToAction("Login", "User");
            }

            int userId = Convert.ToInt32(Session["UserId"]);
            DateTime endTime = startTime.AddMinutes(pomodoroDuration);
            DateTime createdAt = DateTime.Now;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string insertSql = @"
                        INSERT INTO PomodoroSchedule 
                        (StartTime, EndTime, Activity, CreatedAt, UserId, PomodoroDuration, BreakDuration)
                        VALUES 
                        (@StartTime, @EndTime, @Activity, @CreatedAt, @UserId, @PomodoroDuration, @BreakDuration)";

                    using (SqlCommand cmd = new SqlCommand(insertSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@StartTime", startTime);
                        cmd.Parameters.AddWithValue("@EndTime", endTime);
                        cmd.Parameters.AddWithValue("@Activity", activity);
                        cmd.Parameters.AddWithValue("@CreatedAt", createdAt);
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@PomodoroDuration", pomodoroDuration);
                        cmd.Parameters.AddWithValue("@BreakDuration", breakDuration);

                        cmd.ExecuteNonQuery();
                    }
                }

                TempData["Message"] = "Pomodoro Task created and saved successfully!";
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Failed to save Pomodoro Task: " + ex.Message;
            }

            return RedirectToAction("Pomodoro");
        }

        [HttpGet]
        public ActionResult Pomodoro()
        {
            ViewBag.StartTime = TempData["StartTime"];
            ViewBag.PomodoroDuration = TempData["PomodoroDuration"];
            ViewBag.BreakDuration = TempData["BreakDuration"];
            ViewBag.Activity = TempData["Activity"];
            ViewBag.Message = TempData["Message"];
            return View();
        }

        [HttpPost]
        public ActionResult LogPomodoroActivity(string notes)
        {
            TempData["Message"] = "Pomodoro activity notes saved!";
            return RedirectToAction("Pomodoro");
        }








    }
}
