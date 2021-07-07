using System;
using System.Linq;
using GameApi.Entities;
using GameApi.Repositories;
using Xunit;

namespace GameApi.Tests.Repositories
{
    public class ContentRepositoryTests : InMemoryTest
    {
        #region Setup

        private readonly Guid _testContentId;
        private readonly Guid _testGameId = Guid.NewGuid();
        private readonly Guid _testSourceKey = Guid.NewGuid();
        private readonly Guid _testSourceKey2 = Guid.NewGuid();
        
        public ContentRepositoryTests() : base("ContentRepositoryTests")
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
                SourceKey = _testSourceKey
            };

            var tc2 = new Content()
            {
                Id = Guid.NewGuid(),
                GameId = _testGameId,
                Position = 0,
                SourceKey = _testSourceKey2
            };

            var tc3 = new Content()
            {
                Id = Guid.NewGuid(),
                GameId = _testGameId,
                Position = 1,
                SourceKey = Guid.NewGuid()
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

        #endregion
        
        #region GetContentForGame

        [Fact]
        public async void GetContentForGame_NoCountNoOffset_ReturnsAll()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var repository = new ContentRepository(context);
            
            //act
            var contents = await repository.GetContentForGame(_testGameId);
            
            //assert
            Assert.Equal(3, contents.Count);
            Assert.Equal((ulong)0, contents[0].Position);
            Assert.Equal((ulong)42, contents[2].Position);
        }
        
        [Fact]
        public async void GetContentForGame_NoOffset_ReturnPartial()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var repository = new ContentRepository(context);
            
            //act
            var contents = await repository.GetContentForGame(_testGameId, null, 2);
            
            //assert
            Assert.Equal(2, contents.Count);
            Assert.Equal((ulong)0, contents[0].Position);
            Assert.Equal((ulong)1, contents[1].Position);
        }

        [Fact]
        public async void GetContentForGame_NoCount_ReturnPartial()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var repository = new ContentRepository(context);
            
            //act
            var contents = await repository.GetContentForGame(_testGameId, 2);
            
            //assert
            Assert.Single(contents);
            Assert.Equal(_testContentId, contents[0].Id);
            Assert.Equal((ulong)42, contents[0].Position);
        }
        
        [Fact]
        public async void GetContentForGame_OffsetCount_ReturnPartial()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var repository = new ContentRepository(context);
            
            //act
            var contents = await repository.GetContentForGame(_testGameId, 1, 2);
            
            //assert
            Assert.Equal(2, contents.Count);
            Assert.Equal((ulong)1, contents[0].Position);
            Assert.Equal((ulong)42, contents[1].Position);
        }
        
        #endregion
        
        #region GetContentForGameReverse

        [Fact]
        public async void GetContentForGameReverse_NoCountNoOffset_ReturnsAll()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var repository = new ContentRepository(context);
            
            //act
            var contents = await repository.GetContentForGameReverse(_testGameId);
            
            //assert
            Assert.Equal(3, contents.Count);
            Assert.Equal((ulong)42, contents[0].Position);
            Assert.Equal((ulong)0, contents[2].Position);
        }
        
        [Fact]
        public async void GetContentForGameReverse_NoOffset_ReturnPartial()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var repository = new ContentRepository(context);
            
            //act
            var contents = await repository.GetContentForGameReverse(_testGameId, null, 2);
            
            //assert
            Assert.Equal(2, contents.Count);
            Assert.Equal((ulong)42, contents[0].Position);
            Assert.Equal((ulong)1, contents[1].Position);
        }

        [Fact]
        public async void GetContentForGameReverse_NoCount_ReturnPartial()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var repository = new ContentRepository(context);
            
            //act
            var contents = await repository.GetContentForGameReverse(_testGameId, 2);
            
            //assert
            Assert.Single(contents);
            Assert.Equal((ulong)0, contents[0].Position);
        }
        
        [Fact]
        public async void GetContentForGameReverse_OffsetCount_ReturnPartial()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var repository = new ContentRepository(context);
            
            //act
            var contents = await repository.GetContentForGameReverse(_testGameId, 1, 2);
            
            //assert
            Assert.Equal(2, contents.Count);
            Assert.Equal((ulong)1, contents[0].Position);
            Assert.Equal((ulong)0, contents[1].Position);
        }
        
        #endregion

        #region AddContent

        [Fact]
        public async void AddContent_Valid_ContentAdded()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var repository = new ContentRepository(context);
            var content = new Content()
            {
                GameId = _testGameId,
                Position = 43,
                SourceKey = Guid.NewGuid()
            };
            
            //act
            repository.AddContent(content);
            repository.SaveChanges();
            
            //assert
            Assert.Equal(5, context.Contents.Count());
            Assert.NotNull(context.Contents.FirstOrDefault(c => c.Id == content.Id));
        }

        #endregion

        #region GetContentForGameWithPosition

        [Fact]
        public async void GetContentForGameWithPosition_Exists_ReturnsOne()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var repository = new ContentRepository(context);
            
            //act
            var content = await repository.GetContentForGameWithPosition(_testGameId, 42);
            
            //assert
            Assert.NotNull(content);
            Assert.Equal(_testGameId, content.GameId);
            Assert.Equal((ulong)42, content.Position);
        }
        
        [Fact]
        public async void GetContentForGameWithPosition_BadPosition_ReturnsNone()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var repository = new ContentRepository(context);
            
            //act
            var content = await repository.GetContentForGameWithPosition(_testGameId, 5030);
            
            //assert
            Assert.Null(content);
        }
        
        [Fact]
        public async void GetContentForGameWithPosition_BadId_ReturnsNone()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var repository = new ContentRepository(context);
            
            //act
            var content = await repository.GetContentForGameWithPosition(Guid.NewGuid(), 42);
            
            //assert
            Assert.Null(content);
        }

        #endregion

        #region GetContentForGameAfterPosition

        [Fact]
        public async void GetContentForGameAfterPosition_EarlyPosition_ReturnContent()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var repository = new ContentRepository(context);
            
            //act
            var contents = await repository.GetContentForGameAfterPosition(_testGameId, 40);
            
            //assert
            Assert.Single(contents);
            Assert.Equal(_testSourceKey, contents[0].SourceKey);
        }
        
        [Fact]
        public async void GetContentForGameAfterPosition_LastPosition_ReturnNoContent()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var repository = new ContentRepository(context);
            
            //act
            var contents = await repository.GetContentForGameAfterPosition(_testGameId, 42);
            
            //assert
            Assert.Empty(contents);
        }

        #endregion
    }
}