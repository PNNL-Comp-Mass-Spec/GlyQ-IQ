using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SleepDLL;

namespace WriteCheckFile
{
    class Program
    {
        static void Main(string[] args)
        {
            //args = new string[1];
            //args[0] = @"D:\CrazyFileCopy\myCheckFile.txt";

            StringListToDisk writer = new StringListToDisk();
            List<string>  lines = new List<string>();
            writer.toDiskStringList(args[0],lines);
        }
    }
}
