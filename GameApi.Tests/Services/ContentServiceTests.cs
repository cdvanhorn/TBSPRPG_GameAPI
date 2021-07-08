using System;
using System.Linq;
using GameApi.Entities;
using GameApi.Repositories;
using GameApi.Services;
using Xunit;

namespace GameApi.Tests.Services
{
    public class ContentServiceTests : InMemoryTest
    {
        #region Setup
        
        private readonly Guid _testContentId;
        private readonly Guid _testGameId = Guid.NewGuid();
        private readonly Guid _sourceKeyOne = Guid.NewGuid();
        private readonly Guid _sourceKeyTwo = Guid.NewGuid();
        private readonly Guid _sourceKeyLatest = Guid.NewGuid();
        public ContentServiceTests() : base("ContentServiceTests")
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

        private static ContentService CreateService(GameContext context)
        {
            var repository = new ContentRepository(context);
            return new ContentService(repository);
        }
        
        #endregion

        #region GetAllContent

        [Fact]
        public async void GetAllContentForGame_GetsAllContent()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var service = CreateService(context);
            
            //act
            var gameContents = await service.GetAllContentForGame(_testGameId);
            
            //assert
            Assert.Equal(_testGameId, gameContents.FirstOrDefault().GameId);
            Assert.Equal(3, gameContents.Count);
            Assert.Equal(_sourceKeyOne, gameContents.FirstOrDefault().SourceKey);
        }

        #endregion

        #region GetLatestForGame

        [Fact]
        public async void GetLatestForGame_GetsLatest()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var service = CreateService(context);
            
            //act
            var gameContents = await service.GetLatestForGame(_testGameId);
            
