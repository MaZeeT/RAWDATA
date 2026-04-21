using Application.Interfaces.Repositories;

namespace Infrastructure.DataAccess.Database;

public class UnitOfWork : IUnitOfWork
{
    private readonly DatabaseContext _context;

    public UnitOfWork(DatabaseContext context)
    {
        _context = context;
    }

    public void Commit()
    {
        _context.SaveChanges();
    }
}