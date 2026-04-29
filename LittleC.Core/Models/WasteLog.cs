namespace LittleC.Core.Models
{
    public class WasteLog
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int MenuItemId { get; set; }
        public int WasteReasonId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; } // snapshot of price at time of logging
        public decimal TotalCost { get; set; } // UnitPrice x Quantity
        public string Shift { get; set; } = string.Empty; // "Morning" or "Night"
        public string? Notes { get; set; }
        public DateTime LoggedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public User User { get; set; } = null!;
        public MenuItem MenuItem { get; set; } = null!;
        public WasteReason WasteReason { get; set; } = null!;
    }
}