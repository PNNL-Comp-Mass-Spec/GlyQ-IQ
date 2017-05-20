using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetPeaks_DLL.DelegateCode
{
    
    public static class StrategyUnitTestRunner
    {
        public static void execute()
        {
            CalculateClient minusClient = new CalculateClient(new Minus());
            Console.WriteLine("<br />Minus: " + minusClient.Calculate(7, 1).ToString());

            CalculateClient plusClient = new CalculateClient(new Plussus());

            Console.WriteLine("<br />Plussus: " + plusClient.Calculate(7, 1).ToString());
        }
    }
}
