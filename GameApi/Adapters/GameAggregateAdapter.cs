using TbspRpgLib.Aggregates;
using GameApi.Entities;

using System;

namespace GameApi.Adapters {
    public interface IGameAggregateAdapter {
        GameAggregate ToAggregate(Game game);
        Game ToEntity(GameAggregate aggregate);
    }

    public class GameAggregateAdapter : IGameAggregateAdapter {

        public GameAggregate ToAggregate(Game game) {
            GameAggregate agg = new GameAggregate();
            agg.Id = game.Id.ToString();
            agg.UserId = game.UserId.ToString();
            agg.AdventureId = game.AdventureId.ToString();
            agg.AdventureName = game.Adventure.Name;
            return agg;
        }

        public Game ToEntity(GameAggregate aggregate) {
            Game game = new Game();
            game.Id = Guid.Parse(aggregate.Id);
            game.UserId = Guid.Parse(aggregate.UserId);
            game.Adventure = new Adventure() {
                Id = Guid.Parse(aggregate.AdventureId),
                Name = aggregate.AdventureName
            };
            return game;
        }
    }
}