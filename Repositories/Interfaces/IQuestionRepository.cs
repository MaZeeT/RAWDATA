using Domain.Models;
using Domain.Services;

namespace Repositories.Interfaces;

public interface IQuestionRepository
{
    Questions GetQuestion(int questionId);
    IList<Questions> GetQuestions(PagingAttributes pagingAttributes);
    int NumberOfQuestions();
}