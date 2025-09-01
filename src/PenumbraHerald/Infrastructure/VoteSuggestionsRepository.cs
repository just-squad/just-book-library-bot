using System.Collections.Concurrent;
using PenumbraHerald.Domain;

namespace PenumbraHerald.Infrastructure;

public class VoteSuggestionsRepository : IVoteSuggestionsRepository
{
    private readonly ConcurrentDictionary<SuggestionKey, VoteSuggestion> _suggestions = new();

    public Task Create(VoteSuggestion suggestion, CancellationToken token)
    {
        _suggestions.TryAdd(suggestion.Id, suggestion);
        return Task.CompletedTask;
    }

    public async Task<VoteSuggestion?> Get(int suggestionId, long chatId, CancellationToken token)
    {
        if (_suggestions.TryGetValue(new SuggestionKey(suggestionId, chatId), out var val))
        {
            return await Task.FromResult(val);
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
            .OrderBy(it => it.StartedAt)
            .FirstOrDefault(s => s.ClosedAt == null);
        return Task.FromResult(activeSuggestion);
    }
}
