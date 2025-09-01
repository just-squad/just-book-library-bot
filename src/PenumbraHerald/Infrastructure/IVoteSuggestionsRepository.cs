using PenumbraHerald.Domain;

namespace PenumbraHerald.Infrastructure;

public interface IVoteSuggestionsRepository
{
    Task Create(VoteSuggestion suggestion, CancellationToken token);
    Task<VoteSuggestion?> Get(int suggestionId, long chatId, CancellationToken token);
    Task Update(VoteSuggestion suggestion, CancellationToken token);
    Task<VoteSuggestion?> GetActive(CancellationToken token);
}
