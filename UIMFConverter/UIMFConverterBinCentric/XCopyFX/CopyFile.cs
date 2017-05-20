using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace XCopyFX
{
    public static class CopyFile
    {
        public static void ProcessXcopyFolder(string SolutionDirectory, string TargetDirectory)
        {
            // Use ProcessStartInfo class 
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;

            //Give the name as Xcopy 
            startInfo.FileName = "xcopy";

            //make the window Hidden 
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;

            //Send the Source and destination as Arguments to the process 
            startInfo.Arguments = SolutionDirectory + " " + TargetDirectory + @" /e /y /I";

            try
            {
                // Start the process with the info we specified. 
                // Call WaitForExit and then the using statement will close. 
                using (Process exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }
            }

            catch (Exception exp)
            {
                throw exp;
            }
        }

        public static void ProcessXcopyFile(string fileNameTarget, string fileNameDestination)
        {
            
            //// Use ProcessStartInfo class 
            //ProcessStartInfo startInfo = new ProcessStartInfo();
            //startInfo.CreateNoWindow = false;
            //startInfo.UseShellExecute = false;

            //////Give the name as Xcopy 
            //startInfo.FileName = "cmd.exe";



            //////make the window Hidden 
            //startInfo.WindowStyle = ProcessWindowStyle.Hidden;

            //////Send the Source and destination as Arguments to the process 
            ////startInfo.Arguments = "/C copy /b \"" + SolutionDirectory + "\" " + TargetDirectory;
            //startInfo.Arguments = "/C copy /b \"" + fileNameTarget + "\" " + fileNameDestination;
            //try
            //{
            //    // Start the process with the info we specified. 
            //    // Call WaitForExit and then the using statement will close. 
            //    using (Process exeProcess = Process.Start(startInfo))
            //    {
            //        exeProcess.WaitForExit();
            //    }
            //}

            //catch (Exception exp)
            //{
            //    throw exp;
            //}

            System.IO.File.Copy(fileNameTarget, fileNameDestination, true);

        }
        
    }
}
