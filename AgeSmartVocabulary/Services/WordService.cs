using AgeSmartVocabulary.Data;
using AgeSmartVocabulary.Helpers;
using AgeSmartVocabulary.Models;


namespace AgeSmartVocabulary.Services
{
    public class WordService
    {
        private readonly DatamuseApiService _datamuseApi;
        private readonly DictionaryApiService _dictionaryApi;
        private readonly DatabaseService _database;
        private readonly ContentFilterService _contentFilter;

        public WordService(
            DatamuseApiService datamuseApi,
            DictionaryApiService dictionaryApi,
            DatabaseService database)
        {
            _datamuseApi = datamuseApi;
            _dictionaryApi = dictionaryApi;
            _database = database;
            _contentFilter = new ContentFilterService();
        }

        /// <summary>
        /// Get a word for today - either from review schedule or fetch new
        /// </summary>
        public async Task<WordData> GetTodayWordAsync(string ageGroup)
        {
            System.Diagnostics.Debug.WriteLine($"=== Getting word for age group: {ageGroup} ===");

            // Check if there's a review due
            var reviewSchedule = await _database.GetTodayReviewAsync();

            if (reviewSchedule != null && !string.IsNullOrEmpty(reviewSchedule.WordText))
            {
                System.Diagnostics.Debug.WriteLine($"→ Found review word: {reviewSchedule.WordText}");
                return await GetWordDataAsync(reviewSchedule.WordText, ageGroup);
            }

            // Fetch new word from API
            System.Diagnostics.Debug.WriteLine("→ Fetching new word from API...");
            return await FetchNewWordAsync(ageGroup);
        }

