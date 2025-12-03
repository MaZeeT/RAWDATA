namespace Domain.Enums;

public enum SearchType
{
    Tfidf = 0,
    ExactMatch = 1,
    Simple = 2,
    BestMatch = 3,
    WordsTfidf = 4,
    WordsBest = 5
}