namespace DipScooper.BLL.Calculators
{
    public class PBRatioCalculator
    {
        public double Calculate(double marketPrice, double bookValuePerShare)
        {
            return marketPrice / bookValuePerShare;
        }
    }
}
