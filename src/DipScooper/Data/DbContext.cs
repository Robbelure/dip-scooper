using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DipScooper.Models;
using MongoDB.Driver;

namespace DipScooper.Data
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
        public IMongoCollection<Indicator> Indicators => _database.GetCollection<Indicator>("Indicators");
    }
}
