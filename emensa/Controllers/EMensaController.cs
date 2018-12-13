using System;
using System.Text.RegularExpressions;
using emensa.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace emensa.Controllers
{
    public class EMensaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Products()
        {
            return View(new ProductsModel());
        }

        public IActionResult Ingredients()
        {
            return View(Ingredient.GetAll());
        }

        public IActionResult Details(uint id)
        {
            var username = HttpContext.Session.GetString("user");
            User user = null;
            if (username != null)
            {
                var role = Models.User.GetRole(username);
                switch (role)
                {
                    case "no role":
                        user = new User {Username = username};
                        break;
                    case "guest":
                        user = new Guest {Username = username};
                        break;
                    case "employee":
                        user = new Employee {Username = username};
                        break;
                    case "student":
                        user = new Student {Username = username};
                        break;
                    case "member":
                        user = new Member {Username = username};
                        break;
                }
            }

            return View(new DetailsModel(id, user));
        }

        public IActionResult Login()
        {
            if (HttpContext.Request.Method.ToLower() == "get")
            {
                return View(new LoginModel());
            }

            var username = HttpContext.Request.Form["username"];
            var password = HttpContext.Request.Form["password"];
            var user = new LoginModel(username);
            if (user.UserError)
            {
                return View(user);
            }

            if (Service.VerifyPassword(password, user.Salt, user.Hash))
            {
                HttpContext.Session.SetString("user", user.Username);
                HttpContext.Session.SetString("role", Models.User.GetRole(user.Username));
            }
            else
            {
                user.PasswordError = true;
                return View(user);
            }

            return RedirectToAction("Index");
        }

        public IActionResult Register()
        {
            if (HttpContext.Request.Method.ToLower() == "get")
            {
                return View(new RegisterModel());
            }
            var username = HttpContext.Request.Form["username"];
            var email = HttpContext.Request.Form["email"];
            var password = HttpContext.Request.Form["password"];
            var passwordRepeat = HttpContext.Request.Form["password-repeat"];
            var lastName = HttpContext.Request.Form["last_name"];
            var firstName = HttpContext.Request.Form["first_name"];
            var birthday = HttpContext.Request.Form["birthday"];
            var role = HttpContext.Request.Form["role"];
            if (password != passwordRepeat)
            {
                return View(new RegisterModel
                {
                    PasswordError = true
                });
            }
            if (!Regex.IsMatch(email, @"[\w\d\.]+@[-\d\w\.]+\.\w+"))
            {
                Console.Out.WriteLine("email = {0}", email);
                return View(new RegisterModel
                {
                    EmailError = true
                });
            }
            var user = new User
            {
                Username = username,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                Birthday = DateTime.Parse(birthday)
            };
            bool userError;
            switch (role)
            {
                    case "guest":
                        var guest = new Guest(user);
                        guest.Reason = HttpContext.Request.Form["reason"];
                        guest.ValidUntil = DateTime.Parse(HttpContext.Request.Form["valid_until"]);
                        userError = Models.User.RegisterUser(guest, password);
                        break;
                    case "student":
                        var student = new Student(user);
                        student.MatriculationNumber =
                            Convert.ToUInt32(HttpContext.Request.Form["matriculation_number"]);
                        Enum.TryParse(HttpContext.Request.Form["major"], out student.Major);
                        userError = Models.User.RegisterUser(student, password);
                        break;
                    case "employee":
                        var employee = new Employee(user);
                        employee.Office = HttpContext.Request.Form["office"];
                        employee.PhoneNumber = HttpContext.Request.Form["phone_number"];
                        userError = Models.User.RegisterUser(employee, password);
                        break;
                    default:
                        return View(new RegisterModel
                        {
                            RoleError = true
                        });
            }

            if (!userError)
            {
                return View(new RegisterModel
                {
                    UsernameError = true
                });
            }
            HttpContext.Session.SetString("user", username);
            HttpContext.Session.SetString("role", role);
            return RedirectToAction("Index");
        }

        public IActionResult Impressum()
        {
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("user");
            HttpContext.Session.Remove("role");
            return View("Index");
        }
    }
}