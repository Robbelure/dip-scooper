using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DipScooper
{
    public class StockCalculator
    {
        public double CalculatePERatio(double marketPrice, double earningsPerShare)
        {
            return marketPrice / earningsPerShare;
        }

        public double CalculatePBRatio(double marketPrice, double bookValuePerShare)
        {
            return marketPrice / bookValuePerShare;
        }

        public double CalculateDCF(List<double> cashFlows, double discountRate)
        {
            double dcfValue = 0;
            for (int year = 1; year <= cashFlows.Count; year++)
            {
                double discountedCashFlow = cashFlows[year - 1] / Math.Pow(1 + discountRate, year);
                Debug.WriteLine($"Year {year}: Cash Flow = {cashFlows[year - 1]}, Discounted Cash Flow = {discountedCashFlow}");
                dcfValue += discountedCashFlow;
            }
            return dcfValue;
        }

        public double CalculateDDM(double lastDividend, double growthRate, double discountRate)
        {
            return (lastDividend * (1 + growthRate)) / (discountRate - growthRate);
        }
    }
}
