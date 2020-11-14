using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;

using Moq;
using Xunit;

using GameApi.Adapters;
using GameApi.Controllers;
using GameApi.Entities;
using GameApi.Repositories;
using GameApi.Services;

namespace GameApi.Tests.Controllers {
    public class GameControllerTests {
        private GameService _gameService;
        private AdventureService _adventureService;

        public GameControllerTests() {
            _gameService = MockGameService();
            _adventureService = MockAdventureService();
        }

        private AdventureService MockAdventureService() {
            var adventures = new List<Adventure>();
            adventures.Add(new Adventure { Id = "1234", Name = "Demo"});
            var mockAdvRepo = new Mock<IAdventureRepository>();
            mockAdvRepo.Setup(repo => repo.GetAdventureByName(It.IsAny<string>()))
                .ReturnsAsync((string name) => adventures.Find(adv => adv.Name == name));
            return new AdventureService(mockAdvRepo.Object);
        }

        private GameService MockGameService() {
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
            return new GameService(mockGameRepo.Object, new EventAdapter());
        }

        [Fact]
        public async void Start_InvalidAdventureName_Return400() {
            //arrange
            var controller = new GamesController(_gameService, _adventureService);

            //act
            var result = await controller.Start("Demoo");

            //assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}