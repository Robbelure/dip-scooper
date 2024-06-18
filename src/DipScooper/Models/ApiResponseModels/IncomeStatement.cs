using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DipScooper.Models.ApiResponseModels
{
    public class IncomeStatement
    {
        public double BasicEarningsPerShare { get; set; }

        public IncomeStatement(double basicEarningsPerShare)
        {
            BasicEarningsPerShare = basicEarningsPerShare;
        }
    }
}
