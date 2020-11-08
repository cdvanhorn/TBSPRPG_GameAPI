using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace GameApi.Entities {
    public class Game {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string Adventure_Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string User_Id { get; set; }
    }
}