using System;
using System.Threading.Tasks;
using GameApi.Entities;
using GameApi.Repositories;
using Xunit;

namespace GameApi.Tests.Repositories
{
    public class GameRepositoryTests : InMemoryTest
    {
        #region Setup
        private Guid _testGameId;
        private Guid _testUserId;
        private Guid _testAdventureId;
        
        public GameRepositoryTests() : base("GameRepositoryTests")
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
        #endregion

        #region GetAllGames
        [Fact]
        public async Task GetAllGames_ReturnsAll()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var repository = new GameRepository(context);
            
            //act
            var games = await repository.GetAllGames();

            //assert
            Assert.Equal(2, games.Count);
            Assert.Equal(_testUserId, games[0].UserId);
            Assert.Equal(_testAdventureId, games[0].AdventureId);
            Assert.Equal(_testGameId, games[0].Id);
            Assert.Equal(_testUserId, games[1].UserId);
        }
        #endregion
        
        #region GetGameById
        [Fact]
        public async Task GetGameById_Valid_ReturnOne()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var repository = new GameRepository(context);
            
            //act
            var game = await repository.GetGameById(_testGameId);
            
            //assert
            Assert.Equal(_testGameId, game.Id);
            Assert.Equal(_testUserId, game.UserId);
            Assert.Equal(_testAdventureId, game.AdventureId);
            Assert.Equal("Test", game.Adventure.Name);
        }
        
        [Fact]
        public async Task GetGameById_InValid_ReturnNone()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var repository = new GameRepository(context);
            
            //act
            var game = await repository.GetGameById(Guid.NewGuid());
            
            //assert
            Assert.Null(game);
        }
        #endregion

        #region AddGame
        [Fact]
        public async Task AddGame_Valid_CreatesOne()
        {
            //arrange
            await using var context = new GameContext(_dbContextOptions);
            var repository = new GameRepository(context);
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
            repository.AddGame(game);
            repository.SaveChanges();
            var dbGame = await repository.GetGameById(game.Id);

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
            var repository = new GameRepository(context);
            
            //act
            await repository.RemoveAllGames();
            repository.SaveChanges();
            var games = await repository.GetAllGames();

            //assert
            Assert.Empty(games);
        }
        #endregion
    }
}