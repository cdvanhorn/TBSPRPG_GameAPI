using TbspRpgLib.Aggregates;
using GameApi.Entities;

namespace GameApi.Adapters {
    public interface IGameAggregateAdapter {
        GameAggregate ToAggregate(Game game);
        Game ToEntity(GameAggregate aggregate);
    }

    public class GameAggregateAdapter : IGameAggregateAdapter {

        public GameAggregate ToAggregate(Game game) {
            GameAggregate agg = new GameAggregate();
            agg.Id = game.Id;
            agg.UserId = game.UserId;
            agg.AdventureId = game.Adventure.Id;
            agg.AdventureName = game.Adventure.Name;
            return agg;
        }

        public Game ToEntity(GameAggregate aggregate) {
            Game game = new Game();
            game.Id = aggregate.Id;
            game.UserId = aggregate.UserId;
            game.Adventure = new Adventure() {
                Id = aggregate.AdventureId,
                Name = aggregate.AdventureName
            };
            return game;
        }
    }
}