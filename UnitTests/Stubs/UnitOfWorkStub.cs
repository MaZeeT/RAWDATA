using Application.Interfaces.Repositories;
using Xunit; 

namespace UnitTests.Stubs;

public class UnitOfWorkStub : IUnitOfWork
{
    public uint CallCounter { get; private set; }
    public bool HasBeenCalled => CallCounter > 0;

    public void Commit()
    {
        CallCounter++;
    }
}