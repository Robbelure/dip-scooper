using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using DipScooper.BLL.Calculators;
using DipScooper.Dal.Data;
using DipScooper.DAL.API;
using DipScooper.Models;
using DipScooper.Models.APIModels;
using DipScooper.Models.Models;
using MongoDB.Driver;

namespace DipScooper.BLL
{
    // handles business logic related to stock analysis
    public class StockService
    {
        private ApiClient apiClient;
        private DbContext dbContext;
        private DCFCalculator dcfCalculator;
        private PERatioCalculator peRatioCalculator;
        private PBRatioCalculator pbRatioCalculator;
        private DDMCalculator ddmCalculator;

        public StockService()
        {
            apiClient = new ApiClient();
            dbContext = new DbContext();
            dcfCalculator = new DCFCalculator();
            peRatioCalculator = new PERatioCalculator();
            pbRatioCalculator = new PBRatioCalculator();
            ddmCalculator = new DDMCalculator();
        }

        // sjekker og returnerer aksjeinformasjonen fra databasen
        public async Task<Stock> GetOrCreateStockAsync(string symbol)
        {
            var stock = await dbContext.Stocks.Find(s => s.Symbol == symbol).FirstOrDefaultAsync();
            if (stock == null)
            {
                stock = new Stock { Symbol = symbol, Name = "Unknown", Market = "Unknown" };
                await dbContext.Stocks.InsertOneAsync(stock);
            }
            return stock;
        }

        // laster historiske data fra databasen, og henter manglende data fra APIet hvis nødvendig
        public async Task<List<HistoricalData>> LoadDataWithProgressAsync(string symbol, DateTime startDate, DateTime endDate, IProgress<int> progress)
        {
            progress.Report(0);

            var stock = await GetOrCreateStockAsync(symbol);
            progress.Report(20);

            var historicalDataList = await dbContext.HistoricalData
                .Find(hd => hd.StockId == stock.Id && hd.Date >= startDate && hd.Date <= endDate)
                .ToListAsync();
            progress.Report(40);

            var existingDates = historicalDataList.Select(hd => hd.Date).ToList();
            progress.Report(50);

            var missingRanges = GetMissingDateRanges(startDate, endDate, existingDates);
            progress.Report(60);

            foreach (var (rangeStart, rangeEnd) in missingRanges)
            {
                await RunBackgroundSearchAsync(stock.Id, symbol, rangeStart, rangeEnd);
                progress.Report(80);
            }

            // Hente oppdatert historisk data etter API-kall for manglende data
            historicalDataList = await dbContext.HistoricalData
                .Find(hd => hd.StockId == stock.Id && hd.Date >= startDate && hd.Date <= endDate)
                .ToListAsync();
            progress.Report(100);

            return historicalDataList;
        }

