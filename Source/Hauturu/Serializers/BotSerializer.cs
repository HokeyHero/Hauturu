using System.Text.Json.Serialization;
using Telegram.Bot.Types;

namespace Hauturu.Serializers
{
    internal class BotSerializer
    {
        [JsonPropertyName("BotToken")]
        public string BotToken { get; set; } = default!;

        [JsonPropertyName("BotLogger")]
        public bool BotLogger { get; set; } = false;

        [JsonPropertyName("BotCommands")]
        public List<BotCommand> BotCommands { get; set; } = default!;
    }
}
