namespace PenumbraHerald.Settings;

/// <summary>
/// Configuration for the bot.
/// </summary>
public class BotConfiguration
{
    internal static string Name => nameof(BotConfiguration);
    
    /// <summary>
    /// The main telegram bot token.
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// The secret token used for verifying webhook requests.
    /// </summary>
    public string SecretToken { get; set; } = string.Empty;

    /// <summary>
    /// The URL to which the bot will send webhooks.
    /// </summary>
    public string WebHookUrl { get; set; } = string.Empty;
}