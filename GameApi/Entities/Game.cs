using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

using System.Collections.Generic;

namespace GameApi.Entities {
    public class Game {
        public Game() {
            Events = new List<string>();
        }
        
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("user_id")]
        public string UserId { get; set; }

        [BsonElement("adventure")]
        public Adventure Adventure { get; set; }

        [BsonElement("events")]
        public List<string> Events { get; set; }
    }
}