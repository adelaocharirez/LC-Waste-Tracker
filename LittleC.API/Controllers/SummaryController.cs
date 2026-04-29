// ============================================================
// SummaryController.cs
// Handles dashboard data, history, and S3 photo upload
// Routes:
//   GET  /api/summary/today        → today's full waste summary
//   GET  /api/summary/history      → last 30 days of daily totals
//   POST /api/summary/upload-photo → uploads end-of-night photo to S3
// ============================================================

using LittleC.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LittleC.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SummaryController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public SummaryController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET /api/summary/today
        // Returns total waste value and breakdowns by item and reason
        [HttpGet("today")]
        public async Task<IActionResult> GetTodaySummary()
        {
            var today = DateTime.UtcNow.Date;
            var logs = await _context.WasteLogs
                .Include(l => l.MenuItem)
                .Include(l => l.WasteReason)
                .Where(l => l.LoggedAt.Date == today)
                .ToListAsync();

            var summary = new
            {
                TotalWasteValue = logs.Sum(l => l.TotalCost),
                TotalItems = logs.Sum(l => l.Quantity),
                TotalEntries = logs.Count,
                ByItem = logs.GroupBy(l => l.MenuItem.Name)
                    .Select(g => new
                    {
                        Item = g.Key,
                        Total = g.Sum(l => l.TotalCost),
                        Quantity = g.Sum(l => l.Quantity)
                    }),
                ByReason = logs.GroupBy(l => l.WasteReason.Reason)
                    .Select(g => new
                    {
                        Reason = g.Key,
                        Total = g.Sum(l => l.TotalCost),
                        Quantity = g.Sum(l => l.Quantity)
                    })
            };

            return Ok(summary);
        }

        // GET /api/summary/history
        // Returns last 30 days of daily waste totals
        [HttpGet("history")]
        public async Task<IActionResult> GetHistory()
        {
            var history = await _context.WasteLogs
                .Include(l => l.MenuItem)
                .GroupBy(l => l.LoggedAt.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    TotalWasteValue = g.Sum(l => l.TotalCost),
                    TotalItems = g.Sum(l => l.Quantity),
                    TotalEntries = g.Count()
                })
                .OrderByDescending(g => g.Date)
                .Take(30)
                .ToListAsync();

            return Ok(history);
        }

        // POST /api/summary/upload-photo
        // Uploads end-of-night shift photo to AWS S3
        // Returns the public URL of the uploaded photo
        [HttpPost("upload-photo")]
        public async Task<IActionResult> UploadPhoto([FromForm] IFormFile photo)
        {
            if (photo == null || photo.Length == 0)
                return BadRequest(new { message = "No photo provided" });

            var s3 = new S3Service(_configuration);
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(photo.FileName)}";

            using var stream = photo.OpenReadStream();
            var url = await s3.UploadPhotoAsync(stream, fileName, photo.ContentType);

            return Ok(new { url, message = "Photo uploaded successfully" });
        }
    }
}