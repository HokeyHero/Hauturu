using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Hauturu
{
    internal class Handlers
    {
        private readonly ITelegramBotClient _bot;
        private readonly Logger _logger;

        public async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                    await HandleMessageAsync(update.Message!, cancellationToken);
                    break;

                case UpdateType.CallbackQuery:
                    await HandleCallbackQueryAsync(update.CallbackQuery!, cancellationToken);
                    break;

                default: break;
            }
        }

        async Task HandleMessageAsync(Message message, CancellationToken cancellationToken)
        {
            switch (message.Type)
            {
                case MessageType.Text:
                    await HandleTextMessageAsync(message, cancellationToken);
                    break;

                default: break;
            }

            async Task HandleTextMessageAsync(Message message, CancellationToken cancellationToken)
            {
                _logger.LogInformation($"Request: Message {message.MessageId} received from {message.From.Id} at {message.Date}.");

                Commands commands = new Commands(_bot, _logger);
                var answer = message.Text switch
                {
                    "/start" => await commands.RunStartAsync(message, cancellationToken),
                    _ => await commands.RunDefinitionAsync(message, cancellationToken)
                };

                _logger.LogInformation($"Response: Sent message {answer.MessageId} to {answer.ReplyToMessage.From.Id} at {answer.Date}.\n");
            }
        }

        async Task HandleCallbackQueryAsync(CallbackQuery query, CancellationToken cancellationToken)
        {
            await _bot.AnswerCallbackQueryAsync(
                callbackQueryId: query.Id,
                cancellationToken: cancellationToken
                );

            switch (query.Data)
            {
                case "delete":
                    await _bot.DeleteMessageAsync(
                            chatId: query.Message.Chat.Id,
                            messageId: query.Message.MessageId,
                            cancellationToken: cancellationToken
                            );

                    await _bot.DeleteMessageAsync(
                            chatId: query.Message.Chat.Id,
                            messageId: query.Message.ReplyToMessage.MessageId,
                            cancellationToken: cancellationToken
                            );
                    break;
            }
        }

        public Task HandlePollingErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error: [{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

        public Handlers(ITelegramBotClient bot, Logger logger)
        {
            _bot = bot;
            _logger = logger;
        }
    }
}
