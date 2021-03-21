using System;
using System.Collections.Generic;
using System.Text.Json;

using Moq;

using Xunit;

using GameApi.Services;
using GameApi.Adapters;
using TbspRpgLib.Events;
using TbspRpgLib.Aggregates;
using TbspRpgLib.Events.Game.Content;

using GameApi.Tests.Mocks;

namespace GameApi.Tests.Services {
    public class GameLogicTests {
        private List<Event> Events;
        private IGameLogic _gameLogic;
        AdventureService _adventureService;

        public GameLogicTests() {
            Events = new List<Event>();
            var mockEventService = new Mock<IAggregateService>();
            mockEventService.Setup(service => 
                service.AppendToAggregate(It.IsAny<string>(), It.IsAny<Event>(), It.IsAny<bool>())
            ).Callback<string, Event, bool>((type, evnt, n) => Events.Add(evnt));

            GameService gameService = GameServiceMock.MockGameService();
            _adventureService = AdventureServiceMock.MockAdventureService();
            _gameLogic = new GameLogic(
                new EventAdapter(),
                mockEventService.Object,
                gameService,
                _adventureService);
        }

        [Fact]
        public async void StartGame_GameExists_NoEventGenerated() {
            //arrange
            
            //act
            await _gameLogic.StartGame(
                new System.Guid("d4e1de74-7271-4ed8-8e86-35dbb3cd6b3f"),
                new System.Guid("d4e1de74-7271-4ed8-8e86-35dbb3cd6b3e")
            );

            //assert
            Assert.Empty(Events);
        }

        [Fact]
        public async void StartGame_InvalidAdventure_NoEventGenerated() {
            //arrange
            
            //act
            await _gameLogic.StartGame(
                new System.Guid("d4e1de74-7271-4ed8-8e86-35dbb3cd6b4f"),
                new System.Guid("d4e1de74-7271-4ed8-8e86-35dbb3cd6b3a")
            );

            //assert
            Assert.Empty(Events);
        }

        [Fact]
        public async void StartGame_GameDoesntExist_EventGenerated() {
            //act
            await _gameLogic.StartGame(
                new System.Guid("d4e1de74-7271-4ed8-8e86-35dbb3cd6b4f"), 
                new System.Guid("d4e1de74-7271-4ed8-8e86-35dbb3cd6b3e")
            );

            //assert
            Assert.Single<Event>(Events);
            Event evt = Events[0];
            Assert.Equal("game_new", evt.Type);
            var newGame = JsonSerializer.Deserialize<GameNew>(evt.GetDataJson());
            Assert.Equal(evt.GetDataId(), newGame.Id);
        }
    }
}