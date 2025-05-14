using Domain.Entities;
using Domain.Services;

namespace BusinessLogic.Interfaces;

public interface IQuestionService
{
    IList<Questions> GetQuestions(PagingAttributes pagingAttributes);
}