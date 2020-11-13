using System.Collections.Generic;

namespace GameApi.Aggregates {
    public class Aggregate {
        public string Id { get; set; }

        public List<string> EventIds { get; set; }
    }
}