using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OmniFinder.Functions
{
    public class FactorialDesignFactor<T>
    {
        public List<T> FactorSet { get; set; }

        public FactorialDesignFactor()
        {
            FactorSet = new List<T>();
        }
    }
}
