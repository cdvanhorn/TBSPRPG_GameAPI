using GameApi.Entities;
using GameApi.Aggregates;

namespace GameApi.Adapters {
    public interface IAdventureAggregateAdapter {
        AdventureAggregate ToAggregate(Adventure adventure);
        Adventure ToEntity(AdventureAggregate aggregate);
    }

    public class AdventureAggregateAdapter : IAdventureAggregateAdapter {
        public AdventureAggregate ToAggregate(Adventure adventure) {
            AdventureAggregate agg = new AdventureAggregate();
            agg.Id = adventure.Id;
            agg.Name = adventure.Name;
            return agg;
        }

        public Adventure ToEntity(AdventureAggregate aggregate) {
            Adventure adv = new Adventure();
            adv.Id = aggregate.Id;
            adv.Name = aggregate.Name;
            return adv;
        }
    }
}