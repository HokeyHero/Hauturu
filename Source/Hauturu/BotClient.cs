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

            var me = await bot.GetMeAsync();
            Console.WriteLine($"Starting listening for {me.FirstName}.");
            Console.ReadLine();

            cts.Cancel();
        }

        async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
        {
            if (update.Type is UpdateType.Message)
            {
                if (update.Message.Text is not null)
                {
                    await bot.SendTextMessageAsync(
                        chatId: update.Message.Chat.Id,
                        text: update.Message.Text,
                        cancellationToken: cancellationToken
                        );

                    await bot.DeleteMessageAsync(
                        chatId: update.Message.Chat.Id,
                        messageId: update.Message.MessageId
                        );
                }
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