        // henter data fra APIet og setter det inn i databasen
        public async Task<List<HistoricalData>> RunBackgroundSearchAsync(string stockId, string symbol, DateTime startDate, DateTime endDate)
        {
            string jsonData = await apiClient.GetTimeSeriesAsync(symbol, startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
            if (string.IsNullOrEmpty(jsonData))
            {
                throw new Exception("No data retrieved from the API.");
            }

            var historicalData = ProcessJsonData(jsonData, stockId);

            foreach (var data in historicalData)
            {
                var filter = Builders<HistoricalData>.Filter.And(
                    Builders<HistoricalData>.Filter.Eq(hd => hd.StockId, stockId),
                    Builders<HistoricalData>.Filter.Eq(hd => hd.Date, data.Date)
                );

                var update = Builders<HistoricalData>.Update
                    .Set(hd => hd.Open, data.Open)
                    .Set(hd => hd.High, data.High)
                    .Set(hd => hd.Low, data.Low)
                    .Set(hd => hd.Close, data.Close)
                    .Set(hd => hd.Volume, data.Volume);

                var updateOptions = new UpdateOptions { IsUpsert = true };
                await dbContext.HistoricalData.UpdateOneAsync(filter, update, updateOptions);
            }

            var existingData = await dbContext.HistoricalData
                .Find(hd => hd.StockId == stockId && hd.Date >= startDate && hd.Date <= endDate)
                .ToListAsync();

            return existingData;
        }

        private List<(DateTime, DateTime)> GetMissingDateRanges(DateTime startDate, DateTime endDate, List<DateTime> existingDates)
        {
            List<(DateTime, DateTime)> missingRanges = new List<(DateTime, DateTime)>();

            DateTime current = startDate;
            while (current <= endDate)
            {
                if (!existingDates.Contains(current))
                {
                    DateTime rangeStart = current;
                    while (current <= endDate && !existingDates.Contains(current))
                    {
                        current = current.AddDays(1);
                    }
                    DateTime rangeEnd = current.AddDays(-1);
                    missingRanges.Add((rangeStart, rangeEnd));
                }
                current = current.AddDays(1);
            }

            return missingRanges;
        }

        public List<HistoricalData> ProcessJsonData(string jsonData, string stockId)
        {
            if (string.IsNullOrEmpty(jsonData))
                throw new ArgumentException("JSON data is null or empty.");

            var jsonDocument = JsonDocument.Parse(jsonData);
            var root = jsonDocument.RootElement.GetProperty("results");
            List<HistoricalData> historicalDataList = new List<HistoricalData>();

            foreach (var result in root.EnumerateArray())
            {
                try
                {
                    if (result.TryGetProperty("t", out var tProperty) &&
                        result.TryGetProperty("o", out var oProperty) &&
                        result.TryGetProperty("h", out var hProperty) &&
                        result.TryGetProperty("l", out var lProperty) &&
                        result.TryGetProperty("c", out var cProperty) &&
                        result.TryGetProperty("v", out var vProperty))
                    {
                        HistoricalData historicalData = new HistoricalData
                        {
                            StockId = stockId,
                            Date = DateTimeOffset.FromUnixTimeMilliseconds(tProperty.GetInt64()).DateTime,
                            Open = oProperty.GetDouble(),
                            High = hProperty.GetDouble(),
                            Low = lProperty.GetDouble(),
                            Close = cProperty.GetDouble(),
                            Volume = Convert.ToInt64(vProperty.GetDouble())
                        };

                        historicalDataList.Add(historicalData);
                        Debug.WriteLine($"Processed data for date: {historicalData.Date}");
                    }
                    else
                    {
                        Debug.WriteLine("Missing one or more properties in the result element.");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error processing result: {result}");
                    Debug.WriteLine($"Exception: {ex.Message}");
                    Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                    throw;
                }
            }

            Debug.WriteLine("Finished processing JSON data.");
            return historicalDataList;
        }



        public async Task<List<CalculationResult>> CalculatePERatio(string symbol)
        {
            var results = new List<CalculationResult>();
            double marketPrice = await apiClient.GetLatestMarketPriceAsync(symbol);
            double earningsPerShare = await apiClient.GetEarningsPerShareAsync(symbol);

            if (earningsPerShare == 0)
            {
                throw new Exception("EPS data not available.");
            }

            double peRatio = peRatioCalculator.Calculate(marketPrice, earningsPerShare);
            results.Add(new CalculationResult("P/E Ratio", Math.Round(peRatio, 5)));

            List<double> trailingEPS = await apiClient.GetTrailingEPSAsync(symbol);
            if (trailingEPS == null || trailingEPS.Count < 4)
            {
                throw new Exception("Not enough EPS data available for trailing P/E.");
            }
            double totalEPS = trailingEPS.Sum();
            double trailingPERatio = marketPrice / totalEPS;
            results.Add(new CalculationResult("Trailing P/E Ratio", Math.Round(trailingPERatio, 5)));

            return results;
        }

        public async Task<List<CalculationResult>> CalculatePBRatio(string symbol)
        {
            var results = new List<CalculationResult>();
            double marketPrice = await apiClient.GetLatestMarketPriceAsync(symbol);
            double bookValuePerShare = await apiClient.GetBookValuePerShareAsync(symbol);

            if (bookValuePerShare == 0)
            {
                throw new Exception("Book value per share data not available.");
            }

            double pbRatio = pbRatioCalculator.Calculate(marketPrice, bookValuePerShare);
            results.Add(new CalculationResult("P/B Ratio", Math.Round(pbRatio, 5)));

            return results;
        }

        public async Task<List<CalculationResult>> CalculateDCF(string symbol, double discountRate)
        {
            var results = new List<CalculationResult>();
            List<CashFlowResponse> cashFlows = await apiClient.GetCashFlowsAsync(symbol);
            if (cashFlows == null || cashFlows.Count == 0)
            {
                throw new Exception("No cash flow data available.");
            }

            double dcfValue = dcfCalculator.Calculate(cashFlows.Select(cf => cf.NetCashFlow).ToList(), discountRate);
            results.Add(new CalculationResult("DCF Value", Math.Round(dcfValue, 5)));

            return results;
        }

        public async Task<List<CalculationResult>> CalculateDDM(string symbol, double growthRate, double discountRate)
        {
            var results = new List<CalculationResult>();
            double lastDividend = await apiClient.GetLastDividendAsync(symbol);
            if (lastDividend == 0)
            {
                throw new Exception("Dividend data not available.");
            }

            double ddmValue = ddmCalculator.Calculate(lastDividend, growthRate, discountRate);
            results.Add(new CalculationResult("Dividend Discount Model Value", Math.Round(ddmValue, 5)));

            return results;
        }

        public List<CalculationResult> CalculateRSI(List<double> closePrices, int period = 14)
        {
            var results = new List<CalculationResult>();
            if (closePrices.Count < period)
            {
                return results; // Returnerer en tom liste hvis det ikke er nok data
            }

            double gain = 0, loss = 0;

            for (int i = 1; i <= period; i++)
            {
                double change = closePrices[i] - closePrices[i - 1];
                if (change > 0)
                    gain += change;
                else
                    loss -= change;
            }

            gain /= period;
            loss /= period;

            double rs = gain / loss;
            double rsi = 100 - (100 / (1 + rs));
            results.Add(new CalculationResult("RSI", rsi));

            for (int i = period; i < closePrices.Count; i++)
            {
                double change = closePrices[i] - closePrices[i - 1];
                if (change > 0)
                {
                    gain = ((gain * (period - 1)) + change) / period;
                    loss = (loss * (period - 1)) / period;
                }
                else
                {
                    gain = (gain * (period - 1)) / period;
                    loss = ((loss * (period - 1)) - change) / period;
                }

                rs = gain / loss;
                rsi = 100 - (100 / (1 + rs));
                results.Add(new CalculationResult("RSI", rsi));
            }

            return results;
        }

        public List<CalculationResult> CalculateSMA(List<double> prices, int period)
        {
            var results = new List<CalculationResult>();
            List<double> smaValues = new List<double>();
            for (int i = 0; i < prices.Count; i++)
            {
                if (i >= period - 1)
                {
                    double sma = prices.Skip(i - period + 1).Take(period).Average();
                    smaValues.Add(sma);
                }
                else
                {
                    smaValues.Add(double.NaN);  // Sett til NaN for å markere at det ikke er nok data
                    smaValues.Add(double.NaN);  // Sett til NaN for å markere at det ikke er nok data
                }
            }

            for (int i = 0; i < smaValues.Count; i++)
            {
                results.Add(new CalculationResult($"SMA {period} (Day {i + 1})", smaValues[i]));
            }

            return results;
        }
    }
}
