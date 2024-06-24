using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DipScooper.BLL.Calculators
{
    public class PERatioCalculator
    {
        public double Calculate(double marketPrice, double earningsPerShare)
        {
            return marketPrice / earningsPerShare;
        }
    }
}
