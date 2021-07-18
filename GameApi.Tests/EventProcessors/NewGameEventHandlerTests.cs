using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using GameApi.Adapters;
using GameApi.Entities;
using GameApi.Entities.AdventureService;
using GameApi.EventProcessors;
using GameApi.Repositories;
using GameApi.Services;
using Moq;
using TbspRpgLib.Aggregates;
using TbspRpgLib.Events;
using TbspRpgLib.Events.Game;
using TbspRpgLib.InterServiceCommunication;
using TbspRpgLib.Tests.Mocks;
using Xunit;

namespace GameApi.Tests.EventProcessors
{
    public class NewGameEventHandlerTests : InMemoryTest
    {
        #region Setup
        private Guid _testGameId;
        private Guid _testUserId;
        private Guid _testAdventureId;
        
        public NewGameEventHandlerTests() : base("NewGameEventHandlerTests")
        {
            Seed();
        }
        
        private void Seed()
        {
            using var context = new GameContext(_dbContextOptions);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            
            _testGameId = Guid.NewGuid();
            _testUserId = Guid.NewGuid();
            _testAdventureId = Guid.NewGuid();
            var game = new Game()
            {
                Id = _testGameId,
                UserId = _testUserId,
                Adventure = new Adventure()
                {
                    Id = _testAdventureId,
                    Name = "Test"
                },
                Contents = new List<Content>()
                {
                    new Content()
                    {
                        GameId = _testGameId,
                        Position = 0,
                        SourceKey = Guid.NewGuid()
                    }
                }
            };
            
            context.Add(game);
            context.SaveChanges();
        }

        private NewGameEventHandler CreateNewGameEventHandler(GameContext context,
            ICollection<Event> events, IscResponse adventureServiceLinkResponse = null)
        {
            var gameRepository = new GameRepository(context);
            var adventureRepository = new AdventureRepository(context);
            var gameService = new GameService(gameRepository);
            var adventureService = new AdventureService(
                adventureRepository, MockAdventureServiceLink.CreateMockAdventureServiceLink(
                    null, adventureServiceLinkResponse));
            var contentService = new ContentService(
                new ContentRepository(context));
            
            var aggregateService = new Mock<IAggregateService>();
            aggregateService.Setup(service => 
                service.AppendToAggregate(It.IsAny<string>(), It.IsAny<Event>(), It.IsAny<ulong>())
            ).Callback<string, Event, ulong>((type, evnt, n) => events.Add(evnt));
            var aggregateServiceObject = aggregateService.Object;

            var gameLogic = new GameLogic(
                new EventAdapter(),
                aggregateServiceObject,
                gameService,
                adventureService);

            var eventHandlerServices = new EventHandlerServices(
                adventureService,
                contentService,
                gameLogic,
                gameService,
                new EventAdapter(),
                aggregateServiceObject
            );

            return new NewGameEventHandler(eventHandlerServices);
        }
        #endregion
        
        #region HandleEvent
        [Fact]
        public async void HandleEvent_NewGame_GameAdded()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var adventureId = Guid.NewGuid();
            context.Adventures.Add(new Adventure()
            {
                Id = adventureId,
                Name = "NotStartedAdventure"
            });
            context.SaveChanges();
            var events = new List<Event>();
            var gameId = Guid.NewGuid();
            var agg = new GameAggregate()
            {
                Id = gameId.ToString(),
                AdventureId = adventureId.ToString(),
                UserId = _testUserId.ToString(),
                GlobalPosition = 10
            };
            var adventureIsc = new AdventureIsc()
            {
                Id = adventureId,
                Name = "NotStartedAdventure",
                SourceKey = Guid.NewGuid()
            };
            var handler = CreateNewGameEventHandler(context, events, new IscResponse()
            {
                Content = JsonSerializer.Serialize(adventureIsc),
                IsSuccessful = true,
                StatusCode = 200
            });

            //act
            await handler.HandleEvent(agg, new GameNewEvent()
            {
                StreamPosition = 1
            });
            context.SaveChanges();

            //assert
            var games = context.Games;
            Assert.Equal(2, games.Count());
            Assert.NotNull(games.FirstOrDefault(g => g.Id == _testGameId));
            Assert.NotNull(games.FirstOrDefault(g => g.Id == gameId));
            //there should be an event
            Assert.Single(events);
            Assert.IsType<GameAddSourceKeyEvent>(events[0]);
        }

        [Fact]
        public async void HandleEvent_Existing_GameNotAdded()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var events = new List<Event>();
            var agg = new GameAggregate()
            {
                Id = _testGameId.ToString(),
                AdventureId = _testAdventureId.ToString(),
                UserId = _testUserId.ToString(),
                GlobalPosition = 10
            };
            var adventureIsc = new AdventureIsc()
            {
                Id = _testAdventureId,
                Name = "TestAdventure",
                SourceKey = Guid.NewGuid()
            };
            var handler = CreateNewGameEventHandler(context, events, new IscResponse()
            {
                Content = JsonSerializer.Serialize(adventureIsc),
                IsSuccessful = true,
                StatusCode = 200
            });
            
            //act
            await handler.HandleEvent(agg, new GameNewEvent()
            {
                StreamPosition = 0
            });
            context.SaveChanges();

            //assert
            var games = context.Games;
            Assert.Equal(1, games.Count());
            Assert.NotNull(games.FirstOrDefault(g => g.Id == _testGameId));
            //make sure still only one content object
            Assert.Single(events);
        }
        #endregion
    }
}