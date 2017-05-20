using System;
using System.IO;
using Microsoft.Hpc.Scheduler;
using Microsoft.Hpc.Scheduler.Properties;

namespace HPC_JobCreator
{
    public static class Utilities
    {
        public static void RunCMD(string workingFolderPath, string batchFileToRun)
        {
            ////write launchBatchFile
            //string batchName = "SleepLaunch.bat";
            string launchPath = workingFolderPath + @"\" + batchFileToRun;

            ////make a temp batch file to run the real batch file
            //List<string> launchStrings = new List<string>();
            //launchStrings.Add("Call " + "\"" + batchFileToRun + "\"");
            //StringListToDisk writer = new StringListToDisk();
            //writer.toDiskStringList(launchPath, launchStrings);

            //Console.WriteLine("Done writing SleepLaunch");
            ////Console.ReadKey();

            string command = launchPath;


            Console.WriteLine("launchPath is " + launchPath);
            //Console.ReadKey();

            System.Diagnostics.Process proc = new System.Diagnostics.Process(); // Declare New Process
            proc.StartInfo.FileName = @"C:\Windows\System32\cmd.exe";
            proc.StartInfo.Arguments = "/c " + command;

            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.RedirectStandardOutput = false;//false will print the text to the console
            proc.StartInfo.UseShellExecute = false;

            proc.StartInfo.CreateNoWindow = false; // Do not create the black window.//true

            proc.Start();

            //this is needed or it will not run
            proc.WaitForExit();

            Console.WriteLine("Done withProcess");
            //Console.ReadKey();

            //delete temp batch file
            if (File.Exists(launchPath))
            {
                File.Delete(launchPath);
            }
        }


        public static void GetClusterAvailibility(IScheduler scheduler)
        {
            IIntCollection nodID = scheduler.GetNodeIdList(null, null);
            double onlineNodeCount = 0;
            double offlineNodeCount = 0;
            double reachableNodeCount = 0;
            double physicalCoreCount = 0;
            double availibleCoreCount = 0;
            double physicalSocketCount = 0;
            foreach (int nodeNumber in nodID)
            {
                ISchedulerNode openNode = scheduler.OpenNode(nodeNumber);

                string nodeName = openNode.Name;
                bool deceptionCheck = false;
                deceptionCheck = nodeName.Contains("DECEPTION-");
                Console.Write("Node " + nodeName + " is " + openNode.State);
                if (openNode.State == NodeState.Online && deceptionCheck)
                {
                    onlineNodeCount++;
                    Console.Write(" ");
                    Console.WriteLine("with " + openNode.NumberOfSockets + " sockets");
                    Console.WriteLine("with " + openNode.NumberOfSockets + " sockets");
                }
                if (openNode.State == NodeState.Offline && deceptionCheck)
                {
                    offlineNodeCount++;
                }
                if (openNode.Reachable && deceptionCheck)
                {
                    reachableNodeCount++;
                    physicalCoreCount += openNode.NumberOfCores;
                    physicalSocketCount += openNode.NumberOfSockets;
                    ISchedulerCollection returnedCores = openNode.GetCores();
                    int maxCores = returnedCores.Count;
                    int busyCores = 0;
                    foreach (ISchedulerCore whoKnows in returnedCores)
                    {
                        //Console.WriteLine("scheculder Core " + whoKnows.Id + " is " + whoKnows.State); 
                        if (whoKnows.State == SchedulerCoreState.Busy)
                        {
                            busyCores++;
                        }
                    }
                    availibleCoreCount += (maxCores - busyCores);
                    Console.WriteLine(" with " + busyCores + "/" + maxCores + " used");
                }
                else
                {
                    Console.WriteLine("");
                }
            }

            Console.WriteLine("We have " + onlineNodeCount + " Deception nodes online and " + offlineNodeCount + " offline");
            Console.WriteLine(reachableNodeCount + " are reachable allowing for " + physicalSocketCount + " sockets and " +
                              physicalCoreCount + " cores");
            Console.WriteLine(availibleCoreCount + " cores are not in use");
        }


    }
}
