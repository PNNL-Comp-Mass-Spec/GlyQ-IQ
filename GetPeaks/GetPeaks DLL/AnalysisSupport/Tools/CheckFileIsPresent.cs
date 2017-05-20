using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GetPeaks_DLL.AnalysisSupport.Tools
{
    public static class CheckFileIsPresent
    {
        public static void CheckFile(bool readKeyToggle, string name, string PrintType)
        {
            FileInfo fiSQL = new FileInfo(name);
            bool existsSQL = fiSQL.Exists;

            Console.WriteLine(PrintType + ": " + name + " Correct?");
            Console.WriteLine("Is Present: " + existsSQL + Environment.NewLine);

            if (readKeyToggle)
            {
                Console.ReadKey();
            }
        }
    }
}
