using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GetPeaks_DLL;

namespace GetPeaks_DLL.ConosleUtilities
{
    public class ParseStrings
    {
        /// <summary>
        /// read in a parameter file name from the batch file.  seperate arguments by a space
        /// </summary>
        /// <param name="args"></param>
        public List<string> Parse3Args(string[] args)
        {
            List<string> paramatersStrings = new List<string>();
            paramatersStrings.Add("");
            paramatersStrings.Add("");
            paramatersStrings.Add("");

            string[] words = { };
            string argument1 = args[0];
            Console.WriteLine(args[1]);
            words = argument1.Split(Environment.NewLine.ToCharArray(),  //'\n', '\r'
                StringSplitOptions.RemoveEmptyEntries);

            int countArguments = 0;
            foreach (string argument in args)
            {
                //Console.WriteLine(argument);
                countArguments++;
            }
            if (countArguments == 3)
            {
                paramatersStrings[0] = args[0];
                paramatersStrings[1] = args[1];
                paramatersStrings[2] = args[2];
            }
            else
            {
                Console.WriteLine("MissingArguments.  There are: ", countArguments);
                Console.ReadKey();
            }
            //string fileIn = args[countArguments - 1];

            Console.WriteLine("There are " + countArguments + " arguments");
            Console.WriteLine("Our file is " + paramatersStrings[0]);

            //FileInfo serverfi = new FileInfo(paramatersStrings[0]);
            //for DB analysis
            FileInfo serverfi = new FileInfo(paramatersStrings[0] + "\\" + paramatersStrings[1] + paramatersStrings[2]);
            bool serverFileExists = serverfi.Exists;

            Console.WriteLine("Does the parameter file exists? " + serverFileExists);
            //Console.ReadKey();

            return paramatersStrings;
        }

        public List<string> Parse4Args(string[] args)
        {
            List<string> paramatersStrings = new List<string>();
            paramatersStrings.Add("");
            paramatersStrings.Add("");
            paramatersStrings.Add("");
            paramatersStrings.Add("");

            string[] words = { };
            string argument1 = args[0];
            Console.WriteLine(args[1]);
            words = argument1.Split(Environment.NewLine.ToCharArray(),  //'\n', '\r'
                StringSplitOptions.RemoveEmptyEntries);

            int countArguments = 0;
            foreach (string argument in args)
            {
                //Console.WriteLine(argument);
                countArguments++;
            }
            if (countArguments == 4)
            {
                paramatersStrings[0] = args[0];
                paramatersStrings[1] = args[1];
                paramatersStrings[2] = args[2];
                paramatersStrings[3] = args[3];
            }
            else
            {
                Console.WriteLine("MissingArguments.  There are: ", countArguments);
                Console.ReadKey();
            }
            //string fileIn = args[countArguments - 1];

            Console.WriteLine("Ther are " + countArguments + " arguments");
            Console.WriteLine("Our file is " + paramatersStrings[0]);

            FileInfo serverfi = new FileInfo(paramatersStrings[0]);
            bool serverFileExists = serverfi.Exists;

            Console.WriteLine("Does the parameter file exists? " + serverFileExists);
            //Console.ReadKey();

            return paramatersStrings;
        }

        public List<string> Parse7Args(string[] args)
        {
            List<string> paramatersStrings = new List<string>();
            for (int i = 0; i < 7; i++)
            {
                paramatersStrings.Add("");
            }
            
            string[] words = { };
            string argument1 = args[0];
            Console.WriteLine(args[1]);
            words = argument1.Split(Environment.NewLine.ToCharArray(),  //'\n', '\r'
                StringSplitOptions.RemoveEmptyEntries);

            int countArguments = 0;
            foreach (string argument in args)
            {
                //Console.WriteLine(argument);
                countArguments++;
            }
            if (countArguments == 7)
            {
                for (int i = 0; i < 7; i++)
                {
                    paramatersStrings[i] = args[i];
                }
            }
            else
            {
                Console.WriteLine("MissingArguments.  There are: ", countArguments);
                Console.ReadKey();
            }
            //string fileIn = args[countArguments - 1];

            Console.WriteLine("Ther are " + countArguments + " arguments");
            Console.WriteLine("Our file is " + paramatersStrings[0]);

            FileInfo serverfi = new FileInfo(paramatersStrings[0]);
            bool serverFileExists = serverfi.Exists;

            Console.WriteLine("Does the parameter file exists? " + serverFileExists);
            //Console.ReadKey();

            return paramatersStrings;
        }
    }
}
