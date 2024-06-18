using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DipScooper.Calculators
{
    public class DDMCalculator
    {
        public double Calculate(double lastDividend, double growthRate, double discountRate)
        {
            return (lastDividend * (1 + growthRate)) / (discountRate - growthRate);
        }
    }
}
