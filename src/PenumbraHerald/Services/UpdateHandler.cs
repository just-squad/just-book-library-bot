using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;

namespace PenumbraHerald.Services;

public class UpdateHandler(ITelegramBotClient bot) : IUpdateHandler
{
    int themeCount = 0;

    public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source,
        CancellationToken cancellationToken)
    {
        // Cooldown in case of network connection error
        if (exception is RequestException)
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
    }

    public string responseId; //{ get; set; }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await (update switch
        {
            { Message: { } message } => OnMessage(message),
            _ => throw new NotImplementedException()
        });
    }

    private async Task OnMessage(Message message)
    {

        if (message.Text.StartsWith("/start_collecting_book_topics"))
        {
            await bot.SendMessage(message.Chat.Id,
                $"Сбор тем начат. Собрано тем: {themeCount}",
                replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton("Завершить сбор")));
        }
        if (message.Text.Contains("#предлагаю_тему")){
            themeCount += 1;
            await bot.EditMessageText(message.Id.ToString(), text: $"сбор тем начат, собрано тем: {themeCount}");
        }
    }
}
