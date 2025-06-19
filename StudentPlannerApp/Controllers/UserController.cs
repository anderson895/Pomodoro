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
            Session.Clear(); 
            Session.Abandon(); 

            return RedirectToAction("Login", "User"); 
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
                    cmd.Parameters.AddWithValue("@Password", user.Password);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                // Add this line to pass success message
                TempData["SuccessMessage"] = "Registration successful! Please login.";

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
                cmd.Parameters.AddWithValue("@Password", Password); 

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    // Successful login
                    Session["UserId"] = reader["Id"];
                    Session["Fullname"] = reader["Fullname"];
                    conn.Close();
                    return RedirectToAction("Index", "Home"); 
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
