using System.Text.Json.Serialization;

namespace Hauturu.Serializers
{
    internal class DictionarySerializer
    {
        [JsonPropertyName("word")]
        public string Word { get; set; }

        [JsonPropertyName("phonetic")]
        public string? Phonetic { get; set; }

        [JsonPropertyName("phonetics")]
        public List<PhoneticArray>? Phonetics { get; set; }

        internal class PhoneticArray
        {
            [JsonPropertyName("text")]
            public string? Text { get; set; }

            [JsonPropertyName("audio")]
            public string? Audio { get; set; }
        }

        [JsonPropertyName("origin")]
        public string? Origin { get; set; }

        [JsonPropertyName("meanings")]
        public List<MeaningArray> Meanings { get; set; }

        internal class MeaningArray
        {
            [JsonPropertyName("partOfSpeech")]
            public string PartOfSpeech { get; set; }

            [JsonPropertyName("definitions")]
            public List<DefinitionArray> Definitions { get; set; }

            internal class DefinitionArray
            {
                [JsonPropertyName("definition")]
                public string Definition { get; set; }

                [JsonPropertyName("example")]
                public string? Example { get; set; }

                [JsonPropertyName("synonyms")]
                public string[]? Synonyms { get; set; }

                [JsonPropertyName("antonyms")]
                public string[]? Antonyms { get; set; }
            }
        }
    }
}
