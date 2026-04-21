using Domain.Models;

namespace Repositories.Interfaces;

public interface ISharedRepository
{
    string GetPostType(int postId);
    SinglePost GetPost(int postId);
    IList<Posts> GetThread(int questionId);

    static int GetPagination(int matchCount, PagingAttributes pagingAttributes)
    {
        var maxPages = (int)Math.Ceiling((double)matchCount / pagingAttributes.PageSize);
        const int minPages = 1;

        Console.WriteLine($"{maxPages} calculated pages.");

        if (pagingAttributes.Page > maxPages)
        {
            pagingAttributes.Page = maxPages;
        }
        else if (pagingAttributes.Page < minPages)
        {
            pagingAttributes.Page = minPages;
        }

        return pagingAttributes.Page - 1; // return 0 indexed
    }
}