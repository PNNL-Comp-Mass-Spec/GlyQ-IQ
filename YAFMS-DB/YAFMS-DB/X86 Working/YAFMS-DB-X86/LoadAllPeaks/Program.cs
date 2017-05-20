using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YAFMS_DB;

namespace LoadAllPeaks
{
    class Program
    {
        static void Main(string[] args)
        {
            UnitTests tester = new UnitTests();

            int engines = Convert.ToInt32(args[0]);
            int threads = Convert.ToInt32(args[1]);
            bool multihread = Convert.ToBoolean(args[2]);
            
            tester.ParalellReadAllSimplePeaksToMemory(engines, threads, multihread);
            Console.ReadKey();
        }
    }
}
