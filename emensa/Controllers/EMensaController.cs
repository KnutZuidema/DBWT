using emensa.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace emensa.Controllers
{
    public class EMensaController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.User = HttpContext.Session.GetString("user");
            return View();
        }

        public IActionResult Products()
        {
            ViewBag.User = HttpContext.Session.GetString("user");
            return View(new ProductsModel());
        }

        public IActionResult Ingredients()
        {
            ViewBag.User = HttpContext.Session.GetString("user");
            return View(Ingredient.GetAll());
        }

        public IActionResult Details(uint id)
        {
            ViewBag.User = HttpContext.Session.GetString("user");
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
            ViewBag.User = HttpContext.Session.GetString("user");
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
            ViewBag.User = HttpContext.Session.GetString("user");
            return View();
        }

        public IActionResult Impressum()
        {
            ViewBag.User = HttpContext.Session.GetString("user");
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