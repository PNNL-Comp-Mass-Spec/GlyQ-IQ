using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using SleepDLL;

namespace Sleep
{
    class Program
    {
        static void Main(string[] args)
        {
            //args = new string[1];
            //args[0] = @"F:\ScottK\ToPIC\PIC_SleepParameterFile.txt";
            
            ParametersSleep parameters = new ParametersSleep();
            parameters.SetParameters(args[0]);
            
            double time = parameters.TimeInt;//seconds
            
            //write launchBatchFile
            //string uniqueTimeString = DateTime.Now.Ticks.ToString();
            //string uniqueTimeString = DateTime.Now.Ticks.ToString();
            Random randomNumberGeneratorSeed = new Random();
            int seed = Convert.ToInt32(randomNumberGeneratorSeed.Next(0, int.MaxValue));
            Random randomNumberGenerator = new Random(seed);

            //Random randomNumberGenerator = new Random(DateTime.Now.Millisecond);

            string uniqueTimeString = randomNumberGenerator.Next(0, int.MaxValue).ToString();
           
            Console.WriteLine(uniqueTimeString);

            string batchName = "SleepLaunch_" + uniqueTimeString +  ".bat";
            string launchPath = parameters.WorkingFolderPath + @"\" + batchName;


            List<string> launchStrings = new List<string>();
            launchStrings.Add("Call " + "\"" +  parameters.BatchFileToRunAfterLoop + "\"");
            StringListToDisk writer = new StringListToDisk();
            writer.toDiskStringList(launchPath, launchStrings);

            Console.WriteLine("Waiting for file: " + parameters.FileToWaitFor + Environment.NewLine);

            mark1://goto spot
            
            if (!File.Exists(parameters.FileToWaitFor))
            {
                
                Thread.Sleep((int) (time*1000));
                Console.WriteLine("Waiting for " + parameters.TimeInt + " seconds at " + DateTime.Now.ToString("HH:mm:ss tt") + Environment.NewLine + args[0]);
                goto mark1;
            }
            else
            {
                Console.WriteLine("Found it!");
                Console.WriteLine("Execute: " + launchPath);
                
                string command = launchPath;

                System.Diagnostics.Process proc = new System.Diagnostics.Process(); // Declare New Process
                proc.StartInfo.FileName = @"C:\Windows\System32\cmd.exe";
                proc.StartInfo.Arguments = "/c " + command;  
               
                proc.StartInfo.RedirectStandardError = true;
                //proc.StartInfo.RedirectStandardOutput = false;//false will print the text to the console
                proc.StartInfo.RedirectStandardOutput = true;//false will print the text to the console
                proc.StartInfo.UseShellExecute = false;

                proc.StartInfo.CreateNoWindow = false; // Do not create the black window.//true
               
                proc.Start();


                string result = proc.StandardOutput.ReadToEnd();
                // Display the command output.
                //Console.WriteLine(result);


                //proc.WaitForExit
                //    (
                //        (timeout <= 0)
                //        ? int.MaxValue : timeout * NO_MILLISECONDS_IN_A_SECOND * NO_SECONDS_IN_A_MINUTE
                //    );

                //errorMessage = proc.StandardError.ReadToEnd();
                
                //this is needed or it will not run
                proc.WaitForExit();

                //outputMessage = proc.StandardOutput.ReadToEnd();
                //proc.WaitForExit();
            }


            //delete temp batch file
            if(File.Exists(launchPath))
            {
                File.Delete(launchPath);
            }
        }
    }
}
