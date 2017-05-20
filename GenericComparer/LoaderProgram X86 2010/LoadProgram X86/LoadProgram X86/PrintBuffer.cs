using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1

    
{
    public class PrintBuffer
    {

        List<String> m_printBuffer = new List<String>();  //for printing faster
       
        public PrintBuffer()
        {
            
        }

        public void AddLine(string line)
        {
            m_printBuffer.Add(line);
        }

        public void ToScreen()
        {
            int length = m_printBuffer.Count;
            Console.WriteLine("Printing Buffer...\n");
            for(int i=0;i<length;i++)
            {
                Console.WriteLine(m_printBuffer[i]);
            }
        }

    }
}
