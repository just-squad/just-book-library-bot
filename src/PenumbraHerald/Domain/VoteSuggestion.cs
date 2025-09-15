namespace PenumbraHerald.Domain;

public record struct SuggestionKey(int SuggestionId, long ChatId); // ЧОКАВО record

public class VoteSuggestion
{
    public SuggestionKey Id { get;  }
    public string StartedBy { get;  }
    public DateTime StartedAt { get;  }
    public DateTime? ClosedAt { get; private set; } // ЧОКАВО private set

    private readonly List<string> _sugggestions; // ЧОКАВО почему имя списка начинается с _
    public IReadOnlyCollection<string> Suggestions => _sugggestions.AsReadOnly(); // ЧОКАВО шо такое IReadOnlyCollection и зачем мы список делаем ReadOnly

    private VoteSuggestion(
        SuggestionKey suggestionId,
        DateTime startedAt,
        string startedBy,
        DateTime? closedAt,
        List<string> sugggestions)
    {
        Id = suggestionId;
        StartedAt = startedAt;
        StartedBy = startedBy;
        ClosedAt = closedAt;
        _sugggestions = sugggestions;
    }

    public static VoteSuggestion CreateNew(int suggestionId, long chatId, string startedBy)
    {
        return new VoteSuggestion(new SuggestionKey(suggestionId, chatId), DateTime.UtcNow, startedBy, null, []);
    }

    public void AddVote(string vote)
    {
        _sugggestions.Add(vote);
    }

    public void Close()
    {
        ClosedAt = DateTime.UtcNow;
    }
}
