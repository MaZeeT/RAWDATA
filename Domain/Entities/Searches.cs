using Domain.Enums;

namespace Domain.Entities;

public class Searches
{
    public int Id { set; get; }
    public int UserId { set; get; }
    public SearchType SearchType { set; get; }
    public string SearchString { set; get; }
    public DateTime Date { set; get; }
}