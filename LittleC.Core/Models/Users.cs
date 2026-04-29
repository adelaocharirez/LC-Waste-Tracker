namespace LittleC.Core.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PIN { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // "Manager", "AssistantManager", "ShiftLead"
        public bool IsActive { get; set; } = true;

        // Navigation property
        public ICollection<WasteLog> WasteLogs { get; set; } = new List<WasteLog>();
    }
}