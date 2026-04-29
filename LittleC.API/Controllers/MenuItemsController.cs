// Handles fetching menu items for the quick log interface
// Routes:
//   GET /api/menuitems → returns all active menu items with prices

using LittleC.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LittleC.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenuItemsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MenuItemsController(AppDbContext context)
        {
            _context = context;
        }

        // GET /api/menuitems
        // Returns all active menu items
        // IsCustom = true means it's the "Other" option for custom waste entries
        [HttpGet]
        public async Task<IActionResult> GetMenuItems()
        {
            var items = await _context.MenuItems
                .Where(m => m.IsActive)
                .Select(m => new { m.Id, m.Name, m.CustomerPrice, m.IsCustom })
                .ToListAsync();
            return Ok(items);
        }
    }
}