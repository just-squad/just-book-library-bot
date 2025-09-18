using System.Collections.Concurrent; // ЧОКАВО библиотечка новая какая-то...
using PenumbraHerald.Domain;

namespace PenumbraHerald.Infrastructure;

public class VoteSuggestionsRepository : IVoteSuggestionsRepository // ЧОКАВО :
{
    private readonly ConcurrentDictionary<SuggestionKey, VoteSuggestion> _suggestions = new (); // ЧОКАВО new()

    public Task Create(VoteSuggestion suggestion, CancellationToken token)
    {
        _suggestions.TryAdd(suggestion.Id, suggestion); // TryAdd это метод типичный для списков?
        return Task.CompletedTask; // ЧОКАВО тут был ликбез про жизненный цикл таски
    }

    public async Task<VoteSuggestion?> Get(int suggestionId, long chatId, CancellationToken token)
    {
        if (_suggestions.TryGetValue(new SuggestionKey(suggestionId, chatId), out var val)) // ЧОКАВО ryGetValue, out var
        {
            return await Task.FromResult(val); // ЧОКАВО FromResult
        }

        return null;
    }

    public Task Update(VoteSuggestion suggestion, CancellationToken token)
    {
        if (_suggestions.TryGetValue(suggestion.Id, out var val))
        {
            _suggestions.TryUpdate(suggestion.Id, suggestion, val);
            return Task.CompletedTask;
        }

        return Task.CompletedTask;
    }

    public Task<VoteSuggestion?> GetActive(CancellationToken token)
    {
        var activeSuggestion = _suggestions.Values
            .OrderBy(it => it.StartedAt) // ЧОКАВО OrderBy из какой библиотеки?
            .FirstOrDefault(s => s.ClosedAt == null); // ЧОКАВО а FirstOrDefault метод какой библиотеки?
        return Task.FromResult(activeSuggestion);
    }
}
