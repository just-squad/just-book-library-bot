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

    public string inlineQueryId;

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await (update switch
        {
            { Message: { } message } => OnMessage(message),
            { InlineQuery: { } inlineQuery } => OnInlineQuery(inlineQuery),
            { CallbackQuery:{ } callbackQuery} => OnCallbackQuery(callbackQuery),
            _ => throw new NotImplementedException()
        });
    }

    private async Task<Task> OnInlineQuery(InlineQuery inlineQuery)
    {
        var result = new InlineQueryResultArticle(
            id: "start-button",
            title: "начать сбор тем",
            inputMessageContent: new InputTextMessageContent($"сбор тем начат, собрано тем: {themeCount}"))
        {
            Description = "эта команда инициирует старт сбора тем",
            ReplyMarkup = new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("завершить", "сбор тем остановлен"))
        };

        inlineQueryId = inlineQuery.Id;

        return bot.AnswerInlineQuery(
            inlineQueryId: inlineQuery.Id,
            results: new[] { result },
            isPersonal: true,
            cacheTime: 0,
            cancellationToken: new CancellationToken());
    }

    private async Task OnMessage(Message message)
    {
        if (message.Text.Contains("#предлагаю_тему")){
            themeCount++;
            bot.EditMessageText(inlineQueryId, text: $"сбор тем начат, собрано тем: {themeCount}");
        }
    }

    private async Task<Task> OnCallbackQuery(CallbackQuery callbackQuery)
    {
        return bot.AnswerCallbackQuery(callbackQuery.Id, text: "Сбор тем завершен");
    }
}
