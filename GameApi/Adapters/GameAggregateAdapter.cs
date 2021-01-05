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
            agg.Id = game.Guid;
            agg.UserId = game.UserId.ToString();
            agg.AdventureId = game.AdventureId.ToString();
            agg.AdventureName = game.Adventure.Name;
            return agg;
        }

        public Game ToEntity(GameAggregate aggregate) {
            Game game = new Game();
            game.Id = int.Parse(aggregate.Id);
            game.UserId = int.Parse(aggregate.UserId);
            game.Adventure = new Adventure() {
                Id = int.Parse(aggregate.AdventureId),
                Name = aggregate.AdventureName
            };
            return game;
        }
    }
}