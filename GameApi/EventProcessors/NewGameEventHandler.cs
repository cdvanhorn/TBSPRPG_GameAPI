using System;
using System.Threading.Tasks;

using TbspRpgLib.Aggregates;
using TbspRpgLib.Events;

using GameApi.Services;
using GameApi.Entities;

namespace GameApi.EventProcessors {
    public interface INewGameEventHandler : IEventHandler {

    }

    public class NewGameEventHandler : EventHandler, INewGameEventHandler {
        private readonly IGameLogic _gameLogic;

        public NewGameEventHandler(IGameLogic gameLogic) : base() {
            _gameLogic = gameLogic;
        }

        public async Task HandleEvent(GameAggregate gameAggregate, Event evnt) {
            var game = _gameAdapter.ToEntity(gameAggregate);
            Console.WriteLine($"Writing Game {game.Id} {gameAggregate.GlobalPosition}!!");

            //update the game
            await _gameLogic.AddGame(game);
        }
    }
}