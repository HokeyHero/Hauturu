using Telegram.Bot.Types.ReplyMarkups;

namespace Hauturu.Markups
{
    internal class InlineKeyboard
    {
        public readonly static InlineKeyboardMarkup Delete = new InlineKeyboardMarkup(new[]
        {
            new[] { InlineKeyboardButton.WithCallbackData(text: "Delete", callbackData: "delete") }
        });
    }
}