        /// <summary>
        /// Fetch a new word that user hasn't seen
        /// </summary>
        private async Task<WordData> FetchNewWordAsync(string ageGroup)
        {
            var seenWords = await _database.GetSeenWordsAsync();
            System.Diagnostics.Debug.WriteLine($"User has seen {seenWords.Count} words");

            // Try multiple times to find a good word
            for (int attempt = 0; attempt < 5; attempt++)
            {
                System.Diagnostics.Debug.WriteLine($"→ Attempt {attempt + 1}/5");

                // Fetch age-appropriate words from API
                var apiWords = await _datamuseApi.GetWordsForAgeGroupAsync(ageGroup, 50);

                if (apiWords == null || apiWords.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("❌ No words from API");
                    continue;
                }

                System.Diagnostics.Debug.WriteLine($"✓ API returned {apiWords.Count} words");

                // Shuffle for randomness
                var shuffled = apiWords.OrderBy(x => Guid.NewGuid()).ToList();

                // Find a safe word that user hasn't seen
                foreach (var apiWord in shuffled)
                {
                    // Validate word
                    if (string.IsNullOrWhiteSpace(apiWord.Word))
                        continue;

                    // Check if safe
                    if (!_contentFilter.IsSafeWord(apiWord.Word))
                    {
                        System.Diagnostics.Debug.WriteLine($"⚠ Unsafe word filtered: {apiWord.Word}");
                        continue;
                    }

                    // Skip if already seen
                    if (seenWords.Contains(apiWord.Word))
                        continue;

                    // Check word length for age appropriateness
                    if (ageGroup == "5-7" && apiWord.Word.Length > 8)
                        continue;

                    if (ageGroup == "8-10" && apiWord.Word.Length > 12)
                        continue;

                    System.Diagnostics.Debug.WriteLine($"→ Trying word: {apiWord.Word}");

                    var wordData = await GetWordDataAsync(apiWord.Word, ageGroup);

                    // Only return if we successfully got safe definition
                    if (wordData != null && !string.IsNullOrEmpty(wordData.Meaning))
                    {
                        return wordData;
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine("❌ Could not find any valid safe word");
            return null;
        }

        /// <summary>
        /// Get complete word data (word + definition) with content filtering
        /// </summary>
        private async Task<WordData> GetWordDataAsync(string word, string ageGroup)
        {
            if (string.IsNullOrWhiteSpace(word))
                return null;

            // Double-check word safety
            if (!_contentFilter.IsSafeWord(word))
            {
                System.Diagnostics.Debug.WriteLine($"❌ Word blocked by filter: {word}");
                return null;
            }

            System.Diagnostics.Debug.WriteLine($"→ Fetching definition for: {word}");

            try
            {
                var definition = await _dictionaryApi.GetWordDefinitionAsync(word);

                if (definition == null)
                {
                    System.Diagnostics.Debug.WriteLine($"⚠ No definition found for: {word}");
                    return null;
                }

                var (meaning, example, partOfSpeech) = _dictionaryApi.GetBestDefinition(definition);

                if (string.IsNullOrEmpty(meaning))
                {
                    System.Diagnostics.Debug.WriteLine($"⚠ Empty meaning for: {word}");
                    return null;
                }

                // Filter definition content
                if (!_contentFilter.IsSafeDefinition(meaning))
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Unsafe definition for: {word}");
                    return null;
                }

                // Filter example content
                if (!string.IsNullOrEmpty(example) && !_contentFilter.IsSafeExample(example))
                {
                    System.Diagnostics.Debug.WriteLine($"⚠ Unsafe example, clearing for: {word}");
                    example = ""; // Clear example but keep the word
                }

                System.Diagnostics.Debug.WriteLine($"✓ Got safe, complete data for: {word}");

                return new WordData
                {
                    Word = word,
                    AgeGroup = ageGroup,
                    Meaning = meaning,
                    Example = example ?? "",
                    Phonetic = definition.Phonetic ?? "",
                    PartOfSpeech = partOfSpeech ?? ""
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error getting word data for '{word}': {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Mark word as known/reviewed
        /// </summary>
        public async Task MarkWordAsKnownAsync(string word)
        {
            if (string.IsNullOrWhiteSpace(word))
                return;

            var schedule = await _database.GetTodayReviewAsync();

            if (schedule == null || schedule.WordText != word)
            {
                schedule = new ReviewSchedule
                {
                    WordText = word,
                    Stage = 1,
                    LastReviewed = DateTime.Today,
                    NextReviewDate = DateTime.Today.AddDays(1)
                };
            }

            schedule.Stage++;
            schedule.LastReviewed = DateTime.Today;

            schedule.NextReviewDate = schedule.Stage switch
            {
                2 => DateTime.Today.AddDays(3),
                3 => DateTime.Today.AddDays(7),
                4 => DateTime.Today.AddDays(30),
                _ => DateTime.Today.AddDays(1)
            };

            if (schedule.Stage >= 4)
            {
                schedule.IsKnown = true;
            }

            await _database.SaveReviewScheduleAsync(schedule);
            System.Diagnostics.Debug.WriteLine($"✓ Marked '{word}' as known (Stage {schedule.Stage})");
        }

        /// <summary>
        /// Reset word to stage 1 (needs revision)
        /// </summary>
        public async Task MarkWordForRevisionAsync(string word)
        {
            if (string.IsNullOrWhiteSpace(word))
                return;

            var schedule = await _database.GetTodayReviewAsync();

            if (schedule == null || schedule.WordText != word)
            {
                schedule = new ReviewSchedule
                {
                    WordText = word,
                    Stage = 1,
                    LastReviewed = DateTime.Today,
                    NextReviewDate = DateTime.Today.AddDays(1)
                };
            }
            else
            {
                schedule.Stage = 1;
                schedule.LastReviewed = DateTime.Today;
                schedule.NextReviewDate = DateTime.Today.AddDays(1);
            }

            await _database.SaveReviewScheduleAsync(schedule);
            System.Diagnostics.Debug.WriteLine($"✓ Marked '{word}' for revision (Stage 1)");
        }
    }

    public class WordData
    {
        public string Word { get; set; }
        public string AgeGroup { get; set; }
        public string Meaning { get; set; }
        public string Example { get; set; }
        public string Phonetic { get; set; }
        public string PartOfSpeech { get; set; }
    }
}
