using System;
using GameApi.Entities;
using GameApi.Repositories;
using Xunit;

namespace GameApi.Tests.Repositories
{
    public class AdventureRepositoryTests : InMemoryTest
    {
        #region Setup
        private Guid _testAdventureId;
        
        public AdventureRepositoryTests() : base("AdventureRepositoryTests")
        {
            Seed();
        }
        
        private void Seed()
        {
            using var context = new GameContext(_dbContextOptions);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            
            _testAdventureId = Guid.NewGuid();
            var testadv = new Adventure()
            {
                Id = _testAdventureId,
                Name = "TestAdventure"
            };

            var testadv2 = new Adventure()
            {
                Id = Guid.NewGuid(),
                Name = "TestAdventure2"
            };
            
            context.AddRange(testadv, testadv2);
            context.SaveChanges();
        }
        #endregion

        #region GetAllAdventures
        [Fact]
        public async void GetAllAdventures_ReturnsAll()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var repository = new AdventureRepository(context);
            
            //act
            var adventures = await repository.GetAllAdventures();
            
            //assert
            Assert.Equal(2, adventures.Count);
            Assert.Equal(_testAdventureId, adventures[0].Id);
            Assert.Equal("TestAdventure", adventures[0].Name);
            Assert.Equal("TestAdventure2", adventures[1].Name);
        }
        #endregion
        
        #region GetAdventureById
        [Fact]
        public async void GetAdventureById_Invalid_ReturnsNone()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var repository = new AdventureRepository(context);
            
            //act
            var adventure = await repository.GetAdventureById(Guid.NewGuid());
            
            //assert
            Assert.Null(adventure);
        }
        
        [Fact]
        public async void GetAdventureById_Valid_ReturnsOne()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var repository = new AdventureRepository(context);
            
            //act
            var adventure = await repository.GetAdventureById(_testAdventureId);
            
            //assert
            Assert.NotNull(adventure);
            Assert.Equal(_testAdventureId, adventure.Id);
            Assert.Equal("TestAdventure", adventure.Name);
        }
        #endregion
        
        #region GetAdventureByName
        [Fact]
        public async void GetAdventureByName_Valid_ReturnsOne()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var repository = new AdventureRepository(context);
            
            //act
            var adventure = await repository.GetAdventureByName("testAdventure");
            
            //assert
            Assert.NotNull(adventure);
            Assert.Equal(_testAdventureId, adventure.Id);
            Assert.Equal("TestAdventure", adventure.Name);
        }
        
        [Fact]
        public async void GetAdventureByName_Invalid_ReturnsNone()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var repository = new AdventureRepository(context);
            
            //act
            var adventure = await repository.GetAdventureByName("test");
            
            //assert
            Assert.Null(adventure);
        }
        
        [Fact]
        public async void GetAdventureByName_Null_ReturnsNone()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var repository = new AdventureRepository(context);
            
            //act
            var adventure = await repository.GetAdventureByName(null);
            
            //assert
            Assert.Null(adventure);
        }
        #endregion
    }
}