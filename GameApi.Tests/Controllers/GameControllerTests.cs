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
        // private GameService _gameService;
        // private AdventureService _adventureService;
        //
        // public GameControllerTests() {
        //     _gameService = GameServiceMock.MockGameService();
        //     _adventureService = AdventureServiceMock.MockAdventureService();
        // }

        // [Fact]
        // public async void Start_InvalidAdventureName_Return400() {
        //     //arrange
        //     var controller = new GamesController(_gameService, _adventureService);

        //     //act
        //     var result = await controller.Start("Demmo");

        //     //assert
        //     var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        // }
    }
}