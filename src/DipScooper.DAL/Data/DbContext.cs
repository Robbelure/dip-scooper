using DipScooper.Models.Models;
using MongoDB.Driver;

namespace DipScooper.Dal.Data
{
    public class DbContext
    {
        private readonly IMongoDatabase _database;

        public DbContext()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            _database = client.GetDatabase("DipScooperDB");
        }

        public IMongoCollection<Stock> Stocks => _database.GetCollection<Stock>("Stocks");
        public IMongoCollection<HistoricalData> HistoricalData => _database.GetCollection<HistoricalData>("HistoricalData");

        public async Task SaveHistoricalData(IEnumerable<HistoricalData> data)
        {
            var bulkOps = new List<WriteModel<HistoricalData>>();
            foreach (var record in data)
            {
                var filter = Builders<HistoricalData>.Filter.And(
                    Builders<HistoricalData>.Filter.Eq(hd => hd.StockId, record.StockId),
                    Builders<HistoricalData>.Filter.Eq(hd => hd.Date, record.Date)
                );

                var update = Builders<HistoricalData>.Update
                    .Set(hd => hd.Open, record.Open)
                    .Set(hd => hd.High, record.High)
                    .Set(hd => hd.Low, record.Low)
                    .Set(hd => hd.Close, record.Close)
                    .Set(hd => hd.Volume, record.Volume);

                var updateOne = new UpdateOneModel<HistoricalData>(filter, update) { IsUpsert = true };
                bulkOps.Add(updateOne);
            }

            if (bulkOps.Count > 0)
            {
                await HistoricalData.BulkWriteAsync(bulkOps);
            }
        }
    }
}
