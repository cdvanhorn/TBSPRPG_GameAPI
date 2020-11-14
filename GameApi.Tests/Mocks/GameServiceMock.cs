using System.Collections.Generic;

using Moq;

using GameApi.Services;
using GameApi.Entities;
using GameApi.Repositories;
using GameApi.Adapters;

namespace GameApi.Tests.Mocks {
    public class GameServiceMock {
        public static GameService MockGameService() {
            var games = new List<Game>();
            games.Add(new Game {
                Id = "1",
                UserId = "1",
                Adventure = new Adventure {
                    Id = "1234",
                    Name = "Demo"
                }
            });
            var mockGameRepo = new Mock<IGameRepository>();
            mockGameRepo.Setup(repo =>
                repo.GetGameByUserIdAndAdventureName(It.IsAny<string>(), It.IsAny<string>())
            ).ReturnsAsync((string userid, string name) => 
                games.Find(game => game.UserId == userid && game.Adventure.Name == name)
            );
            return new GameService(mockGameRepo.Object, new EventAdapter(), null);
        }
    }
}