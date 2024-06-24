namespace DipScooper.Models.APIModels
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
