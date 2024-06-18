using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DipScooper.Calculators
{
    public class PBRatioCalculator
    {
        public double Calculate(double marketPrice, double bookValuePerShare)
        {
            return marketPrice / bookValuePerShare;
        }
    }
}
