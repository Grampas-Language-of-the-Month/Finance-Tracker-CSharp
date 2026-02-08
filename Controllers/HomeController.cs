using System.Diagnostics;
using finance_tracker.Models;
using Microsoft.AspNetCore.Mvc;
using finance_tracker.Data;
using finance_tracker.Models;
using Microsoft.AspNetCore.Mvc;

namespace finance_tracker.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0) return View(new List<Transaction>());

            var newTransactions = new List<Transaction>();

            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var values = line.Split(',')
                        .Select(v => v.Trim('"'))
                        .ToArray();

                    var transaction = new Transaction
                    (
                        DateTime.Parse(values[0]),
                        values[4],
                        decimal.Parse(values[1])
                    );

                    bool exists = _context.Transactions
                        .Any(t => t.Hash == transaction.Hash);

                    if (!exists)
                    {
                        _context.Transactions.Add(transaction);
                        newTransactions.Add(transaction);
                    }
                }
            }

            await _context.SaveChangesAsync();

            return View(newTransactions);
        }

        public IActionResult Reports()
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
