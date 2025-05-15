using Domain;
using Domain.Entities;
using Domain.Services;

namespace Repositories.Interfaces;

public interface ISharedRepository
{
    string GetPostType(int postId);
    SinglePost GetPost(int postId);
    IList<Posts> GetThread(int questionId);

    Answers GetAnswer(int answerId);

    static int GetPagination(int matchcount, PagingAttributes pagingAttributes)
    {
        //calc max pages and set requested page to last page if out of bounds
        var maxPages = (int)Math.Ceiling((double)matchcount / pagingAttributes.PageSize);
        var minPages = 1;

        System.Console.WriteLine($"{maxPages} calculated pages.");

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