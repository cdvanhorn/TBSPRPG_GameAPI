using GameApi.Events;

namespace GameApi.Events.Content {
    public class NewGame : EventContent {
        public string UserId { get; set; }

        public string AdventureName { get; set; }

        public string AdventureId { get; set; }
    }
}