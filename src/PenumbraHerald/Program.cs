using System;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using DotNetEnv;
using PenumbraHerald.settings;
using Insight.TelegramBot.WebHook.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

Env.Load();
var token = Environment.GetEnvironmentVariable("TOKEN");

var builder = WebApplication.CreateBuilder(args);
var botConfiguration = builder.Configuration.GetSection("BotConfiguration");
builder.Services.Configure<BotConfiguration>(botConfiguration);
builder.Services.AddHttpClient("telegramwebhook").RemoveAllLoggers().AddTypedClient<ITelegramBotClient>(
    httpClient => new TelegramBotClient(botConfiguration.Get<BotConfiguration>()!.Token, httpClient));

builder.Services.AddSingleton<DefaultUpdateController>();

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(opt => { });

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(opt =>
{
    opt.SwaggerEndpoint("/swagger/v1/swagger.json", "PenumbraHerald");
    opt.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
