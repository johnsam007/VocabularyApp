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

                // Different strategies based on age group
                string[] topics;

                switch (ageGroup)
                {
                    case "5-7":
                        // Very simple, common words for young children
                        topics = new[] { "animal", "color", "food", "family", "toy", "school", "home" };
                        break;

                    case "8-10":
                        // Elementary school vocabulary
                        topics = new[] { "nature", "sport", "friend", "learn", "play", "read", "science" };
                        break;

                    case "11-13":
                        // Middle school vocabulary
                        topics = new[] { "technology", "history", "geography", "mathematics", "literature" };
                        break;

                    case "14-18":
                        // High school vocabulary
                        topics = new[] { "philosophy", "economics", "biology", "physics", "society" };
                        break;

                    default: // Adult
                        topics = new[] { "professional", "academic", "business", "research", "analysis" };
                        break;
                }

                var wordsPerTopic = Math.Max(10, maxWords / topics.Length);

                foreach (var topic in topics)
                {
                    try
                    {
                        // ml= means "words related to meaning"
                        // topics= means "topic category"
                        var url = $"{BaseUrl}?ml={topic}&max={wordsPerTopic}";

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

                System.Diagnostics.Debug.WriteLine($"✓ Total unique words: {uniqueWords.Count}");
                return uniqueWords;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Datamuse API Error: {ex.Message}");
                return new List<DatamuseWord>();
            }
        }

        /// <summary>
        /// Extract frequency score from DatamuseWord
        /// Datamuse score represents word popularity
        /// </summary>
        public double ExtractFrequency(DatamuseWord word)
        {
            if (word.Score <= 0)
                return 0;

            // Higher score = more common word
            // Normalize to 0-100 scale
            var normalized = Math.Log10(word.Score + 1) * 20;
            return Math.Min(100, Math.Max(0, normalized));
        }

        /// <summary>
        /// Get very simple, common words for young children
        /// </summary>
        public async Task<List<DatamuseWord>> GetSimpleWordsAsync(int maxWords = 50)
        {
            try
            {
                // Use vocabulary= parameter for simple words
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