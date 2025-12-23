using SQLite;

namespace AgeSmartVocabulary.Models
{
    [Table("UserProfile")]
    public class UserProfile
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string AgeGroup { get; set; }
        public string PreferredNotificationTime { get; set; } = "07:00";
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}