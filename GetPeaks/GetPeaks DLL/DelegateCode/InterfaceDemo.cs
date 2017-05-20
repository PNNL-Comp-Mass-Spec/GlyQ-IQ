using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetPeaks_DLL.DelegateCode
{
    /// <summary>
    /// interface for strategy design pattern
    /// </summary>
    public interface InterfaceDemo
    {
        //define method headder 
        int Calculate(int value1, int value2);
    }

    public interface ICalculateInterface
    {
        //define method headder 
        int Calculate(int value1, int value2);
    }
}
