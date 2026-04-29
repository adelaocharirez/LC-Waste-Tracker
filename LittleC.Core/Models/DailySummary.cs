namespace LittleC.Core.Models
{
    public class DailySummary
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalWasteValue { get; set; }
        public string? PhotoUrl { get; set; } // S3 URL for end of night photo
        public int SubmittedByUserId { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public User SubmittedBy { get; set; } = null!;
    }
}