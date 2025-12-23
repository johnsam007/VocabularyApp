namespace AgeSmartVocabulary.Helpers
{
    public static class DifficultyMapper
    {
        /// <summary>
        /// Map normalized frequency (0-100) to age groups
        /// Higher frequency = easier word = younger age group
        /// </summary>
        public static string GetDifficultyFromFrequency(double frequency)
        {
            // Frequency is now 0-100 (normalized from Datamuse score)

            if (frequency >= 70) return "5-7";      // Very common words
            if (frequency >= 50) return "8-10";     // Common words
            if (frequency >= 30) return "11-13";    // Medium frequency
            if (frequency >= 15) return "14-18";    // Less common
            return "Adult";                          // Rare words
        }

        public static string GetAgeGroupLabel(string ageGroup)
        {
            return ageGroup switch
            {
                "5-7" => "Early Learner (5-7 years)",
                "8-10" => "Elementary (8-10 years)",
                "11-13" => "Middle School (11-13 years)",
                "14-18" => "High School (14-18 years)",
                "Adult" => "Adult",
                _ => "Unknown"
            };
        }
    }
}