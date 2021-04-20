using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

using Moq;

using Xunit;

using GameApi.Services;
using GameApi.Adapters;
using GameApi.Entities;
using GameApi.Repositories;
using TbspRpgLib.Events;
using TbspRpgLib.Aggregates;
using TbspRpgLib.Events.Game.Content;

namespace GameApi.Tests.Services {
    public class GameLogicTests : InMemoryTest{
        #region Setup
        private Guid _testGameId;
        private Guid _testUserId;
        private Guid _testAdventureId;

        public GameLogicTests() : base("GameLogicTests") {
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
            
            var game2 = new Game()
            {
                Id = Guid.NewGuid(),
                UserId = _testUserId,
                Adventure = new Adventure()
                {
                    Id = Guid.NewGuid(),
                    Name = "TestTwo"
                }
            };
            
            context.AddRange(game, game2);
            context.SaveChanges();
        }

        private static GameLogic CreateGameLogic(GameContext context, ICollection<Event> events)
        {
            var gameRepository = new GameRepository(context);
            var adventureRepository = new AdventureRepository(context);
            var gameService = new GameService(gameRepository);
            var adventureService = new AdventureService(adventureRepository);
            
            var aggregateService = new Mock<IAggregateService>();
            aggregateService.Setup(service => 
                service.AppendToAggregate(It.IsAny<string>(), It.IsAny<Event>(), It.IsAny<bool>())
            ).Callback<string, Event, bool>((type, evnt, n) => events.Add(evnt));

            return new GameLogic(
                new EventAdapter(),
                aggregateService.Object,
                gameService,
                adventureService);
        }
        #endregion

        #region StartGame
        [Fact]
        public async void StartGame_GameExists_NoEventGenerated() {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var events = new List<Event>();
            var gameLogic = CreateGameLogic(context, events);
            
            //act
            await gameLogic.StartGame(_testUserId, _testAdventureId);

            //assert
            Assert.Empty(events);
        }

        [Fact]
        public async void StartGame_InvalidAdventure_NoEventGenerated() {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var events = new List<Event>();
            var gameLogic = CreateGameLogic(context, events);
            
            //act
            await gameLogic.StartGame(_testUserId, Guid.NewGuid());

            //assert
            Assert.Empty(events);
        }

        [Fact]
        public async void StartGame_GameDoesntExist_EventGenerated() {
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
            var gameLogic = CreateGameLogic(context, events);
            
            //act
            await gameLogic.StartGame(_testUserId, adventureId);
        
            //assert
            Assert.Single<Event>(events);
            Event evt = events[0];
            Assert.Equal("game_new", evt.Type);
            var newGame = JsonSerializer.Deserialize<GameNew>(evt.GetDataJson());
            Assert.Equal(evt.GetDataId(), newGame.Id);
        }
        #endregion
        
        #region AddGame
        [Fact]
        public async void AddGame_GameInDb_NotAdded()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var events = new List<Event>();
            var gameLogic = CreateGameLogic(context, events);
            
            //act
            await gameLogic.AddGame(context.Games.FirstOrDefault(g => g.Id == _testGameId));
            
            //assert
            var games = context.Games;
            Assert.Equal(2, games.Count());
        }

        [Fact]
        public async void AddGame_InvalidAdventureId_NotAdded()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var events = new List<Event>();
            var gameLogic = CreateGameLogic(context, events);
            var game = new Game()
            {
                Id = Guid.NewGuid(),
                AdventureId = Guid.NewGuid(),
                UserId = _testUserId
            };
            
            //act
            await gameLogic.AddGame(game);
            
            //assert
            var games = context.Games;
            Assert.Equal(2, games.Count());
        }

        [Fact]
        public async void AddGame_Valid_GameAdded()
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
            var gameLogic = CreateGameLogic(context, events);
            var gameId = Guid.NewGuid();
            var game = new Game()
            {
                Id = gameId,
                AdventureId = adventureId,
                UserId = _testUserId
            };
            
            //act
            await gameLogic.AddGame(game);
            context.SaveChanges();
            
            //assert
            var games = context.Games;
            Assert.Equal(3, games.Count());
            var dbgame = context.Games.FirstOrDefault(g => g.Id == gameId);
            Assert.Equal(gameId, dbgame.Id);
            Assert.Equal(_testUserId, dbgame.UserId);
            Assert.Equal(adventureId, dbgame.AdventureId);
        }
        #endregion
    }
}