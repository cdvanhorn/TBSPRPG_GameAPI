using GameApi.Aggregates;
using GameApi.Entities;

namespace GameApi.Adapters {
    public interface IGameAggregateAdapter {
        GameAggregate ToAggregate(Game game);
        Game ToEntity(GameAggregate aggregate);
    }

    public class GameAggregateAdapter : IGameAggregateAdapter {
        private IAdventureAggregateAdapter _adventureAggregateAdapter;

        public GameAggregateAdapter(IAdventureAggregateAdapter adventureAggregateAdapter) {
            _adventureAggregateAdapter = adventureAggregateAdapter;
        } 

        public GameAggregate ToAggregate(Game game) {
            GameAggregate agg = new GameAggregate();
            agg.Id = game.Id;
            agg.UserId = game.UserId;
            agg.Adventure = _adventureAggregateAdapter.ToAggregate(game.Adventure);
            return agg;
        }

        public Game ToEntity(GameAggregate aggregate) {
            Game game = new Game();
            game.Id = aggregate.Id;
            game.UserId = aggregate.UserId;
            game.Adventure = _adventureAggregateAdapter.ToEntity(aggregate.Adventure);
            return game;
        }
    }
}