            //assert
            Assert.Equal(_testGameId, gameContents.GameId);
            Assert.Equal(_sourceKeyLatest, gameContents.SourceKey);
        }

        #endregion
        
        #region GetPartialContentForGame

        [Fact]
        public async void GetPartialContentForGame_NoDirection_ContentsForward()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var service = CreateService(context);
            
            //act
            var gameContents = await service.GetPartialContentForGame(
                _testGameId, null, null, null);
            
            //assert
            Assert.Equal(_testGameId, gameContents.FirstOrDefault().GameId);
            Assert.Equal(3, gameContents.Count);
            Assert.Equal(_sourceKeyOne, gameContents.FirstOrDefault().SourceKey);
        }
        
        [Fact]
        public async void GetPartialContentForGame_Forward_ContentsForward()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var service = CreateService(context);
            
            //act
            var gameContents = await service.GetPartialContentForGame(
                _testGameId, "f", null, null);
            
            //assert
            Assert.Equal(_testGameId, gameContents.FirstOrDefault().GameId);
            Assert.Equal(3, gameContents.Count);
            Assert.Equal(_sourceKeyOne, gameContents.FirstOrDefault().SourceKey);
        }
        
        [Fact]
        public async void GetPartialContentForGame_ForwardStart_PartialContentsForward()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var service = CreateService(context);
            
            //act
            var gameContents = await service.GetPartialContentForGame(
                _testGameId, "f", 2, null);
            
            //assert
            Assert.Equal(_testGameId, gameContents.FirstOrDefault().GameId);
            Assert.Single(gameContents);
            Assert.Equal(_sourceKeyLatest, gameContents.FirstOrDefault().SourceKey);
        }
        
        [Fact]
        public async void GetPartialContentForGame_ForwardCountStart_PartialContentsForward()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var service = CreateService(context);
            
            //act
            var gameContents = await service.GetPartialContentForGame(
                _testGameId, "f", 1, 2);

            //assert
            Assert.Equal(_testGameId, gameContents.FirstOrDefault().GameId);
            Assert.Equal(2, gameContents.Count);
            Assert.Equal(_sourceKeyTwo, gameContents.FirstOrDefault().SourceKey);
        }
        
        [Fact]
        public async void GetPartialContentForGame_ForwardCount_PartialContentsForward()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var service = CreateService(context);
            
            //act
            var gameContents = await service.GetPartialContentForGame(
                _testGameId, "f", null, 2);
            
            //assert
            Assert.Equal(_testGameId, gameContents.FirstOrDefault().GameId);
            Assert.Equal(2, gameContents.Count);
            Assert.Equal(_sourceKeyOne, gameContents.FirstOrDefault().SourceKey);
        }
        
        [Fact]
        public async void GetPartialContentForGame_Backward_ContentsBackward()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var service = CreateService(context);
            
            //act
            var gameContents = await service.GetPartialContentForGame(
                _testGameId, "b", null, null);
            
            //assert
            Assert.Equal(_testGameId, gameContents.FirstOrDefault().GameId);
            Assert.Equal(3, gameContents.Count);
            Assert.Equal(_sourceKeyLatest, gameContents.FirstOrDefault().SourceKey);
        }
        
        [Fact]
        public async void GetPartialContentForGame_BackwardStart_PartialContentsBackward()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var service = CreateService(context);
            
            //act
            var gameContents = await service.GetPartialContentForGame(
                _testGameId, "b", 1, null);

            //assert
            Assert.Equal(_testGameId, gameContents.FirstOrDefault().GameId);
            Assert.Equal(2, gameContents.Count);
            Assert.Equal(_sourceKeyTwo, gameContents.FirstOrDefault().SourceKey);
        }
        
        [Fact]
        public async void GetPartialContentForGame_BackwardCount_PartialContentsBackward()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var service = CreateService(context);
            
            //act
            var gameContents = await service.GetPartialContentForGame(
                _testGameId, "b", null, 2);

            //assert
            Assert.Equal(_testGameId, gameContents.FirstOrDefault().GameId);
            Assert.Equal(2, gameContents.Count);
            Assert.Equal(_sourceKeyLatest, gameContents.FirstOrDefault().SourceKey);
        }
        
        [Fact]
        public async void GetPartialContentForGame_BackwardStartCount_PartialContentsBackward()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var service = CreateService(context);
            
            //act
            var gameContents = await service.GetPartialContentForGame(
                _testGameId, "b", 1, 2);
            
            //assert
            Assert.Equal(_testGameId, gameContents.FirstOrDefault().GameId);
            Assert.Equal(2, gameContents.Count);
            Assert.Equal(_sourceKeyOne, gameContents[1].SourceKey);
        }
        
        [Fact]
        public async void GetPartialContentForGame_BadDirection_Error()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var service = CreateService(context);

            //act
            //assert
            var exception = Assert.ThrowsAsync<ArgumentException>(() =>
                service.GetPartialContentForGame(
                    _testGameId, "zebra", -3, 2));
        }
        
        #endregion

        #region AddContent

        [Fact]
        public async void AddContent_NotExists_ContentAdded()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var service = CreateService(context);
            var content = new Content()
            {
                Id = Guid.NewGuid(),
                GameId = _testGameId,
                Position = 43,
                SourceKey = Guid.NewGuid()
            };
            
            //act
            await service.AddContent(content);
            
            //assert
            context.SaveChanges();
            Assert.Equal(4, context.Contents.Count(c => c.GameId == _testGameId));
            Assert.NotNull(context.Contents.FirstOrDefault(c => c.Id == content.Id));
        }
        
        [Fact]
        public async void AddContent_Exists_ContentNotAdded()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var service = CreateService(context);
            var content = new Content()
            {
                Id = Guid.NewGuid(),
                GameId = _testGameId,
                Position = 42,
                SourceKey = Guid.NewGuid()
            };
            
            //act
            await service.AddContent(content);
            
            //assert
            context.SaveChanges();
            Assert.Equal(3, context.Contents.Count(c => c.GameId == _testGameId));
            Assert.Null(context.Contents.FirstOrDefault(c => c.Id == content.Id));
        }

        #endregion
        
        #region GetContentForGameAfterPosition

        [Fact]
        public async void GetContentForGameAfterPosition_EarlyPosition_ReturnContent()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var service = CreateService(context);
            
            //act
            var contents = await service.GetContentForGameAfterPosition(_testGameId, 40);
            
            //assert
            Assert.Single(contents);
            Assert.Equal(_sourceKeyLatest, contents[0].SourceKey);
        }
        
        [Fact]
        public async void GetContentForGameAfterPosition_LastPosition_ReturnNoContent()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var service = CreateService(context);
            
            //act
            var contents = await service.GetContentForGameAfterPosition(_testGameId, 42);
            
            //assert
            Assert.Empty(contents);
        }
        
        #endregion
    }
}