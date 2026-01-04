using System.Net.Http.Json;
using AgeSmartVocabulary.Models.ApiModels;

namespace AgeSmartVocabulary.Services
{
    public class DatamuseApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://api.datamuse.com/words";

        public DatamuseApiService()
        {
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
        }

        /// <summary>
        /// Fetch age-appropriate words from Datamuse API
        /// </summary>
        public async Task<List<DatamuseWord>> GetWordsForAgeGroupAsync(string ageGroup, int maxWords = 100)
        {
            try
            {
                var allWords = new List<DatamuseWord>();

                string[] topics;

                switch (ageGroup)
                {
                    case "5-7":
                        topics = new[] { "animal", "color", "food", "family", "toy", "school", "home" };
                        break;

                    case "8-10":
                        topics = new[] { "nature", "sport", "friend", "learn", "play", "read", "science" };
                        break;

                    case "11-13":
                        topics = new[] { "technology", "history", "geography", "mathematics", "literature" };
                        break;

                    case "14-18":
                        topics = new[] { "philosophy", "economics", "biology", "physics", "society" };
                        break;

                    default:
                        topics = new[] { "professional", "academic", "business", "research", "analysis" };
                        break;
                }

                var wordsPerTopic = Math.Max(10, maxWords / topics.Length);

                foreach (var topic in topics)
                {
                    try
                    {
                        var url = $"{BaseUrl}?ml={topic}&max={wordsPerTopic}&md=s";

                        System.Diagnostics.Debug.WriteLine($"→ Fetching words related to: {topic}");

                        var response = await _httpClient.GetFromJsonAsync<List<DatamuseWord>>(url);

                        if (response != null && response.Count > 0)
                        {
                            allWords.AddRange(response);
                            System.Diagnostics.Debug.WriteLine($"✓ Got {response.Count} words for topic '{topic}'");
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Topic '{topic}' failed: {ex.Message}");
                    }
                }

                // Remove duplicates
                var uniqueWords = allWords
                    .GroupBy(w => w.Word)
                    .Select(g => g.First())
                    .ToList();

                // 🔽 NEW: Apply extra simplification ONLY for age group 5–7
                if (ageGroup == "5-7")
                {
                    uniqueWords = ApplyYoungKidsFilter(uniqueWords);
                }

                System.Diagnostics.Debug.WriteLine($"✓ Total final words: {uniqueWords.Count}");
                return uniqueWords;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Datamuse API Error: {ex.Message}");
                return new List<DatamuseWord>();
            }
        }

        /// <summary>
        /// Extra filtering rules for very young kids (5–7)
        /// </summary>
        private List<DatamuseWord> ApplyYoungKidsFilter(List<DatamuseWord> words)
        {
            var filtered = words
                .Where(w =>
                    !string.IsNullOrWhiteSpace(w.Word) &&
                    w.Word.Length <= 6 &&
                    w.Word.All(char.IsLetter) &&
                    ExtractFrequency(w) >= 30 // common words only
                )
                .ToList();

            // Fallback – never return empty list
            if (!filtered.Any())
            {
                filtered = words
                    .Where(w => w.Word.Length <= 6 && w.Word.All(char.IsLetter))
                    .Take(30)
                    .ToList();
            }

            return filtered;
        }

        /// <summary>
        /// Extract frequency score from DatamuseWord
        /// Datamuse score represents word popularity
        /// </summary>
        public double ExtractFrequency(DatamuseWord word)
        {
            if (word.Score <= 0)
                return 0;

            var normalized = Math.Log10(word.Score + 1) * 20;
            return Math.Min(100, Math.Max(0, normalized));
        }

        /// <summary>
        /// Get very simple, common words for young children
        /// (kept as-is, untouched)
        /// </summary>
        public async Task<List<DatamuseWord>> GetSimpleWordsAsync(int maxWords = 50)
        {
            try
            {
                var url = $"{BaseUrl}?topics=kindergarten&max={maxWords}";

                var response = await _httpClient.GetFromJsonAsync<List<DatamuseWord>>(url);

                return response ?? new List<DatamuseWord>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Simple words error: {ex.Message}");
                return new List<DatamuseWord>();
            }
        }
    }
}
