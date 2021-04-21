using System;
using GameApi.Entities;
using GameApi.Repositories;
using GameApi.Services;
using Xunit;

namespace GameApi.Tests.Services
{
    public class AdventureServiceTests : InMemoryTest
    {
        #region Setup
        private Guid _testAdventureId;

        public AdventureServiceTests() : base("AdventureServiceTests")
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

        private AdventureService CreateService(GameContext context)
        {
            var repository = new AdventureRepository(context);
            return new AdventureService(repository);
        }
        #endregion
        
        #region GetAllAdventures

        [Fact]
        public async void GetAllAdventures_ReturnsAll()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var service = CreateService(context);

            //act
            var adventures = await service.GetAllAdventures();

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
            var service = CreateService(context);
            
            //act
            var adventure = await service.GetAdventureById(Guid.NewGuid());
            
            //assert
            Assert.Null(adventure);
        }
        
        [Fact]
        public async void GetAdventureById_Valid_ReturnsOne()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var service = CreateService(context);
            
            //act
            var adventure = await service.GetAdventureById(_testAdventureId);
            
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
            var service = CreateService(context);
            
            //act
            var adventure = await service.GetAdventureByName("testAdventure");
            
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
            var service = CreateService(context);
            
            //act
            var adventure = await service.GetAdventureByName("test");
            
            //assert
            Assert.Null(adventure);
        }
        
        [Fact]
        public async void GetAdventureByName_Null_ReturnsNone()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var service = CreateService(context);
            
            //act
            var adventure = await service.GetAdventureByName(null);
            
            //assert
            Assert.Null(adventure);
        }
        #endregion
    }
}