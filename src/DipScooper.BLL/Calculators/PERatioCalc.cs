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
