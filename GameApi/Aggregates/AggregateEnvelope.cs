using System.Collections.Generic;

namespace GameApi.Aggregates {
    public class AggregateEnvelope {
        public Aggregate Aggregate { get; set; }
        public List<string> EventIds { get; set; }
    }
}