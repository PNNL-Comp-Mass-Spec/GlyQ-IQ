using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetPeaks_DLL.DelegateCode
{
    class WorkerClass
    {
    }

    public class Minus : ICalculateInterface
    {
        public int Calculate(int value1, int value2)
        {
            //define logic  
            return value1 - value2;
        }
    }


    //Strategy 2: Plussus  

    public class Plussus : ICalculateInterface
    {
        public int Calculate(int value1, int value2)
        {
            //define logic  
            return value1 + value2;
        }
    } 
}
