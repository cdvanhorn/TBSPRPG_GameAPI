using System;
using GameApi.Entities;

namespace GameApi.ViewModels {
    public class GameViewModel {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid AdventureId { get; set; }

        public GameViewModel() {}

        public GameViewModel(Game game) {
            Id = game.Id;
            UserId = game.UserId;
            AdventureId = game.AdventureId;
        }
    }
}