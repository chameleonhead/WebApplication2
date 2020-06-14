using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq;
using WebApplication2.Data;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MessageContext _context;

        public HomeController(ILogger<HomeController> logger, MessageContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("messages")]
        public IActionResult GetMessage()
        {
            return Ok(_context.Messages.ToList());
        }

        [HttpPost("messages")]
        public IActionResult PostMessage([FromForm(Name = "message")] string message)
        {
            _context.Messages.Add(new Message()
            {
                Value = message
            });
            _context.SaveChanges();
            return Ok();
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
