using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DipScooper.Models
{
    public class CalculationResult
    {
        public string Name { get; set; }
        public double Value { get; set; }

        public CalculationResult(string name, double value)
        {
            Name = name;
            Value = value;
        }
    }
}
