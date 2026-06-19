using API.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace API.Tests;

public abstract class TestBase
{
    protected DatabaseContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>().UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new DatabaseContext(options);
    }
}