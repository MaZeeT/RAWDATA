using Domain.Entities;
using Domain.Services;

namespace BusinessLogic.Interfaces;

public interface IThreadService
{
    string GetPostType(int postId);
    SinglePost GetPost(int postId);
    IList<Posts> GetThread(int questionId);
    IList<Questions> GetQuestions(PagingAttributes pagingAttributes);
    int NumberOfQuestions();
}