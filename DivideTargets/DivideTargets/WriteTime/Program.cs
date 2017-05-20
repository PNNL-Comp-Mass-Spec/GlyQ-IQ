using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SleepDLL;

namespace WriteTime
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileOfInterest = args[0];

            StringLoadTextFileLine reader = new StringLoadTextFileLine();
            StringListToDisk wrtier = new StringListToDisk();

            List<string> lines = new List<string>();
            if(File.Exists(fileOfInterest))
            {
                
                List<string> existingLines = reader.SingleFileByLine(fileOfInterest);
                lines.AddRange(existingLines);

                lines.Add("Stop " + DateTime.Now.ToString(@"MM\/dd\/yyyy h\:mm tt"));
                
            }
            else
            {
                lines.Add("Start " + DateTime.Now.ToString(@"MM\/dd\/yyyy h\:mm tt"));
                
            }
            
            wrtier.toDiskStringList(fileOfInterest,lines);
            Console.WriteLine("Time Written");
            
        }
    }
}
