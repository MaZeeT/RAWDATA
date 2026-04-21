using Domain.Models;

namespace Repositories.Interfaces;

public interface IQuestionRepository
{
    Questions GetQuestion(int questionId);
    IList<Questions> GetQuestions(PagingAttributes pagingAttributes);
    int NumberOfQuestions();
}