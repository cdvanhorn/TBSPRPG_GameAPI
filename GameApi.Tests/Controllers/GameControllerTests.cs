using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

using Microsoft.AspNetCore.Mvc;

using Moq;
using Xunit;

using GameApi.Adapters;
using GameApi.Controllers;
using GameApi.Entities;
using GameApi.Repositories;
using GameApi.Services;
using GameApi.ViewModels;
using Microsoft.AspNetCore.Http;
using TbspRpgLib.Aggregates;
using TbspRpgLib.Events;
using TbspRpgLib.Events.Game.Content;

namespace GameApi.Tests.Controllers {
    public class GameControllerTests : InMemoryTest {
        
        #region Setup
        private Guid _testGameId;
        private Guid _testUserId;
        private Guid _testAdventureId;
        
        public GameControllerTests() : base("GameControllerTests")
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
        
        private GamesController CreateController(GameContext context, ICollection<Event> events)
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
            
            var controller = new GamesController(gameService, gameLogic)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
            
            controller.ControllerContext.HttpContext.Items = new Dictionary<object, object>()
            {
                {AuthorizeAttribute.USER_ID_CONTEXT_KEY, _testUserId.ToString()}
            };
            
            return controller;
        }
        #endregion
        
        #region Start
        [Fact]
        public async void Start_InvalidAdventureId_ReturnBadRequest()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var events = new List<Event>();
            var controller = CreateController(context, events);

            //act
            var result = await controller.Start(Guid.NewGuid());
            
            //assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.Equal(400, badRequestResult.StatusCode);
        }
        
        [Fact]
        public async void Start_Valid_AcceptedStartsGame()
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
            var controller = CreateController(context, events);
            
            //act
            var result = await controller.Start(adventureId);
            
            //assert
            var acceptedResult = result as AcceptedResult;
            Assert.NotNull(acceptedResult);
            Assert.Equal(202, acceptedResult.StatusCode);
            
            //assert event created
            Assert.Single<Event>(events);
            var evt = events[0];
            Assert.Equal("game_new", evt.Type);
            var newGame = JsonSerializer.Deserialize<GameNew>(evt.GetDataJson());
            Assert.Equal(evt.GetDataId(), newGame.Id);
        }
        #endregion
        
        #region GetAll
        [Fact]
        public async void GetAll_ReturnAll()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var events = new List<Event>();
            var controller = CreateController(context, events);
            
            //act
            var result = await controller.GetAll();

            //assert
            var okObjectResult = result as OkObjectResult;
            Assert.NotNull(okObjectResult);
            var games = okObjectResult.Value as IEnumerable<GameViewModel>;
            Assert.NotNull(games);
            Assert.Equal(2, games.Count());
        }
        #endregion
        
        #region GetByAdventure
        [Fact]
        public async void GetByAdventure_Valid_ReturnOne()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var events = new List<Event>();
            var controller = CreateController(context, events);
            
            //act
            var result = await controller.GetByAdventure(_testAdventureId);
            
            //assert
            var okObjectResult = result as OkObjectResult;
            Assert.NotNull(okObjectResult);
            var game = okObjectResult.Value as GameViewModel;
            Assert.NotNull(game);
            Assert.Equal(_testGameId, game.Id);
            Assert.Equal(_testAdventureId, game.AdventureId);
        }

        [Fact]
        public async void GetByAdventure_NoGame_ReturnNone()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var events = new List<Event>();
            var controller = CreateController(context, events);

            //act
            var result = await controller.GetByAdventure(Guid.NewGuid());

            //assert
            var okObjectResult = result as OkObjectResult;
            Assert.NotNull(okObjectResult);
            var game = okObjectResult.Value as GameViewModel;
            Assert.Null(game);
        }
        #endregion
    }
}