using System.Net.Http.Json;
using AgeSmartVocabulary.Models.ApiModels;

namespace AgeSmartVocabulary.Services
{
    public class DictionaryApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://api.dictionaryapi.dev/api/v2/entries/en";

        public DictionaryApiService()
        {
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
        }

        /// <summary>
        /// Get word definition, examples, and phonetics
        /// </summary>
        public async Task<DictionaryResponse> GetWordDefinitionAsync(string word)
        {
            // Add null/empty check
            if (string.IsNullOrWhiteSpace(word))
            {
                System.Diagnostics.Debug.WriteLine("❌ DictionaryAPI: Word is null or empty");
                return null;
            }

            try
            {
                var cleanWord = word.Trim().ToLower();
                var url = $"{BaseUrl}/{cleanWord}";

                System.Diagnostics.Debug.WriteLine($"→ Fetching definition: {url}");

                var response = await _httpClient.GetFromJsonAsync<List<DictionaryResponse>>(url);

                if (response == null || response.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine($"⚠ No definition found for '{word}'");
                    return null;
                }

                System.Diagnostics.Debug.WriteLine($"✓ Got definition for '{word}'");
                return response.FirstOrDefault();
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"⚠ Dictionary API Error for '{word}': {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Unexpected error for '{word}': {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Extract best definition and example from response
        /// </summary>
        public (string meaning, string example, string partOfSpeech) GetBestDefinition(DictionaryResponse response)
        {
            if (response?.Meanings == null || response.Meanings.Count == 0)
                return (null, null, null);

            var firstMeaning = response.Meanings.First();

            if (firstMeaning.Definitions == null || firstMeaning.Definitions.Count == 0)
                return (null, null, null);

            var firstDefinition = firstMeaning.Definitions.First();

            return (
                meaning: firstDefinition.DefinitionText,
                example: firstDefinition.Example,
                partOfSpeech: firstMeaning.PartOfSpeech
            );
        }

        /// <summary>
        /// Get all definitions (for advanced view)
        /// </summary>
        public List<(string meaning, string example, string partOfSpeech)> GetAllDefinitions(DictionaryResponse response)
        {
            var definitions = new List<(string, string, string)>();

            if (response?.Meanings == null)
                return definitions;

            foreach (var meaning in response.Meanings)
            {
                if (meaning.Definitions == null)
                    continue;

                foreach (var def in meaning.Definitions)
                {
                    definitions.Add((
                        meaning: def.DefinitionText,
                        example: def.Example,
                        partOfSpeech: meaning.PartOfSpeech
                    ));
                }
            }

            return definitions;
        }
    }
}