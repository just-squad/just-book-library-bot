using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PenumbraHerald.Services;
using PenumbraHerald.Settings;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace PenumbraHerald.Controllers;

[ApiController]
[Route("/api/v1/bot")]
public class BotController(IOptions<BotConfiguration> config) : ControllerBase
{
    [HttpGet("setWebhook")]
    public async Task<string> SetWebHook([FromServices] ITelegramBotClient bot, CancellationToken ct)
    {
        var webHookUrl = new Uri(config.Value.WebHookUrl + "/api/v1/bot/update");;
        await bot.SetWebhook(
            webHookUrl.ToString(),
            allowedUpdates: [],
            secretToken: config.Value.SecretToken,
            cancellationToken: ct);
        return $"Webhook set to {webHookUrl}";
    }

    [HttpPost("update")]
    public async Task<IActionResult> Post([FromBody] Update update, [FromServices] ITelegramBotClient bot,
        [FromServices] UpdateHandler handleUpdateService, CancellationToken ct)
    {
        if (Request.Headers[Constants.TelegramHeaders.SecretToken] != config.Value.SecretToken)
            return Forbid();
        try
        {
            await handleUpdateService.HandleUpdateAsync(bot, update, ct);
        }
        catch (Exception exception)
        {
            await handleUpdateService.HandleErrorAsync(bot, exception,
                Telegram.Bot.Polling.HandleErrorSource.HandleUpdateError, ct);
        }

        return Ok();
    }
}