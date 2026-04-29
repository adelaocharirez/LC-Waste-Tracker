// Core controller — handles logging waste, today's logs,
// and deleting individual entries
// Routes:
//   GET    /api/wastelog/reasons  → returns all waste reasons
//   GET    /api/wastelog/today    → returns all waste logs for today
//   POST   /api/wastelog          → logs a new waste entry
//   DELETE /api/wastelog/{id}     → deletes a specific waste entry

using LittleC.Core.Models;
using LittleC.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LittleC.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WasteLogController : ControllerBase
    {
        private readonly AppDbContext _context;

        public WasteLogController(AppDbContext context)
        {
            _context = context;
        }

        // GET /api/wastelog/reasons
        // Returns all available waste reasons (Burnt, Dropped, Expired etc.)
        [HttpGet("reasons")]
        public async Task<IActionResult> GetReasons()
        {
            var reasons = await _context.WasteReasons
                .Select(r => new { r.Id, r.Reason })
                .ToListAsync();
            return Ok(reasons);
        }

        // GET /api/wastelog/today
        // Returns all waste logs for today including item name, reason, and who logged it
        [HttpGet("today")]
        public async Task<IActionResult> GetTodayLogs()
        {
            var today = DateTime.UtcNow.Date;
            var logs = await _context.WasteLogs
                .Include(l => l.MenuItem)
                .Include(l => l.WasteReason)
                .Include(l => l.User)
                .Where(l => l.LoggedAt.Date == today)
                .Select(l => new
                {
                    l.Id,
                    l.Quantity,
                    l.UnitPrice,
                    l.TotalCost,
                    l.Shift,
                    l.Notes,
                    l.LoggedAt,
                    ItemName = l.MenuItem.Name,
                    Reason = l.WasteReason.Reason,
                    LoggedBy = l.User.Name
                })
                .ToListAsync();
            return Ok(logs);
        }

        // POST /api/wastelog
        // Logs a new waste entry
        // If CustomPrice > 0 it uses that instead of the menu item price
        [HttpPost]
        public async Task<IActionResult> LogWaste([FromBody] WasteLogRequest request)
        {
            var menuItem = await _context.MenuItems.FindAsync(request.MenuItemId);
            if (menuItem == null)
                return BadRequest(new { message = "Invalid menu item" });

            var unitPrice = request.CustomPrice > 0
                ? request.CustomPrice
                : menuItem.CustomerPrice;

            var log = new WasteLog
            {
                UserId = request.UserId,
                MenuItemId = request.MenuItemId,
                WasteReasonId = request.WasteReasonId,
                Quantity = request.Quantity,
                UnitPrice = unitPrice,
                TotalCost = unitPrice * request.Quantity,
                Shift = request.Shift,
                Notes = request.Notes,
                LoggedAt = DateTime.UtcNow
            };

            _context.WasteLogs.Add(log);
            await _context.SaveChangesAsync();

            return Ok(new { log.Id, log.TotalCost, message = "Waste logged successfully" });
        }

        // DELETE /api/wastelog/{id}
        // Deletes a specific waste log entry by ID
        // Returns 404 if the entry does not exist
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLog(int id)
        {
            var log = await _context.WasteLogs.FindAsync(id);
            if (log == null)
                return NotFound(new { message = "Log not found" });

            _context.WasteLogs.Remove(log);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Log deleted" });
        }
    }

    // Request model for logging waste
    public class WasteLogRequest
    {
        public int UserId { get; set; }
        public int MenuItemId { get; set; }
        public int WasteReasonId { get; set; }
        public int Quantity { get; set; }
        public decimal CustomPrice { get; set; } // 0 means use menu price
        public string Shift { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}