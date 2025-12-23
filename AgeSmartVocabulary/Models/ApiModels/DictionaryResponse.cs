using System.Text.Json.Serialization;

namespace AgeSmartVocabulary.Models.ApiModels
{
    public class DictionaryResponse
    {
        [JsonPropertyName("word")]
        public string Word { get; set; }

        [JsonPropertyName("phonetic")]
        public string Phonetic { get; set; }

        [JsonPropertyName("meanings")]
        public List<Meaning> Meanings { get; set; }
    }

    public class Meaning
    {
        [JsonPropertyName("partOfSpeech")]
        public string PartOfSpeech { get; set; }

        [JsonPropertyName("definitions")]
        public List<Definition> Definitions { get; set; }
    }

    public class Definition
    {
        [JsonPropertyName("definition")]
        public string DefinitionText { get; set; }

        [JsonPropertyName("example")]
        public string Example { get; set; }
    }
}