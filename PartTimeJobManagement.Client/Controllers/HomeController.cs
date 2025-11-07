using Microsoft.AspNetCore.Mvc;
using PartTimeJobManagement.Client.Models;
using PartTimeJobManagement.Client.Services;
using System.Diagnostics;

namespace PartTimeJobManagement.Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IJobService _jobService;

        public HomeController(ILogger<HomeController> logger, IJobService jobService)
        {
            _logger = logger;
            _jobService = jobService;
        }

        public async Task<IActionResult> Index()
        {
            // Get latest active jobs for homepage
            var jobs = await _jobService.GetActiveJobsAsync();
            var latestJobs = jobs?.Take(6) ?? Enumerable.Empty<Models.DTOs.JobResponseDTO>();
            
            return View(latestJobs);
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
    }
}
