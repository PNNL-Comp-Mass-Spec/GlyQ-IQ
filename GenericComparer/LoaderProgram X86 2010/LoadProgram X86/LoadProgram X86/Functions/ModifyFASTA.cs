using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    public class ModifyFASTA
    {
        public string HeaderToName(string header)
        {
            string proteinName;
            char[] splitParameters = new char[2];
            splitParameters[0] = '|';
            splitParameters[1] = ' ';

            String[] nameArray = header.Split(splitParameters, StringSplitOptions.RemoveEmptyEntries);

            proteinName = nameArray[2];
          

            return proteinName;
        }

        
    }
}
