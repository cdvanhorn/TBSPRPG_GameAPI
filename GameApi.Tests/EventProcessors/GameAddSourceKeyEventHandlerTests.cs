using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameApi.Adapters;
using GameApi.EventProcessors;
using GameApi.Repositories;
using GameApi.Services;
using Moq;
using TbspRpgLib.Aggregates;
using TbspRpgLib.Events;
using TbspRpgLib.Tests.Mocks;
using Xunit;

namespace GameApi.Tests.EventProcessors
{
    public class GameAddSourceKeyEventHandlerTests : InMemoryTest
    {
        #region Setup

        public GameAddSourceKeyEventHandlerTests() : base("GameAddSourceKeyEventHandlerTests")
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

        private GameAddSourceKeyEventHandler CreateHandler(GameContext context)
        {
            var repository = new GameRepository(context);
            var gameService = new GameService(repository);
            var service = new GameService(repository);
            var contentService = new ContentService(
                new ContentRepository(context));
            var adventureService = new AdventureService(
                new AdventureRepository(context), MockAdventureServiceLink.CreateMockAdventureServiceLink());
            
            var aggregateService = new Mock<IAggregateService>();
            var events = new List<Event>();
            aggregateService.Setup(service => 
                service.AppendToAggregate(It.IsAny<string>(), It.IsAny<Event>(), It.IsAny<bool>())
            ).Callback<string, Event, bool>((type, evnt, n) => events.Add(evnt));
            
            var gameLogic = new GameLogic(
                new EventAdapter(),
                aggregateService.Object,
                gameService,
                adventureService);
            var eventHandlerServices = new EventHandlerServices(
                adventureService,
                contentService,
                gameLogic,
                gameService,
                new EventAdapter(),
                aggregateService.Object
            );
            return new GameAddSourceKeyEventHandler(eventHandlerServices);
        }
        #endregion
    }
}