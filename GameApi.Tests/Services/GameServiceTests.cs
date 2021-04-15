using System;
using System.Threading.Tasks;
using GameApi.Entities;
using GameApi.Repositories;
using GameApi.Services;
using Xunit;

namespace GameApi.Tests.Services
{
    public class GameServiceTests : InMemoryTest
    {
        #region Setup
        private Guid _testGameId;
        private Guid _testUserId;
        private Guid _testAdventureId;
        public GameServiceTests() : base("GameServiceTests")
        {
            Seed();
        }

        private void Seed()
        {
            using var context = new GameContext(_dbContextOptions);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            _testGameId = Guid.NewGuid();
            _testUserId = Guid.NewGuid();
            _testAdventureId = Guid.NewGuid();
            var game = new Game()
            {
                Id = _testGameId,
                UserId = _testUserId,
                Adventure = new Adventure()
                {
                    Id = _testAdventureId,
                    Name = "Test"
                }
            };
            
            var game2 = new Game()
            {
                Id = Guid.NewGuid(),
                UserId = _testUserId,
                Adventure = new Adventure()
                {
                    Id = Guid.NewGuid(),
                    Name = "TestTwo"
                }
            };
            
            context.AddRange(game, game2);
            context.SaveChanges();
        }

        private GameService CreateService(GameContext context)
        {
            var repository = new GameRepository(context);
            return new GameService(repository);
        }
        #endregion
        
        #region GetAllGames
        [Fact]
        public async Task GetAll_ReturnsAll()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var service = CreateService(context);

            //act
            var games = await service.GetAll();
            
            //assert
            Assert.Equal(2, games.Count);
            Assert.Equal(_testUserId, games[0].UserId);
            Assert.Equal(_testAdventureId, games[0].AdventureId);
            Assert.Equal(_testGameId, games[0].Id);
            Assert.Equal(_testUserId, games[1].UserId);
        }
        #endregion
        
        #region GetByUserIdAndAdventureId
        [Fact]
        public async Task GetByUserIdAndAdventureId_Valid_ReturnOne()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var service = CreateService(context);
            
            //act
            var game = await service.GetByUserIdAndAdventureId(_testUserId, _testAdventureId);
            
            //assert
            Assert.Equal(_testGameId, game.Id);
            Assert.Equal(_testUserId, game.UserId);
            Assert.Equal(_testAdventureId, game.AdventureId);
            Assert.Equal("Test", game.Adventure.Name);
        }
        
        [Fact]
        public async Task GetByUserIdAndAdventureIdVm_Valid_ReturnOne()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var service = CreateService(context);
            
            //act
            var game = await service.GetByUserIdAndAdventureIdVm(_testUserId, _testAdventureId);
            
            //assert
            Assert.Equal(_testGameId.ToString(), game.Id);
            Assert.Equal(_testUserId.ToString(), game.UserId);
            Assert.Equal(_testAdventureId.ToString(), game.AdventureId);
        }
        #endregion
        
        #region GetGameById
        [Fact]
        public async Task GetGameById_Valid_ReturnOne()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var service = CreateService(context);

            //act
            var game = await service.GetGameById(_testGameId);

            //assert
            Assert.Equal(_testGameId, game.Id);
            Assert.Equal(_testUserId, game.UserId);
            Assert.Equal(_testAdventureId, game.AdventureId);
            Assert.Equal("Test", game.Adventure.Name);
        }
        #endregion
        
        #region AddGame
        [Fact]
        public async Task AddGame_Valid_CreatesOne()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var repository = new GameRepository(context);
            var service = new GameService(repository);
            var game = new Game()
            {
                Id = Guid.NewGuid(),
                UserId = _testUserId,
                Adventure = new Adventure()
                {
                    Id = Guid.NewGuid(),
                    Name = "TestThree"
                }
            };

            //act
            service.AddGame(game);
            repository.SaveChanges();
            var dbGame = await service.GetGameById(game.Id);

            //assert
            Assert.Equal(game.Id, dbGame.Id);
            Assert.Equal(_testUserId, dbGame.UserId);
            Assert.Equal(game.Adventure.Id, dbGame.AdventureId);
            Assert.Equal("TestThree", dbGame.Adventure.Name);
        }
        #endregion

        #region RemoveAllGames
        [Fact]
        public async Task RemoveAllGames_NoGames()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var service = CreateService(context);
            
            //act
            await service.ClearData();
            var games = await service.GetAll();

            //assert
            Assert.Empty(games);
        }
        #endregion
    }
}