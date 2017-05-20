using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace BatchDelay
{
    class Program
    {
        static void Main(string[] args)
        {
            
            int seconds = Convert.ToInt32(args[0]);

            ValueType time = System.DateTime.Now;
            Console.WriteLine(time);
            
            for (int i = 1; i < seconds +1 ; i++)
            {
                Console.WriteLine("Sleeping... " + i + " out of " + seconds + " seconds");
                Thread.Sleep(1000);
                
            }

            time = System.DateTime.Now;
            Console.WriteLine(time);

            Console.WriteLine("Main thread exits.");

        }
    }
}
