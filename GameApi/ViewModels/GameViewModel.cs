using GameApi.Entities;

namespace GameApi.ViewModels {
    public class GameViewModel {
        public string Id { get; set; }

        public string UserId { get; set; }

        public string AdventureId { get; set; }

        public GameViewModel() {}

        public GameViewModel(Game game) {
            Id = game.Id.ToString();
            UserId = game.UserId.ToString();
            AdventureId = game.AdventureId.ToString();
        }
    }
}