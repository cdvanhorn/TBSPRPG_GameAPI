using GameApi.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GameApi.Tests
{
    public class InMemoryTest
    {
        protected readonly DbContextOptions<GameContext> _dbContextOptions;

        protected InMemoryTest(string dbName)
        {
            _dbContextOptions = new DbContextOptionsBuilder<GameContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
        }

    }
}