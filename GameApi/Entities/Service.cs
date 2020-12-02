using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

using System.Collections.Generic;

namespace GameApi.Entities {
    public class Service {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("event_prefix")]
        public string EventPrefix { get; set;}

        [BsonElement("event_indexes")]
        public List<EventIndex> EventIndexes { get; set; }
    }
}