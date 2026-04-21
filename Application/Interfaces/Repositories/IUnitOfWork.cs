namespace Application.Interfaces.Repositories;

public interface IUnitOfWork
{
    void Commit();
}