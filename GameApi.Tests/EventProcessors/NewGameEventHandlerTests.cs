using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using GameApi.Adapters;
using GameApi.Entities;
using GameApi.EventProcessors;
using GameApi.Repositories;
using GameApi.Services;
using Moq;
using TbspRpgLib.Aggregates;
using TbspRpgLib.Events;
using TbspRpgLib.Events.Game.Content;
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
                }
            };
            
            context.Add(game);
            context.SaveChanges();
        }

        private NewGameEventHandler CreateNewGameEventHandler(GameContext context, ICollection<Event> events)
        {
            var gameRepository = new GameRepository(context);
            var adventureRepository = new AdventureRepository(context);
            var gameService = new GameService(gameRepository);
            var adventureService = new AdventureService(adventureRepository);
            
            var aggregateService = new Mock<IAggregateService>();
            aggregateService.Setup(service => 
                service.AppendToAggregate(It.IsAny<string>(), It.IsAny<Event>(), It.IsAny<bool>())
            ).Callback<string, Event, bool>((type, evnt, n) => events.Add(evnt));

            var gameLogic = new GameLogic(
                new EventAdapter(),
                aggregateService.Object,
                gameService,
                adventureService);

            return new NewGameEventHandler(gameLogic);
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
            var handler = CreateNewGameEventHandler(context, events);
            var gameId = Guid.NewGuid();
            var agg = new GameAggregate()
            {
                Id = gameId.ToString(),
                AdventureId = adventureId.ToString(),
                UserId = _testUserId.ToString(),
                GlobalPosition = 10
            };

            //act
            await handler.HandleEvent(agg, null);
            context.SaveChanges();

            //assert
            var games = context.Games;
            Assert.Equal(2, games.Count());
            Assert.NotNull(games.FirstOrDefault(g => g.Id == _testGameId));
            Assert.NotNull(games.FirstOrDefault(g => g.Id == gameId));
        }

        [Fact]
        public async void HandleEvent_Existing_GameNotAdded()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var events = new List<Event>();
            var handler = CreateNewGameEventHandler(context, events);
            var agg = new GameAggregate()
            {
                Id = _testGameId.ToString(),
                AdventureId = _testAdventureId.ToString(),
                UserId = _testUserId.ToString(),
                GlobalPosition = 10
            };
            
            //act
            await handler.HandleEvent(agg, null);
            context.SaveChanges();

            //assert
            var games = context.Games;
            Assert.Equal(1, games.Count());
            Assert.NotNull(games.FirstOrDefault(g => g.Id == _testGameId));
        }
        #endregion
    }
}