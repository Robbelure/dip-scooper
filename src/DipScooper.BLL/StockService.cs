using System.Data;
using DipScooper.BLL.Calculators;
using DipScooper.DAL.API;
using DipScooper.Models;
using DipScooper.Models.APIModels;


namespace DipScooper.BLL
{
    /// <summary>
    /// Klasse som håndterer beregninger og logikk knyttet til aksjeanalyse.
    /// </summary>
    public class StockService
    {
        private ApiClient apiClient;
        private DCFCalculator dcfCalculator;
        private PERatioCalculator peRatioCalculator;
        private PBRatioCalculator pbRatioCalculator;
        private DDMCalculator ddmCalculator;

        public StockService()
        {
            apiClient = new ApiClient();
            dcfCalculator = new DCFCalculator();
            peRatioCalculator = new PERatioCalculator();
            pbRatioCalculator = new PBRatioCalculator();
            ddmCalculator = new DDMCalculator();
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
            results.Add(new CalculationResult("P/E Ratio", peRatio));

            List<double> trailingEPS = await apiClient.GetTrailingEPSAsync(symbol);
            if (trailingEPS == null || trailingEPS.Count < 4)
            {
                throw new Exception("Not enough EPS data available for trailing P/E.");
            }
            double totalEPS = trailingEPS.Sum();
            double trailingPERatio = marketPrice / totalEPS;
            results.Add(new CalculationResult("Trailing P/E Ratio", trailingPERatio));

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
            results.Add(new CalculationResult("P/B Ratio", pbRatio));

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
            results.Add(new CalculationResult("DCF Value", dcfValue));

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
            results.Add(new CalculationResult("Dividend Discount Model Value", ddmValue));

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
