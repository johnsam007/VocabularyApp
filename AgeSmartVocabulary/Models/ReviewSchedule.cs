using SQLite;

namespace AgeSmartVocabulary.Models
{
    [Table("ReviewSchedule")]
    public class ReviewSchedule
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public string WordText { get; set; } // Changed from WordId to WordText

        public DateTime NextReviewDate { get; set; }
        public int Stage { get; set; } = 1; // 1,2,3,4
        public DateTime LastReviewed { get; set; }
        public bool IsKnown { get; set; } = false;
    }
}