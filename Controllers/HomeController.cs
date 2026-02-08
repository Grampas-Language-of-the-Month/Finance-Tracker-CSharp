using finance_tracker.Data;
using finance_tracker.Models;
using finance_tracker.Models;
using finance_tracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace finance_tracker.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly CategoryService _categoryService;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, CategoryService categoryService)
        {
            _logger = logger;
            _context = context;
            _categoryService = categoryService;
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

                    var values = line.Trim()
                        .Split(',')
                        .Select(v => v.Trim().Trim('"'))
                        .ToArray();

                    var transaction = new Transaction
                    (
                        DateTime.Parse(values[0]),
                        values[4],
                        decimal.Parse(values[1]),
                        _categoryService.GetCategory(values[4])
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
            var transactions = _context.Transactions.ToList();

            // Grouping for the charts
            var categorySummary = transactions
                .GroupBy(t => new { t.Direction, t.Category })
                .Select(g => new {
                    Label = $"{g.Key.Direction}: {g.Key.Category}",
                    Total = Math.Abs(g.Sum(t => t.Amount)),
                    Direction = g.Key.Direction
                })
                .ToList();

            return View(categorySummary);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
