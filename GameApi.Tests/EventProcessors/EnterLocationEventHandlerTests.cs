using System;
using System.Threading.Tasks;
using GameApi.EventProcessors;
using GameApi.Repositories;
using GameApi.Services;
using TbspRpgLib.Aggregates;
using Xunit;

namespace GameApi.Tests.EventProcessors
{
    public class EnterLocationEventHandlerTests : InMemoryTest
    {
        #region Setup

        public EnterLocationEventHandlerTests() : base("EnterLocationEventHandlerTests")
        {
            Seed();
        }
        
        private void Seed()
        {
            using var context = new GameContext(_dbContextOptions);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            context.SaveChanges();
        }

        private EnterLocationEventHandler CreateHandler(GameContext context)
        {
            var repository = new GameRepository(context);
            var service = new GameService(repository);
            return new EnterLocationEventHandler(service);
        }
        #endregion
        
        #region HandleEvent
        [Fact]
        public async void HandleEvent_NotInDb_ThrowException()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var handler = CreateHandler(context);
            var gameId = Guid.NewGuid();
            var agg = new GameAggregate()
            {
                Id = gameId.ToString(),
                AdventureId = Guid.NewGuid().ToString(),
                UserId = Guid.NewGuid().ToString(),
                GlobalPosition = 10
            };
            
            //act
            Task Act() => handler.HandleEvent(agg, null);

            //assert
            var exception = await Assert.ThrowsAsync<Exception>(Act);
        }
        #endregion
    }
}