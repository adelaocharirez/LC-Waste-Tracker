namespace LittleC.Core.Models
{
    public class MenuItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal CustomerPrice { get; set; } // customer facing price
        public bool IsActive { get; set; } = true;
        public bool IsCustom { get; set; } = false; // true for one-off custom entries

        // Navigation property
        public ICollection<WasteLog> WasteLogs { get; set; } = new List<WasteLog>();
    }
}