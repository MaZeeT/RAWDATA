using Domain;
using Domain.Entities;
using Domain.Services;

namespace Repositories.Interfaces;

public interface IShared
{
    string GetPostType(int postId);
    SinglePost GetPost(int postId);
    IList<Posts> GetThread(int questionId);
    Questions GetQuestion(int questionId);
    Answers GetAnswer(int answerId);
    int NumberOfQuestions();
    int GetPagination(int matchcount, PagingAttributes pagingAttributes);
}

