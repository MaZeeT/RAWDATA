using BusinessLogic.Interfaces;
using Domain.Entities;
using Domain.Services;
using Repositories.Interfaces;

namespace BusinessLogic.Implementations;

public class QuestionService : IQuestionService
{
    private readonly ISearchRepository _searchRepository;

    public QuestionService(ISearchRepository searchRepository)
    {
        _searchRepository = searchRepository;
    }

    public IList<Questions> GetQuestions(PagingAttributes pagingAttributes)
    {
        return _searchRepository.GetQuestions(pagingAttributes);
    }
}