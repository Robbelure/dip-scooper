using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DipScooper.Models.APIModels
{
    public class CashFlowStatement
    {
        public double NetCashFlow { get; set; }

        public CashFlowStatement(double netCashFlow)
        {
            NetCashFlow = netCashFlow;
        }
    }
}
