namespace PenumbraHerald.Domain;

public record struct SuggestionKey(int SuggestionId, long ChatId);

public class VoteSuggestion
{
    public SuggestionKey Id { get; }
    public string StartedBy { get; }
    public DateTime StartedAt { get; }
    public DateTime? ClosedAt { get; private set; }

    private readonly List<string> _suggestions;
    public IReadOnlyCollection<string> Suggestions => _suggestions.AsReadOnly();

    private VoteSuggestion(
        SuggestionKey suggestionId,
        DateTime startedAt,
        string startedBy,
        DateTime? closedAt,
        List<string> suggestions)
    {
        Id = suggestionId;
        StartedAt = startedAt;
        StartedBy = startedBy;
        ClosedAt = closedAt;
        _suggestions = suggestions;
    }

    public static VoteSuggestion CreateNew(int suggestionId, long chatId, string startedBy)
    {
        return new VoteSuggestion(new SuggestionKey(suggestionId, chatId), DateTime.UtcNow, startedBy, null, []);
    }

    public void AddVote(string vote)
    {
        _suggestions.Add(vote);
    }

    public void Close()
    {
        ClosedAt = DateTime.UtcNow;
    }
}
