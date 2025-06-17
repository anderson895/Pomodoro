using System.Web.Mvc;
using StudentPlannerApp.Models;
using System.Data.SqlClient;
using System.Configuration;

namespace StudentPlannerApp.Controllers
{
    public class UserController : Controller
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        // GET: User/Register
        public ActionResult Register()
        {
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear(); // Clear all session data
            Session.Abandon(); // Optional: Abandon the session

            return RedirectToAction("Login", "User"); // Redirect to Login page
        }


        // POST: User/Register
        [HttpPost]
        public ActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sql = "INSERT INTO Users (Fullname, Email, Password) VALUES (@Fullname, @Email, @Password)";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@Fullname", user.Fullname);
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@Password", user.Password); // For security: hash this in real apps

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                return RedirectToAction("Login");
            }

            return View(user);
        }

        // GET: User/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: User/Login
        [HttpPost]
        public ActionResult Login(string Email, string Password)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "SELECT Id, Fullname FROM Users WHERE Email = @Email AND Password = @Password";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Email", Email);
                cmd.Parameters.AddWithValue("@Password", Password); // Insecure for real apps - hash this

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    // Successful login
                    Session["UserId"] = reader["Id"];
                    Session["Fullname"] = reader["Fullname"];
                    conn.Close();
                    return RedirectToAction("Index", "Home"); // or another page after login
                }
                else
                {
                    ViewBag.Error = "Invalid email or password.";
                }
            }

            return View();
        }
    }
}
