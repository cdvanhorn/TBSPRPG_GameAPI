
namespace GameApi.Aggregates {
    public class GameAggregate : Aggregate {
        public string UserId { get; set; }
        public AdventureAggregate Adventure { get; set; }
    }
}