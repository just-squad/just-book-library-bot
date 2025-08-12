using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace PenumbraHerald.Services;

public class UpdateHandler(ITelegramBotClient bot) : IUpdateHandler
{
    public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source,
        CancellationToken cancellationToken)
    {
        // Cooldown in case of network connection error
        if (exception is RequestException)
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
    }

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

    private async Task<Message> OnMessage(Message msg)
    {
        var chatId = msg.Chat.Id;
        var messageText = msg.Text;

        Console.WriteLine($"Получено сообщение {messageText} из чата {chatId}");

        return await bot.SendMessage(chatId, $"вашим сообщением было: \n{messageText}",
            cancellationToken: new CancellationToken());
    }
}