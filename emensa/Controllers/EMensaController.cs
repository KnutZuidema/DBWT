using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using emensa.DataModels;
using emensa.Utility;
using emensa.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
            return View(new Products());
        }

        public IActionResult Ingredients()
        {
            return View(new Ingredients());
        }

        public IActionResult Details(int id)
        {
            var username = HttpContext.Session.GetString("user");

            return View(new Details(id, EmensaContext.GetRole(username)));
        }

        public IActionResult Login()
        {
            if (Request.Method.ToLower() == "get")
            {
                return View(new Login());
            }

            var username = HttpContext.Request.Form["username"];
            var password = HttpContext.Request.Form["password"];

            var userExists = EmensaContext.DoesUserExist(username);
            var isActivated = EmensaContext.IsUserActivated(username);
            var isPasswordCorrect = EmensaContext.IsPasswordCorrect(username, password);
            if (!userExists || !isActivated || !isPasswordCorrect)
            {
                return View(new Login
                {
                    UserExists = userExists,
                    IsActivated = isActivated,
                    IsPasswordValid = isPasswordCorrect
                });
            }

            HttpContext.Session.SetString("user", username);
            HttpContext.Session.SetString("role", EmensaContext.GetRole(username).ToString());
            return RedirectToAction("Index");
        }

        public IActionResult Register()
        {
            if (HttpContext.Request.Method.ToLower() == "get")
            {
                return View(new Register());
            }

            var username = Request.Form["username"];
            var email = Request.Form["email"];
            var password = Request.Form["password"];
            var passwordRepeat = Request.Form["password-repeat"];
            var lastName = Request.Form["last_name"];
            var firstName = Request.Form["first_name"];
            var birthday = DateTime.Parse(Request.Form["birthday"]);
            Enum.TryParse(Request.Form["role"], true, out Role role);
            var doPasswordsMatch = password == passwordRepeat;
            var isValidEmail = Regex.IsMatch(email, @"[\w\d\.]+@[-\d\w\.]+\.\w+");
            var doesUserExist = EmensaContext.DoesUserExist(username);
            if (!doPasswordsMatch || !isValidEmail || doesUserExist)
            {
                return View(new Register
                {
                    DoPasswordsMatch = doPasswordsMatch,
                    IsEmailValid = isValidEmail,
                    DoesUserExist = doesUserExist
                });
            }

            var (hash, salt) = PasswordStorage.CreateHash(password);
            var user = new User
            {
                LastName = lastName,
                FirstName = firstName,
                Username = username,
                Salt = salt,
                Hash = hash,
                Birthday = birthday,
                Email = email
            };
            switch (role)
            {
                case Role.Employee:
                    var phoneNumber = Request.Form["phone_number"];
                    var office = Request.Form["office"];
                    ViewModels.Register.RegisterEmployee(new Employee
                    {
                        Member = new Member
                        {
                            User = user
                        },
                        PhoneNumber = phoneNumber,
                        Office = office
                    });
                    break;
                case Role.Student:
                    var matriculationNumber = Convert.ToInt32(Request.Form["matriculation_number"]);
                    var major = Request.Form["major"];
                    ViewModels.Register.RegisterStudent(new Student
                    {
                        Member = new Member
                        {
                            User = user
                        },
                        MatriculationNumber = matriculationNumber,
                        Major = major
                    });
                    break;
                default:
                    var reason = Request.Form["reason"];
                    var validUntil = DateTime.Parse(Request.Form["valid_until"]);
                    ViewModels.Register.RegisterGuest(new Guest
                    {
                        User = user,
                        Reason = reason,
                        ValidUntil = validUntil
                    });
                    break;
            }
            Response.Cookies.Append("new_register", "");
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

        public IActionResult Order()
        {
            var cookie = Request.Cookies["orders"];
            var orders = JsonConvert.DeserializeObject<ViewModels.Order>(cookie ?? "") 
                         ?? new ViewModels.Order();
            
            if (Request.Method.ToLower() == "get")
            {
                return View(orders);
            }

            var username = HttpContext.Session.GetString("user");
            if (username is null)
            {
                return RedirectToAction("Login");
            }

            Response.Cookies.Delete("orders");
            return RedirectToAction(ViewModels.Order.PlaceOrder(orders, username) ? "Index" : "Order");
        }

        public IActionResult Orders()
        {
            Request.Headers.TryGetValue("X-Authorize", out var code);
            if (code != "secretcode")
            {
                return Unauthorized();
            }

            using (var db = new EmensaContext())
            {
                var orders = (from order in db.Order
                    where order.CollectedAt < DateTime.Now.AddHours(1)
                    join user in db.User on order.UserId equals user.Id
                    select new
                    {
                        User = new
                        {
                            user.FirstName,
                            user.LastName,
                            user.Username,
                            user.Email
                        },
                        order.CollectedAt,
                        OrderId = order.Id,
                        Meals = (from relation in db.OrderMealRelation
                            where relation.OrderId == order.Id
                            join meal in db.Meal on relation.MealId equals meal.Id
                            join category in db.Category on meal.CategoryId equals category.Id
                            select new
                            {
                                relation.Amount,
                                Category = category.Name,
                                meal.Name,
                                meal.Stock
                            }).ToList()
                    }).ToList();
                return Json(orders);
            }
        }
    }
}