using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IQuestionRepository
{
    Questions GetQuestion(int questionId);
    IList<Questions> GetQuestions(PagingAttributes pagingAttributes);
    int NumberOfQuestions();
}