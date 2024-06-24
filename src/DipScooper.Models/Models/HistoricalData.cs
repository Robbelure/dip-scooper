using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace DipScooper.Models.Models
{
    public class HistoricalData
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("stockId")]
        public string StockId { get; set; }

        [BsonElement("date")]
        public DateTime Date { get; set; }

        [BsonElement("open")]
        public double Open { get; set; }

        [BsonElement("high")]
        public double High { get; set; }

        [BsonElement("low")]
        public double Low { get; set; }

        [BsonElement("close")]
        public double Close { get; set; }

        [BsonElement("volume")]
        public long Volume { get; set; }
    }
}
