﻿using System.Diagnostics;
using FitnessConnect.Areas.Identity.Data;
using FitnessConnect.Interfaces;
using FitnessConnect.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FitnessConnect.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICommonRepository _commonrepo;
        
        public HomeController(ILogger<HomeController> logger, ICommonRepository commonrepo)
        {
            _commonrepo = commonrepo;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult ContactUs()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ContactUs(Contact contact)
        {
            try
            {
                _commonrepo.ContactUsSubmission(contact);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _commonrepo.AddLogger("Home", "ContactUs", ex.Message);
                return null;
            }
        }

        public IActionResult AboutUs()
        {
            return View();
        }

        public async Task<IActionResult> GenerateExercise(string name, string type, string muscle, string difficulty,
            CancellationToken token)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("X-Api-Key", "kjObhgIO6kctyWA/DIdERw==m6lqeAwzTqk1jJZD");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

            var valueQuery = new List<string>();

            if (name != "")
            {
                valueQuery.Add("name=" + name);
            }

            if (type != "")
            {
                valueQuery.Add("type=" + type);
            }

            if (muscle != "")
            {
                valueQuery.Add("muscle=" + muscle);
            }

            if (difficulty != "")
            {
                valueQuery.Add("difficulty=" + difficulty);
            }

            var query = string.Join("&", valueQuery);
            
            var response = await httpClient.GetStringAsync(
                "https://api.api-ninjas.com/v1/exercises?" + query, token);
            var data = JsonConvert.DeserializeObject<List<ExerciseModel>>(response);
            if (data != null) ViewBag.ExerciseData = data;
            return View();
        }
    }
}

public class ExerciseModel
{
    public string Name { get; set; }
    public string Type { get; set; }
    public string Muscle { get; set; }
    public string Equipment { get; set; }
    public string Difficulty { get; set; }
    public string Instructions { get; set; }
}