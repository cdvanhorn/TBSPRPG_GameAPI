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
            //we need a mock EventService
            var events = new List<Event>();
            var mockEventService = new Mock<IEventService>();
            mockEventService.Setup(service => 
                service.SendEvent(It.IsAny<Event>(), It.IsAny<bool>())
            ).Callback<Event, bool>((evnt, n) => events.Add(evnt));

            return MockGameService(mockEventService.Object);
        }

        public static GameService MockGameService(IEventService eventService) {
            var games = new List<Game>();
            games.Add(new Game {
                Id = new Guid("d4e1de74-7271-4ed8-8e86-35dbb3cd6b3e"),
                UserId = new Guid("d4e1de74-7271-4ed8-8e86-35dbb3cd6b3f"),
                Adventure = new Adventure {
                    Id = new Guid("d4e1de74-7271-4ed8-8e86-35dbb3cd6b3d"),
                    Name = "Demo"
                }
            });
            var mockGameRepo = new Mock<IGameRepository>();
            mockGameRepo.Setup(repo =>
                repo.GetGameByUserIdAndAdventureName(It.IsAny<Guid>(), It.IsAny<string>())
            ).ReturnsAsync((Guid userid, string name) => 
                games.Find(game => game.UserId == userid && game.Adventure.Name == name)
            );
            return new GameService(mockGameRepo.Object, new EventAdapter(), eventService);
        }
    }
}