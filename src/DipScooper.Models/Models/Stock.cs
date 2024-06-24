using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace DipScooper.Models.Models
{
    public class Stock
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("symbol")]
        public string Symbol { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("market")]
        public string Market { get; set; }
    }
}
