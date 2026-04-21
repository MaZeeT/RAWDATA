namespace Domain.Models;

public class PagingAttributes
{
    private const int MaxPageSize = 100;
    private const int DefPageSize = 10;
    private int _pageSize = DefPageSize;
    private const int FirstPage = 1;
    private int _page = FirstPage;
    public int Page
    {
        get => _page;
        set => _page = Math.Max(value, FirstPage);
    }
    public int PageSize {
        get => _pageSize;
        set => _pageSize = Math.Abs(Math.Min(value, MaxPageSize));
    }
}