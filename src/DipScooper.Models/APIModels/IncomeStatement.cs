namespace DipScooper.Models.APIModels
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
