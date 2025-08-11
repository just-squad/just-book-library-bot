using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PenumbraHerald.Services;
using PenumbraHerald.settings;
using Telegram.Bot;
using Telegram.Bot.Types;
using Insight.TelegramBot.WebHook.Controllers;


namespace PenumbraHerald.Controllers;

[ApiController]
[Route("[controller]")]
public class BotController(IOptions<BotConfiguration> Config) : ControllerBase
{
    [HttpGet("setWebhook")]
    public async Task<string> SetWebHook([FromServices] ITelegramBotClient bot, CancellationToken ct)
    {
        var botWebhookUrl = Config.Value.IPAddress;
        var myBotToken = Config.Value.Token;
        // var webhookUrl = Config.Value.BotWebhookUrl.AbsoluteUri;
        var webhookUrl = $"https://api.telegram.org/bot{myBotToken}/setWebhook?url={botWebhookUrl}";
        await bot.SetWebhook(webhookUrl, allowedUpdates: [], secretToken: Config.Value.Token, cancellationToken: ct);
        return $"Webhook set to {webhookUrl}";
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Update update, [FromServices] ITelegramBotClient bot, [FromServices] UpdateHandler handleUpdateService, CancellationToken ct)
    {
        if (Request.Headers["X-Telegram-Bot-Api-Secret-Token"] != Config.Value.Token)
            return Forbid();
        try
        {
            await handleUpdateService.HandleUpdateAsync(bot, update, ct);
        }
        catch (Exception exception)
        {
            await handleUpdateService.HandleErrorAsync(bot, exception, Telegram.Bot.Polling.HandleErrorSource.HandleUpdateError, ct);
        }
        return Ok();
    }
}