using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using SleepDLL;

namespace MultiSleep
{
    class Program
    {
        static void Main(string[] args)
        {
            //args = new string[1];
            //args[0] = @"F:\ScottK\ToPIC\PIC_SleepParameterFile.txt";
            //@"F:\ScottK\ToPIC\PIC_MultiSleepParameterFile.txt";
            //args[0] = @"D:\CrazyFileCopy\MultiSleepParameter_1.txt";


            ParametersSleep parameters = new ParametersSleep();
            parameters.SetParameters(args[0]);

            int inputSeed = Convert.ToInt32(args[1]);

            double time = parameters.TimeInt;//seconds

            //write launchBatchFile
            //string uniqueTimeString = DateTime.Now.Ticks.ToString();
            Random randomNumberGeneratorSeed = new Random();
            int seed = Convert.ToInt32(randomNumberGeneratorSeed.Next(0, int.MaxValue)) + inputSeed;
            Random randomNumberGenerator = new Random(seed);

            //Random randomNumberGenerator = new Random(DateTime.Now.Millisecond);

            int randomNumber = randomNumberGenerator.Next(0, int.MaxValue);

            string uniqueTimeString = randomNumber.ToString();
            Console.WriteLine(uniqueTimeString);

            string batchName = "MultiSleepLaunch_" + uniqueTimeString + ".bat";
            string launchPath = parameters.WorkingFolderPath + @"\" + batchName;


            List<string> launchStrings = new List<string>();
            launchStrings.Add("Call " + "\"" + parameters.BatchFileToRunAfterLoop + "\"");
            StringListToDisk writer = new StringListToDisk();
            writer.toDiskStringList(launchPath, launchStrings);

            Console.WriteLine("Waiting for file: " + parameters.FileToWaitFor + Environment.NewLine);


            StringLoadTextFileLine reader = new StringLoadTextFileLine();
            List<string> filesToWaitForLines = reader.SingleFileByLine(parameters.FileToWaitFor);
            //filesToWaitFor.RemoveAt(0);//remove first file

            //remove .txt from files and add _done
            List<string> filesToWaitFor = new List<string>();
            for (int i = 0; i < filesToWaitForLines.Count; i++)
            {
                //string file = filesToWaitFor[i];
                //char[] ending = ".txt".ToArray();
                //string newEnding = "_Done.txt";
                //filesToWaitFor[i] = parameters.WorkingFolderPath + @"\" + file.TrimEnd(ending) + newEnding;
                filesToWaitFor.Add(parameters.WorkingFolderPath + @"\" + filesToWaitForLines[i]);
                
            }

            int counterForCorrect = filesToWaitFor.Count;

        mark1://goto spot

            //add multi check here and return a bool
            //!File.Exists(parameters.FileToWaitFor)
            
            int counter = 0;
            foreach (var fileName in filesToWaitFor)
            {
                Console.WriteLine("Looking for: " + fileName);

               
                

                if(File.Exists(fileName))
                {
                    //FileInfo fileDetails = new FileInfo(fileName);
                    //Console.WriteLine(fileDetails.Length);
                    //
                    //if (fileDetails.Length > 0)//file needs to have size
                    //{
                        counter++;
                    //}
                }
            }

            Console.WriteLine("We found " + counter + " out of " + counterForCorrect);

            bool areAllFilesPresent = (counterForCorrect == counter);

        if (!areAllFilesPresent)
            {

                Thread.Sleep((int)(time * 1000));
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
            
      //next to add.  this is suposed to close when it finishes running          WaitlTillComplete(proc);


                //proc.HasExited
                //outputMessage = proc.StandardOutput.ReadToEnd();
                //proc.WaitForExit();
            }


            //delete temp batch file
            if (File.Exists(launchPath))
            {
                File.Delete(launchPath);
            }
        }

        private static void WaitlTillComplete(Process proc)
        {
            TimeSpan retryFor = new TimeSpan();
            retryFor = System.TimeSpan.FromSeconds(1); //EVERY SECOND
            DateTime startTime = DateTime.Now;
            while (DateTime.Now - startTime <= retryFor)
            {
                Thread.Sleep(100); // wait a little
                if (proc.HasExited)//the idea here is that once the proc has exited, we can exit  //if (!myProcess.HasExited) = if running

                {
                    try
                    {
                        // Close process by sending a close message to its main window.
                        proc.CloseMainWindow();
                        // Free resources associated with process.
                        proc.Close();

                        break;
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                    }
                }
            }
        }

        //possibility to fix the delete problem
        //http://social.msdn.microsoft.com/Forums/vstudio/en-US/5e3e8f7c-3ff8-40f0-b797-8c4bc25253dd/processwaitforexit-doesnt-actually-wait-for-the-process-to-completely-exit
        //http://multipleinheritance.wordpress.com/2012/07/31/process-waitforexit-and-exited-event-arent-working/
        public static bool RetryDeleteFile(string filename, TimeSpan retryFor)
        {
            DateTime startTime = DateTime.Now;
            while (DateTime.Now - startTime <= retryFor)
            {
                Thread.Sleep(100); // wait a little
                try
                {
                    File.Delete(filename);
                    return true;
                }
                catch (UnauthorizedAccessException ex)
                {
                }
            }
            return false;
        }


    }
}
