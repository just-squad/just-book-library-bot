using Insight.TelegramBot.WebHook;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace PenumbraHerald.controllers;

public class botControllers
{
    [ApiController]
    [Route("update")]
    public class UpdateController : ControllerBase
    {
        private static ITelegramBotClient? _botClient;

        public UpdateController(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        [HttpPost]
       public async Task<IActionResult> Post([FromBody] botControllers botControllers)
        {
            var cts = new CancellationTokenSource();
            _botClient.StartReceiving(ITelegramBotClient,HandleUpdateAsync, HandleErrorAsync, new ReceiverOptions { },
                cancellationToken: cts.Token
            );

            User me = _botClient.GetMe().Result;

            Console.WriteLine($"I am bot and my name is {me.Username}.");

            Console.WriteLine($"@{me.Username} is running... Press Enter to terminate");
            Console.ReadLine();
            cts.Cancel(); // stop the bot
            return;

            // method that handle messages received by the bot:
            async Task HandleUpdateAsync(ITelegramBotClient client, botControllers update, CancellationToken cancellationToken)
            {
                if (update.Message is { Text: null }) return;

                if (update.Message != null)
                {
                    var chatId = update.Message.Chat.Id;
                    var messageText = update.Message.Text;

                    Console.WriteLine($"Получено сообщение {messageText} из чата {chatId}");

                    await client.SendMessage(chatId, $"вашим сообщением было: \n{messageText}",
                        cancellationToken: cancellationToken);

                    if (messageText == "/start")
                    {
                        await client.SendMessage(chatId, "нажми на меня",
                            replyMarkup: new InlineKeyboardButton[] { "пуньк", "поньк" },
                            cancellationToken: cancellationToken);
                    }
                }

                if (update is { CallbackQuery: { } query })
                {
                    await client.AnswerCallbackQuery(query.Id, $"кто-то жамкнул на кнопку {query.Data}",
                        cancellationToken: cancellationToken);
                    if (query.Message != null)
                        await client.SendMessage(query.Message!.Chat,
                            $"юзер @{query.From.Username} жамкнул на кнопку {query.Data}",
                            cancellationToken: cancellationToken);
                }
            }

            Task HandleErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken cancellationToken)
            {
                Console.WriteLine(exception);
                return Task.CompletedTask;
            }
        }
    }
}