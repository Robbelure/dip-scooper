using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DipScooper.Models.ApiResponseModels
{
    public class CashFlowResponse
    {
        public double NetCashFlow { get; set; }
        public DateTime Date { get; set; }
    }
}
