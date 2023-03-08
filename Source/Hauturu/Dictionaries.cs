using Hauturu.Serializers;
using System.Text.Json;

namespace Hauturu
{
    internal class Dictionaries
    {
        public async Task<DictionarySerializer?> GetDefinitionAsync(string word)
        {
            var response = await GetResponseAsync($"https://api.dictionaryapi.dev/api/v2/entries/en/{word}");
            List<DictionarySerializer>? definition;

            try
            {
                definition = JsonSerializer.Deserialize<List<DictionarySerializer>>(response);
            }
            catch (Exception)
            {
                return null;
            }

            return definition![0];
        }

        async Task<string> GetResponseAsync(string url)
        {
            var httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var response = await httpClient.SendAsync(request);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
