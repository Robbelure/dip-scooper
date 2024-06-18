using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DipScooper.Models.ApiResponseModels
{
    public class BalanceSheet
    {
        public double Equity { get; set; }
        public double SharesOutstanding { get; set; }

        public BalanceSheet(double equity, double sharesOutstanding)
        {
            Equity = equity;
            SharesOutstanding = sharesOutstanding;
        }
    }
}
