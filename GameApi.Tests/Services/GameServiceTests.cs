using System;
using System.Collections.Generic;
using System.Text.Json;

using Moq;

using Xunit;

using GameApi.Services;
using TbspRpgLib.Events;
using TbspRpgLib.Events.Content;

using GameApi.Tests.Mocks;

namespace GameApi.Tests.Services {
    public class GameServiceTests {
        private GameService _gameService;
        private AdventureService _adventureService;
        private List<Event> Events;

        public GameServiceTests() {
            Events = new List<Event>();
            var mockEventService = new Mock<IEventService>();
            mockEventService.Setup(service => 
                service.SendEvent(It.IsAny<Event>(), It.IsAny<bool>())
            ).Callback<Event, bool>((evnt, n) => Events.Add(evnt));

            _gameService = GameServiceMock.MockGameService(mockEventService.Object);
            _adventureService = AdventureServiceMock.MockAdventureService();
        }

        [Fact]
        public async void StartGame_GameExists_NoEventGenerated() {
            //arrange
            var adventure = await _adventureService.GetAdventureByName("Demo");
            
            //act
            _gameService.StartGame("1", adventure);

            //assert
            Assert.Empty(Events);
        }

        [Fact]
        public async void StartGame_GameDoesntExist_EventGenerated() {
            //arrange
            var adventure = await _adventureService.GetAdventureByName("Demo");

            //act
            _gameService.StartGame("2", adventure);

            //assert
            Assert.Single<Event>(Events);
            Event evt = Events[0];
            Assert.Equal("new_game", evt.Type);
            var newGame = JsonSerializer.Deserialize<NewGame>(evt.GetDataJson());
            Assert.Equal(evt.GetStreamId(), newGame.Id);
        }
    }
}