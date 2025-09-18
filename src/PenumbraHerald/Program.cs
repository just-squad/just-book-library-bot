using PenumbraHerald.Extensions;
using Telegram.Bot;
using PenumbraHerald.Infrastructure;
using PenumbraHerald.Settings;

var builder = WebApplication.CreateBuilder(args);
var botConfiguration = builder.Configuration.GetSection(BotConfiguration.Name);
builder.Services.Configure<BotConfiguration>(botConfiguration);
builder.Services.AddUpdateHandlers();
builder.Services.AddSingleton<IVoteSuggestionsRepository, VoteSuggestionsRepository>();
builder.Services.AddHttpClient("telegramwebhook").RemoveAllLoggers().AddTypedClient<ITelegramBotClient>(
    httpClient => new TelegramBotClient(botConfiguration.Get<BotConfiguration>()!.Token, httpClient));

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(opt => { });

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(opt =>
{
    opt.SwaggerEndpoint("/swagger/v1/swagger.json", "PenumbraHerald");
    opt.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
