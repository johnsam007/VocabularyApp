using System.Text.Json.Serialization;

namespace AgeSmartVocabulary.Models.ApiModels
{
    public class DatamuseWord
    {
        [JsonPropertyName("word")]
        public string Word { get; set; }

        [JsonPropertyName("score")]
        public int Score { get; set; }

        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; }
    }
}