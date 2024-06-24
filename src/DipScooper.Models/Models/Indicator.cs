using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace DipScooper.Models.Models
{
    public class Indicator
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("stockId")]
        public string StockId { get; set; }

        [BsonElement("date")]
        public DateTime Date { get; set; }

        [BsonElement("type")]
        public string Type { get; set; } // RSI, SMA50, SMA200

        [BsonElement("value")]
        public double Value { get; set; }
    }
}
