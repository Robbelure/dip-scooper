using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DipScooper.Calculators;
using DipScooper.Models.ApiResponseModels;

namespace DipScooper.Services
{
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

        public async Task CalculatePERatio(string symbol, DataTable dataTable)
        {
            double marketPrice = await apiClient.GetLatestMarketPriceAsync(symbol);
            double earningsPerShare = await apiClient.GetEarningsPerShareAsync(symbol);

            if (earningsPerShare == 0)
            {
                throw new Exception("EPS data not available.");
            }

            double peRatio = peRatioCalculator.Calculate(marketPrice, earningsPerShare);
            DataRow rowPERatio = dataTable.NewRow();
            rowPERatio["Calculation"] = "P/E Ratio";
            rowPERatio["Result"] = peRatio;
            dataTable.Rows.Add(rowPERatio);

            List<double> trailingEPS = await apiClient.GetTrailingEPSAsync(symbol);
            if (trailingEPS == null || trailingEPS.Count < 4)
            {
                throw new Exception("Not enough EPS data available for trailing P/E.");
            }
            double totalEPS = trailingEPS.Sum();
            double trailingPERatio = marketPrice / totalEPS;
            DataRow rowTrailingPERatio = dataTable.NewRow();
            rowTrailingPERatio["Calculation"] = "Trailing P/E Ratio";
            rowTrailingPERatio["Result"] = trailingPERatio;
            dataTable.Rows.Add(rowTrailingPERatio);
        }

        public async Task CalculatePBRatio(string symbol, DataTable dataTable)
        {
            double marketPrice = await apiClient.GetLatestMarketPriceAsync(symbol);
            double bookValuePerShare = await apiClient.GetBookValuePerShareAsync(symbol);

            if (bookValuePerShare == 0)
            {
                throw new Exception("Book value per share data not available.");
            }

            double pbRatio = pbRatioCalculator.Calculate(marketPrice, bookValuePerShare);
            DataRow rowPBRatio = dataTable.NewRow();
            rowPBRatio["Calculation"] = "P/B Ratio";
            rowPBRatio["Result"] = pbRatio;
            dataTable.Rows.Add(rowPBRatio);
        }

        public async Task CalculateDCF(string symbol, DataTable dataTable, double discountRate)
        {
            List<CashFlowResponse> cashFlows = await apiClient.GetCashFlowsAsync(symbol);
            if (cashFlows == null || cashFlows.Count == 0)
            {
                throw new Exception("No cash flow data available.");
            }

            List<double> cashFlowValues = cashFlows.Select(cf => cf.NetCashFlow).ToList();
            double dcfValue = dcfCalculator.Calculate(cashFlowValues, discountRate);
            DataRow rowDCF = dataTable.NewRow();
            rowDCF["Calculation"] = "DCF Value";
            rowDCF["Result"] = dcfValue;
            dataTable.Rows.Add(rowDCF);
        }

        public async Task CalculateDDM(string symbol, DataTable dataTable, double growthRate, double discountRate)
        {
            double lastDividend = await apiClient.GetLastDividendAsync(symbol);
            if (lastDividend == 0)
            {
                throw new Exception("Dividend data not available.");
            }

            double ddmValue = ddmCalculator.Calculate(lastDividend, growthRate, discountRate);
            DataRow row = dataTable.NewRow();
            row["Calculation"] = "Dividend Discount Model Value";
            row["Result"] = ddmValue;
            dataTable.Rows.Add(row);
        }
    }
}
