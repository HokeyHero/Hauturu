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
            switch (update.Type)
            {
                case UpdateType.Message:
                    await HandleMessageAsync(bot, update.Message, cancellationToken);
                    break;
                case UpdateType.CallbackQuery:
                    await HandleCallbackQueryAsync(bot, update.CallbackQuery, cancellationToken);
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
            var answer = message.Text switch
            {
                "/start" => await Commands.RunStartAsync(bot, message, cancellationToken),
                _ => await Commands.RunDefinitionAsync(bot, message, cancellationToken)
            };
        }

        async Task HandleCallbackQueryAsync(ITelegramBotClient bot, CallbackQuery query, CancellationToken cancellationToken)
        {
            await bot.AnswerCallbackQueryAsync(
                callbackQueryId: query.Id,
                cancellationToken: cancellationToken
                );

            if (query.Data == "delete")
            {
                await bot.DeleteMessageAsync(
                    chatId: query.Message.Chat.Id,
                    messageId: query.Message.MessageId,
                    cancellationToken: cancellationToken
                    );

                await bot.DeleteMessageAsync(
                    chatId: query.Message.Chat.Id,
                    messageId: query.Message.ReplyToMessage.MessageId,
                    cancellationToken: cancellationToken
                    );
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
