using System.Collections.Generic;

namespace GameApi.Entities {
    public class Game {
        public int Id { get; set; }
        public string Guid { get; set; }
        public int UserId { get; set; }
        public int AdventureId { get; set; }
        public Adventure Adventure { get; set; }
        public List<string> Events { get; set; }
    }
}