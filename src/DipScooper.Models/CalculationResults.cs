namespace DipScooper.Models
{
    // the result of a financial calculation (StockService)
    public class CalculationResult
    {
        public string Name { get; set; }
        public double Value { get; set; }

        public CalculationResult(string name, double value)
        {
            Name = name;
            Value = Math.Round(value, 5);
        }
    }
}
