using System.Collections.Generic;
using System;

using Moq;

using GameApi.Services;
using GameApi.Entities;
using GameApi.Repositories;
using GameApi.Adapters;

using TbspRpgLib.Events;

namespace GameApi.Tests.Mocks {
    public class GameServiceMock {
        public static GameService MockGameService() {
            var games = new List<Game>();
            games.Add(new Game {
                Id = new Guid("d4e1de74-7271-4ed8-8e86-35dbb3cd6b3e"),
                UserId = new Guid("d4e1de74-7271-4ed8-8e86-35dbb3cd6b3f"),
                AdventureId = new Guid("d4e1de74-7271-4ed8-8e86-35dbb3cd6b3e"),
                Adventure = new Adventure {
                    Id = new Guid("d4e1de74-7271-4ed8-8e86-35dbb3cd6b3e"),
                    Name = "Demo"
                }
            });
            var mockGameRepo = new Mock<IGameRepository>();
            // mockGameRepo.Setup(repo =>
            //     repo.GetGameByUserIdAndAdventureName(It.IsAny<Guid>(), It.IsAny<string>())
            // ).ReturnsAsync((Guid userid, string name) => 
            //     games.Find(game => game.UserId == userid && game.Adventure.Name == name)
            // );
            mockGameRepo.Setup(repo =>
                repo.GetGameByUserIdAndAdventureId(It.IsAny<Guid>(), It.IsAny<Guid>())
            ).ReturnsAsync((Guid userid, Guid advid) => 
                games.Find(game => game.UserId == userid && game.Adventure.Id == advid)
            );
            return new GameService(mockGameRepo.Object);
        }
    }
}