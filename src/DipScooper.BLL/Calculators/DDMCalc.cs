namespace DipScooper.BLL.Calculators
{
    public class DDMCalculator
    {
        public double Calculate(double lastDividend, double growthRate, double discountRate)
        {
            return (lastDividend * (1 + growthRate)) / (discountRate - growthRate);
        }
    }
}
