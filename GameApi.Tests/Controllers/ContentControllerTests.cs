using System;
using System.Linq;
using GameApi.Controllers;
using GameApi.Entities;
using GameApi.Repositories;
using GameApi.Services;
using GameApi.ViewModels;
using Microsoft.AspNetCore.Mvc;
using TbspRpgLib.InterServiceCommunication.RequestModels;
using Xunit;

namespace GameApi.Tests.Controllers
{
    public class ContentControllerTests : InMemoryTest
    {
        #region Setup

        private readonly Guid _testContentId;
        private readonly Guid _testGameId = Guid.NewGuid();
        private readonly Guid _sourceKeyOne = Guid.NewGuid();
        private readonly Guid _sourceKeyTwo = Guid.NewGuid();
        private readonly Guid _sourceKeyLatest = Guid.NewGuid();
        public ContentControllerTests() : base("ContentControllerTests")
        {
            _testContentId = Guid.NewGuid();
            Seed();
        }
        
        private void Seed()
        {
            using var context = new GameContext(_dbContextOptions);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var tc = new Content()
            {
                Id = _testContentId,
                GameId = _testGameId,
                Position = 42,
                SourceKey = _sourceKeyLatest
            };

            var tc2 = new Content()
            {
                Id = Guid.NewGuid(),
                GameId = _testGameId,
                Position = 0,
                SourceKey = _sourceKeyOne
            };

            var tc3 = new Content()
            {
                Id = Guid.NewGuid(),
                GameId = _testGameId,
                Position = 1,
                SourceKey = _sourceKeyTwo
            };

            var tc4 = new Content()
            {
                Id = Guid.NewGuid(),
                GameId = Guid.NewGuid(),
                Position = 43,
                SourceKey = Guid.NewGuid()
            };
            
            context.Contents.AddRange(tc, tc2, tc3, tc4);
            context.SaveChanges();
        }

        private static ContentController CreateController(GameContext context)
        {
            var repository = new ContentRepository(context);
            var service = new ContentService(repository);
            return new ContentController(service);
        }

        #endregion

        #region GetLatestForGame
        
        [Fact]
        public async void GetLatestForGame_GetLatestContent()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var controller = CreateController(context);
            
            //act
            var result = await controller.GetLatestForGame(_testGameId);
            
            //assert
            var okObjectResult = result as OkObjectResult;
            Assert.NotNull(okObjectResult);
            var apiContents = okObjectResult.Value as ContentViewModel;
            Assert.NotNull(apiContents);
            Assert.Single(apiContents.SourceKeys);
            Assert.Equal(_sourceKeyLatest, apiContents.SourceKeys.First());
        }
        
        [Fact]
        public async void GetLatestForGame_NoGame_ReturnEmptyReponse()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var controller = CreateController(context);
            
            //act
            var result = await controller.GetLatestForGame(Guid.NewGuid());
            
            //assert
            var okResult = result as OkResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
        }
        
        #endregion

        #region FilterContent

        [Fact]
        public async void FilterContent_Valid_GetRequestedContent()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var controller = CreateController(context);
            
            //act
            var result = await controller.FilterContent(_testGameId, new ContentFilterRequest()
            {
                Direction = "f",
                Start = 1,
                Count = 1
            });
            
            //assert
            var okObjectResult = result as OkObjectResult;
            Assert.NotNull(okObjectResult);
            var apiContents = okObjectResult.Value as ContentViewModel;
            Assert.NotNull(apiContents);
            Assert.Single(apiContents.SourceKeys);
            Assert.Equal(_sourceKeyTwo, apiContents.SourceKeys.First());
        }
        
        [Fact]
        public async void FilterContent_GameDoesntExist_ReturnEmptyResponse()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var controller = CreateController(context);
            
            //act
            var result = await controller.FilterContent(Guid.NewGuid(), new ContentFilterRequest()
            {
                Direction = "f",
                Start = 1,
                Count = 1
            });
            
            //assert
            var okResult = result as OkResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
        }
        
        [Fact]
        public async void FilterContent_BadDirection_ReturnBadRequest()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var controller = CreateController(context);
            
            //act
            var result = await controller.FilterContent(_testGameId, new ContentFilterRequest()
            {
                Direction = "zebra",
                Start = 2,
                Count = 2
            });
            
            //assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        #endregion
        
        #region GetContentAfterPosition

        [Fact]
        public async void GetContentAfterPosition_Valid_ReturnsContent()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var controller = CreateController(context);
            
            //act
            var result = await controller.GetContentAfterPosition(_testGameId, 40);

            //assert
            var okObjectResult = result as OkObjectResult;
            Assert.NotNull(okObjectResult);
            var apiContents = okObjectResult.Value as ContentViewModel;
            Assert.NotNull(apiContents);
            Assert.Single(apiContents.SourceKeys);
            Assert.Equal(_sourceKeyLatest, apiContents.SourceKeys.First());
        }
        
        [Fact]
        public async void GetContentAfterPosition_NoContent_EmptyResponse()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var controller = CreateController(context);
            
            //act
            var result = await controller.GetContentAfterPosition(_testGameId, 42);
            
            //assert
            var okResult = result as OkResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
        }

        #endregion
    }
}