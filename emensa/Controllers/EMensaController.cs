using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Cryptography;
using emensa.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;

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
            return View(Service.GetProductsModel());
        }

        public IActionResult Ingredients()
        {
            var ingredients = Service.GetIngredients();

            return View(ingredients);
        }

        public IActionResult Details(uint id)
        {
            return View(Service.GetDetailsModel(id));
        }

        public IActionResult Login()
        {
            if (HttpContext.Request.Method.ToLower() == "get")
            {
                return View(new LoginModel());
            }

            var username = HttpContext.Request.Form["username"];
            var password = HttpContext.Request.Form["password"];
            var user = Service.GetUserLogin(username);
            if (user.UserError)
            {
                return View(user);
            }

            if (Service.VerifyPassword(password, user.Salt, user.Hash))
            {
                HttpContext.Session.SetString("user", user.Username);
            }
            else
            {
                user.PasswordError = true;
                return View(user);
            }

            return RedirectToAction("Index");
        }
    }
}