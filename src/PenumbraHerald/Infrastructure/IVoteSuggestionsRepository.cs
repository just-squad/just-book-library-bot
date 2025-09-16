using PenumbraHerald.Domain;

namespace PenumbraHerald.Infrastructure;

public interface IVoteSuggestionsRepository // ЧОКАВО interface, как сам по себе так и в контексте проекта в целом, мб схему тут какую...
{
    Task Create(VoteSuggestion suggestion, CancellationToken token);
    Task<VoteSuggestion?> Get(int suggestionId, long chatId, CancellationToken token); // ЧОКАВО тип в <>, что это и зачем
    Task Update(VoteSuggestion suggestion, CancellationToken token);
    Task<VoteSuggestion?> GetActive(CancellationToken token);
}
