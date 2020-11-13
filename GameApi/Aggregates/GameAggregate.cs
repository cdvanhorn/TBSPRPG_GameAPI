
namespace GameApi.Aggregates {
    public class GameAggregate : Aggregate {
        public string UserId { get; set; }
        public string AdventureId { get; set; }
        public string AdventureName { get; set; }
    }
}