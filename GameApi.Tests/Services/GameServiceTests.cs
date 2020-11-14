using Xunit;

using GameApi.Services;

using GameApi.Tests.Mocks;

namespace GameApi.Tests.Services {
    public class GameServiceTests {
        private GameService _gameService;

        public GameServiceTests() {
            _gameService = GameServiceMock.MockGameService();
        }

        [Fact]
        public void Start_InvalidUserId_NoEventGenerated() {
            //arrange
            //act
            //assert
        }
    }
}