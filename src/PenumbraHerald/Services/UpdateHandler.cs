using PenumbraHerald.Domain;
using PenumbraHerald.Infrastructure;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;

namespace PenumbraHerald.Services;

public class UpdateHandler(ITelegramBotClient bot, IVoteSuggestionsRepository voteSuggestionsRepository)
    : IUpdateHandler
{
    int themeCount = 0;

    public async Task HandleErrorAsync(
        ITelegramBotClient botClient,
        Exception exception,
        HandleErrorSource source,
        CancellationToken cancellationToken)
    {
        // Cooldown in case of network connection error
        if (exception is RequestException)
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
    }

    public string inlineQueryId;

    public async Task HandleUpdateAsync(
        ITelegramBotClient botClient,
        Update update,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await (update switch
        {
            { ChannelPost: { } message } => OnChannelPost(message, cancellationToken),
            { Message: { } message } => OnMessage(message, cancellationToken),
            { InlineQuery: { } inlineQuery } => OnInlineQuery(inlineQuery, cancellationToken),
            { CallbackQuery: { } callbackQuery } => OnCallbackQuery(callbackQuery, cancellationToken),
            _ => throw new NotImplementedException()
        });
    }

    private async Task OnInlineQuery(InlineQuery inlineQuery, CancellationToken token)
    {
        var result = new InlineQueryResultArticle(
            id: "start-button",
            title: "начать сбор тем",
            inputMessageContent: new InputTextMessageContent($"сбор тем начат, собрано тем: {themeCount}"))
        {
            Description = "эта команда инициирует старт сбора тем",
            ReplyMarkup =
                new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("завершить", "сбор тем остановлен"))
        };

        await bot.AnswerInlineQuery(
            inlineQueryId: inlineQuery.Id,
            results: new[] { result },
            isPersonal: true,
            cacheTime: 0,
            cancellationToken: new CancellationToken());
    }

    private async Task OnMessage(Message message, CancellationToken token)
    {
        var mes = "#предлагаю_тему";
        if (message.Text != null && message.Text.StartsWith(mes))
        {
            var active = await voteSuggestionsRepository.GetActive(token);
            if (active is not null)
            {
                active.AddVote(message.Text.Remove(0, mes.Length));
                await voteSuggestionsRepository.Update(active, token);
                await bot.EditMessageText(
                    inlineQueryId,
                    text: $"сбор тем начат, собрано тем: {active.Suggestions.Count}",
                    cancellationToken: token);
            }
        }
    }

    private async Task OnChannelPost(Message message, CancellationToken token)
    {
        var mes = "#предлагаю_тему";
        if (message.ViaBot is not null)
        {
            var active = await voteSuggestionsRepository.GetActive(token);
            if (active is not null)
            {
                await bot.SendMessage(message.Chat.Id, "Уже есть активный сбор", cancellationToken: token);
                return;
            }

            var vote = VoteSuggestion.CreateNew(message.Id, message.Chat.Id, message.From?.Username ?? "unknown");
            await voteSuggestionsRepository.Create(vote, token);
            return;
        }

        if (message.Text != null && message.Text.StartsWith(mes))
        {
            var active = await voteSuggestionsRepository.GetActive(token);
            if (active is not null)
            {
                active.AddVote(message.Text.Remove(0, mes.Length));
                await voteSuggestionsRepository.Update(active, token);
                await bot.EditMessageText(
                    active.Id.ChatId,
                    active.Id.SuggestionId,
                    text: $"сбор тем начат, собрано тем: {active.Suggestions.Count}",
                    cancellationToken: token,
                    replyMarkup:
                    new InlineKeyboardMarkup(
                        InlineKeyboardButton.WithCallbackData("завершить", "сбор тем остановлен")));
            }
        }
    }

    private async Task OnCallbackQuery(CallbackQuery callbackQuery, CancellationToken token)
    {
        var active = await voteSuggestionsRepository.GetActive(token);
        if (active is not null)
        {
            active.Close();
            await voteSuggestionsRepository.Update(active, token);
        }

        await bot.AnswerCallbackQuery(
            callbackQuery.Id,
            text: "Сбор тем завершен",
            cancellationToken: token);
    }
}
