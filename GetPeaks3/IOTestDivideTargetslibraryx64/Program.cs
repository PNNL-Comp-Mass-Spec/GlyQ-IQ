using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DivideTargetsLibraryX64.FromGetPeaks;
using DivideTargetsLibraryX64.Objects;
using Run64.Backend.Core;
using Run64.Backend.Runs;
using Run64.Utilities;

namespace IOTestDivideTargetslibraryx64
{
    class Program
    {
        static void Main(string[] args)
        {
            string filename = @"\\picfs\projects\DMS\PIC_HPC\Hot\DeceptionTests\Testing_if_installed\IOTest\Gly08_Velos4_Jaguar_200nL_D60A_1X_C1_2Sept12_1.raw";
            string outPutLocation = @"\\picfs\projects\DMS\PIC_HPC\Hot\DeceptionTests\DivideTargetLibraryx64Test\DivideTargetLibraryRunx64Works.txt";
            bool useArgs = true;
            if (useArgs)
            {
                if (args.Length != 2)
                {
                    Console.WriteLine("Args are wrong length");
                    Console.ReadKey();
                }
                filename = args[0];
                outPutLocation = args[1];
            }
            Console.WriteLine("The file + path is: " + filename);
            Console.WriteLine("Does It Exist?: " + filename);


            Console.WriteLine("Try creating Run");


            RunFactory rf = new RunFactory();
            Run run = rf.CreateRun(filename);

            Console.WriteLine("RunCreated");

            Check.Ensure(run != null, "RunUtilites could not create run. Run is null.");
            //end copy run

            int numberOfScans = run.GetNumMSScans();
            string lineToWrite = "there should be 3173 scans.  There are <" + numberOfScans + "> scans";
            Console.WriteLine(lineToWrite);
            List<string> lines = new List<string>();
            lines.Add(lineToWrite);


            DivideTargetsLibraryX64.Objects.Whale testObject = new Whale();
            testObject.MyAtom = new Atom(1,2,3,4);
            Console.WriteLine("Object from DivideTargetsLibraryX64 was created");
            lines.Add("Object from DivideTargetsLibraryX64 was created");
            lines.Add(testObject.MyAtom.Composition[0].ToString());

            DivideTargetsLibraryX64.FromGetPeaks.StringListToDisk writer = new DivideTargetsLibraryX64.FromGetPeaks.StringListToDisk();
            writer.toDiskStringList(outPutLocation, lines);
            
            
           

        }
    }
}
