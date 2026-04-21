namespace Domain.Entities;

public class History
{
    public int Id { set; get; }
    public int UserId { set; get; }
    public int PostId { set; get; }
    public string? PostTableName { set; get; }
    public DateTime Date { set; get; }
    public bool IsBookmark { set; get; }
}