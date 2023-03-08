using Hauturu.Serializers;
using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace Hauturu
{
    internal class Bot
    {
        private readonly string _botToken;
        private readonly Logger _botLogger;
        private readonly List<BotCommand> _botCommands;

        private readonly TelegramBotClient _bot;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly ReceiverOptions _receiverOptions;

        public async Task StartAsync()
        {
            var handlers = new Handlers(_bot, _botLogger);

            _bot.StartReceiving(
                updateHandler: handlers.HandleUpdateAsync,
                pollingErrorHandler: handlers.HandlePollingErrorAsync,
                receiverOptions: _receiverOptions,
                cancellationToken: _cancellationTokenSource.Token
                );

            await _bot.SetMyCommandsAsync(_botCommands);

            var botData = await _bot.GetMeAsync();
            _botLogger.LogInformation($"Starting listening for {botData.FirstName} at {DateTime.Now}.");
            Console.ReadKey();

            _cancellationTokenSource.Cancel();
        }

        public Bot()
        {
            var file = new FileStream("Config.json", FileMode.OpenOrCreate);
            var config = JsonSerializer.Deserialize<BotSerializer>(file)!;

            _botToken = config.BotToken;
            _botLogger = new Logger(config.BotLogger);
            _botCommands = config.BotCommands;

            _bot = new TelegramBotClient(_botToken);
            _cancellationTokenSource = new CancellationTokenSource();
            _receiverOptions = new ReceiverOptions();
        }
    }
}