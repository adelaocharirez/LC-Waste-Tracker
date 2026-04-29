namespace LittleC.Core.Models
{
    public class WasteReason
    {
        public int Id { get; set; }
        public string Reason { get; set; } = string.Empty; // Burnt, Dropped, Expired etc.

        // Navigation property
        public ICollection<WasteLog> WasteLogs { get; set; } = new List<WasteLog>();
    }
}