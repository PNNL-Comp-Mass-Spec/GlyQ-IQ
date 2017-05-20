using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Hpc.Scheduler;
using HPC_JobCreator;

namespace CheckHPCStatus
{
    class Program
    {
        static void Main(string[] args)
        {
            string clusterName = "Deception2.pnnl.gov";

           
            // Create a scheduler object to be used to 
            // establish a connection to the scheduler on the headnode
            using (IScheduler scheduler = new Scheduler())
            {
                // Connect to the scheduler
                Console.WriteLine("Connecting to {0}...", clusterName);
                try
                {
                    scheduler.Connect(clusterName);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("Could not connect to the scheduler: {0}", e.Message);
                    return; //abort if no connection could be made
                }

                Utilities.GetClusterAvailibility(scheduler);
                Console.WriteLine("continue...");
                //Console.ReadKey();
            }
        }
    }
}
