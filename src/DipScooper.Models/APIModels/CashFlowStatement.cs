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
