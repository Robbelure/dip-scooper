using System.Data;
using DipScooper.Calculators;
using DipScooper.Models.ApiResponseModels;

namespace DipScooper.Services
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

        /// <summary>
        /// Utfører P/E-ratio beregning og legger resultatene til i DataGridView.
        /// </summary>
        /// <param name="symbol">Aksjesymbol.</param>
        /// <param name="dataGridView_analyze">DataGridView for å vise analyse resultatene.</param>
        public async Task CalculatePERatio(string symbol, DataGridView dataGridView_analyze)
        {
            double marketPrice = await apiClient.GetLatestMarketPriceAsync(symbol);
            double earningsPerShare = await apiClient.GetEarningsPerShareAsync(symbol);

            if (earningsPerShare == 0)
            {
                throw new Exception("EPS data not available.");
            }

            double peRatio = peRatioCalculator.Calculate(marketPrice, earningsPerShare);
            var rowPERatio = new DataGridViewRow();
            rowPERatio.CreateCells(dataGridView_analyze, "P/E Ratio", peRatio);
            dataGridView_analyze.Rows.Add(rowPERatio);

            List<double> trailingEPS = await apiClient.GetTrailingEPSAsync(symbol);
            if (trailingEPS == null || trailingEPS.Count < 4)
            {
                throw new Exception("Not enough EPS data available for trailing P/E.");
            }
            double totalEPS = trailingEPS.Sum();
            double trailingPERatio = marketPrice / totalEPS;
            var rowTrailingPERatio = new DataGridViewRow();
            rowTrailingPERatio.CreateCells(dataGridView_analyze, "Trailing P/E Ratio", trailingPERatio);
            dataGridView_analyze.Rows.Add(rowTrailingPERatio);
        }

        /// <summary>
        /// Utfører P/B-ratio beregning og legger resultatene til i DataGridView.
        /// </summary>
        /// <param name="symbol">Aksjesymbol.</param>
        /// <param name="dataGridView_analyze">DataGridView for å vise analyse resultatene.</param>
        public async Task CalculatePBRatio(string symbol, DataGridView dataGridView_analyze)
        {
            double marketPrice = await apiClient.GetLatestMarketPriceAsync(symbol);
            double bookValuePerShare = await apiClient.GetBookValuePerShareAsync(symbol);

            if (bookValuePerShare == 0)
            {
                throw new Exception("Book value per share data not available.");
            }

            double pbRatio = pbRatioCalculator.Calculate(marketPrice, bookValuePerShare);
            var rowPBRatio = new DataGridViewRow();
            rowPBRatio.CreateCells(dataGridView_analyze, "P/B Ratio", pbRatio);
            dataGridView_analyze.Rows.Add(rowPBRatio);
        }

        /// <summary>
        /// Utfører DCF-beregning og legger resultatene til i DataGridView.
        /// </summary>
        /// <param name="symbol">Aksjesymbol.</param>
        /// <param name="dataGridView_analyze">DataGridView for å vise analyse resultatene.</param>
        /// <param name="discountRate">Diskonteringsrente for DCF-beregningen.</param>
        public async Task CalculateDCF(string symbol, DataGridView dataGridView_analyze, double discountRate)
        {
            List<CashFlowResponse> cashFlows = await apiClient.GetCashFlowsAsync(symbol);
            if (cashFlows == null || cashFlows.Count == 0)
            {
                throw new Exception("No cash flow data available.");
            }

            double dcfValue = dcfCalculator.Calculate(cashFlows.Select(cf => cf.NetCashFlow).ToList(), discountRate);
            var rowDCF = new DataGridViewRow();
            rowDCF.CreateCells(dataGridView_analyze, "DCF Value", dcfValue);
            dataGridView_analyze.Rows.Add(rowDCF);
        }

        /// <summary>
        /// Utfører DDM-beregning og legger resultatene til i DataGridView.
        /// </summary>
        /// <param name="symbol">Aksjesymbol.</param>
        /// <param name="dataGridView_analyze">DataGridView for å vise analyse resultatene.</param>
        /// <param name="growthRate">Vekstrate for DDM-beregningen.</param>
        /// <param name="discountRate">Diskonteringsrente for DDM-beregningen.</param>
        public async Task CalculateDDM(string symbol, DataGridView dataGridView_analyze, double growthRate, double discountRate)
        {
            double lastDividend = await apiClient.GetLastDividendAsync(symbol);
            if (lastDividend == 0)
            {
                throw new Exception("Dividend data not available.");
            }

            double ddmValue = ddmCalculator.Calculate(lastDividend, growthRate, discountRate);
            var row = new DataGridViewRow();
            row.CreateCells(dataGridView_analyze, "Dividend Discount Model Value", ddmValue);
            dataGridView_analyze.Rows.Add(row);
        }

        /// <summary>
        /// Beregner Relative Strength Index (RSI) for en liste av lukkepriser.
        /// </summary>
        /// <param name="closePrices">Liste av lukkepriser.</param>
        /// <param name="period">Perioden for RSI-beregningen (standard er 14).</param>
        /// <returns>Liste av RSI-verdier.</returns>
        public List<double> CalculateRSI(List<double> closePrices, int period = 14)
        {
            List<double> rsiValues = new List<double>();
            if (closePrices.Count < period)
                return rsiValues;

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
            rsiValues.Add(100 - (100 / (1 + rs)));

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
                rsiValues.Add(100 - (100 / (1 + rs)));
            }

            for (int i = 0; i < period; i++)
            {
                rsiValues.Insert(0, 0);
            }

            return rsiValues;
        }

        public List<double> CalculateSMA(List<double> prices, int period)
        {
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
                    smaValues.Add(0); 
                }
            }
            return smaValues;
        }
    }
}
