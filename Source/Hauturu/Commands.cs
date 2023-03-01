using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Hauturu
{
    internal class Commands
    {
        public static async Task<Message> RunStartAsync(ITelegramBotClient bot, Message message, CancellationToken cancellationToken)
        {
            string text = "*Hauturu* — open\\-source Telegram\\-bot for finding definitions and learning new words\\.\n" +
                          "Find words by using presented dictionaries and word lists, or create your own\\.\n" +
                          "Practice with exercises for learning new words and remember already learned\\.\n\n" +
                          "Name of the project inspired by [*kakapo*](https://en.wikipedia.org/wiki/K%C4%81k%C4%81p%C5%8D) — one of the oldest bird on the earth\\.\n" +
                          "Kakapo is _endangered species_ today\\. Arrival of humans was _main factor in the decline of the kakapo_\\.\n\n" +
                          "Source code of the project published in [*GitHub*](https://github.com/HokeyHero/Hauturu)\\.\n\n";

            return await bot.SendTextMessageAsync(
                       chatId: message.Chat.Id,
                       replyToMessageId: message.MessageId,
                       parseMode: ParseMode.MarkdownV2,
                       disableWebPagePreview: true,
                       text: text,
                       cancellationToken: cancellationToken
                       );
        }

        public static async Task<Message> RunDefinitionAsync(ITelegramBotClient bot, Message message, CancellationToken cancellationToken)
        {
            var dictionary = new DictionaryClient();
            var response = await dictionary.GetDefinitionAsync(message.Text);
            string text;

            if (response is null)
            {
                text = "Sorry, I didn't find any definitions for this word\\. \n" +
                       "Please, check is the word written correctly\\.";
            }
            else
            {
                text = $"*{response.Word}*";
                if (response.Phonetic is not null)
                {
                    var parsedString = ParseDefinition(response.Phonetic);
                    text = string.Concat(text, $" _{parsedString}_");
                }

                foreach (var meaning in response.Meanings)
                {
                    text = string.Concat(text, $"\n*· _{meaning.PartOfSpeech}_ *\n");

                    int n = 1;
                    foreach (var definition in meaning.Definitions)
                    {
                        var definition_parsed = ParseDefinition(definition.Definition);
                        text = string.Concat(text, $"*{n++}\\) {definition_parsed}*\n");

                        if (definition.Example is not null)
                        {
                            var example_parsed = ParseDefinition(definition.Example);
                            text = string.Concat(text, $"_{example_parsed}_\n");
                        }
                    }
                }
            }

            return await bot.SendTextMessageAsync(
                       chatId: message.Chat.Id,
                       replyToMessageId: message.MessageId,
                       replyMarkup: new InlineKeyboardMarkup(new[] { new[] { InlineKeyboardButton.WithCallbackData(text: "Delete", callbackData: "delete") } } ),
                       parseMode: ParseMode.MarkdownV2,
                       text: text,
                       cancellationToken: cancellationToken
                       );
        }

        static string ParseDefinition(string input)
        {
            var characters = new List<string>() {"_", "*", "[", "]", "(", ")", "~", "`", ">", "#", "+", "-", "=", "|", "{", "}", ".", "!"};
            foreach (var character in characters) 
            {
                input = input.Replace(character, $"\\{character}");
            }

            return input;
        }
    }
}
