using System.Collections.Generic;
using System;

namespace GameApi.Entities {
    public class Game {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid AdventureId { get; set; }
        public Adventure Adventure { get; set; }
        //public List<string> Events { get; set; }
        public ICollection<GameEvent> Events { get; set; }
    }
}