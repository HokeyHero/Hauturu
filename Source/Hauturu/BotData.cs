using System.Text.Json;
using System.Text.Json.Serialization;
using Telegram.Bot.Types;

namespace Hauturu
{
    internal class BotData
    {
        public string Token { get; }
        public readonly List<BotCommand> Commands = new()
        {
            new BotCommand { Command = "/start", Description = "starts the bot" },
        /*  new BotCommand { Command = "/help", Description = "shows list of features" },
            new BotCommand { Command = "/settings", Description = "sets the bot preferences" } */
        };

        public BotData()
        {
            var file = new FileStream("Configuration.json", FileMode.OpenOrCreate);
            var jsonData = JsonSerializer.Deserialize<JsonData>(file);

            Token = jsonData.Token;
        }

        private class JsonData
        {
            [JsonPropertyName("bot_token")]
            public string Token { get; set; }
        }
    }
}
