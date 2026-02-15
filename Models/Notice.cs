namespace StudentPortal.Models
{
    public class Notice
    {
        public int NoticeId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Category { get; set; } = "General"; // Exam, Sports, Events, General
        public string? ImageUrl { get; set; }

        public string CreatedByUserId { get; set; } = string.Empty;
        public virtual ApplicationUser? CreatedByUser { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
