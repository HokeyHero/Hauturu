using System.Text.Json;
using System.Text.Json.Serialization;

namespace Hauturu
{
    internal class DictionaryClient
    {
        public async Task<WordData?> GetDefinitionAsync(string word)
        {
            var response = await GetResponseAsync($"https://api.dictionaryapi.dev/api/v2/entries/en/{word}");
            List<WordData>? definition;

            try
            {
                definition = JsonSerializer.Deserialize<List<WordData>>(response);
            }
            catch (Exception)
            {
                return null;
            }

            return definition[0];
        }

        async Task<string> GetResponseAsync(string url)
        {
            HttpClient httpClient = new();
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var response = await httpClient.SendAsync(request);
            return await response.Content.ReadAsStringAsync();
        }

        internal class WordData
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
}
