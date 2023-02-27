using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Hauturu
{
    internal class BotClient
    {
        public async Task StartBotAsync(BotData data)
        {
            var bot = new TelegramBotClient(data.Token);
            var cts = new CancellationTokenSource();
            var receiverOptions = new ReceiverOptions();

            bot.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
                );

            await bot.SetMyCommandsAsync(data.Commands);

            var dictionary = new Dictionary();
            var word = await dictionary.GetWordDefinition("word");
            if (word != null)
                Console.WriteLine(word.Word);

            var me = await bot.GetMeAsync();
            Console.WriteLine($"Starting listening for {me.FirstName}.");
            Console.ReadLine();

            cts.Cancel();
        }

        async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                    await HandleMessageAsync(bot, update.Message, cancellationToken);
                    break;
            }
        }

        async Task HandleMessageAsync(ITelegramBotClient bot, Message message, CancellationToken cancellationToken)
        {
            switch (message.Type) 
            {
                case MessageType.Text:
                    await HandleTextMessageAsync(bot, message, cancellationToken);
                    break;
            }
        }

        async Task HandleTextMessageAsync(ITelegramBotClient bot, Message message, CancellationToken cancellationToken)
        {
            switch (message.Text)
            {
                case "/start":
                    await bot.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        replyToMessageId: message.MessageId,
                        parseMode: ParseMode.MarkdownV2,
                        disableWebPagePreview: true,
                        text: "*Hauturu* — open\\-source Telegram\\-bot for finding definitions and learning new words\\. \n" +
                              "Find words by using presented dictionaries and word lists, or create your own\\. \n" +
                              "Practice with exercises for learning new words and remember already learned\\. \n\n" +
                              "Name of the project inspired by [*kakapo*](https://en.wikipedia.org/wiki/K%C4%81k%C4%81p%C5%8D) — one of the oldest bird on the earth\\. \n" +
                              "Kakapo is _endangered species_ today\\. Arrival of humans was _main factor in the decline of the kakapo_\\.\n\n" +
                              "Source code of the project published in [*GitHub*](https://github.com/HokeyHero/Hauturu)\\.",
                        cancellationToken: cancellationToken
                        );
                    break;
            }
        }

        Task HandlePollingErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}
