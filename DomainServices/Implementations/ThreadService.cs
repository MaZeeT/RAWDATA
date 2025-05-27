using Domain.Models;
using Domain.Services;
using DomainServices.Interfaces;
using Repositories.Interfaces;

namespace DomainServices.Implementations;

public class ThreadService : IThreadService
{
    private readonly IQuestionRepository _questionRepository;
    private readonly ISharedRepository _sharedRepository;

    public ThreadService(IQuestionRepository questionRepository, ISharedRepository sharedRepository)
    {
        _questionRepository = questionRepository;
        _sharedRepository = sharedRepository;
    }

    public string GetPostType(int postId)
    {
        return _sharedRepository.GetPostType(postId);
    }

    public SinglePost GetPost(int postId)
    {
        return _sharedRepository.GetPost(postId);
    }

    public IList<Posts> GetThread(int questionId)
    {
        return _sharedRepository.GetThread(questionId);
    }

    public IList<Questions> GetQuestions(PagingAttributes pagingAttributes)
    {
        return _questionRepository.GetQuestions(pagingAttributes);
    }

    public int NumberOfQuestions()
    {
        return _questionRepository.NumberOfQuestions();
    }
}