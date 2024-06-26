﻿using System.Diagnostics;

namespace DipScooper.BLL.Calculators
{
    public class DCFCalculator
    {
        public double Calculate(List<double> cashFlows, double discountRate)
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
    }
}
