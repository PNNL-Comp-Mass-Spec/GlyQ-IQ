using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HPC_Connector;
using HPC_Submit;
using NUnit.Framework;

namespace HPCUnitTests
{
    class X64DllTest
    {
        private string s = @"\";

        string fileSystem = @"\\picfs.pnl.gov";
        string rootFolder = @"projects\DMS\PIC_HPC\Hot\DeceptionTests";
        string fileRoot = @"\\picfs.pnl.gov\projects\DMS\PIC_HPC\Hot\DeceptionTests";

        [Test]
        public void ThermoX64DllTest()
        {
            string testFile = fileRoot + s + @"Testing_if_installed\TestThermoDLLx64_D2_Success.txt";
            bool exists = System.IO.File.Exists(testFile);
            if (exists)
            {
                System.IO.File.Delete(testFile);
            }
            
            string nameOfJob = "ThermoTestx64"; string nameTask = "x64_IO";

            string commandLine = 
                fileRoot + s + @"ThermoIOX64\Release\IOTestx64.exe" + " " + 
                fileRoot + s + @"Testing_if_installed\IOTest\Gly08_Velos4_Jaguar_200nL_D60A_1X_C1_2Sept12_1.raw" + " " +
                testFile + " " +
                " >> x64Test_%computername%_%CCP_JOBID%.txt";

            string workingDirectory = fileRoot + s + @"Testing_if_installed";

            JobToHPC sendMe = BasicTask(nameOfJob, nameTask, commandLine, workingDirectory);
            HPC_Submit.WindowsHPC2012 toScheduler = new WindowsHPC2012();
            toScheduler.Send(sendMe);

            Console.WriteLine("Waiting for 5 seconds");
            System.Threading.Thread.Sleep(5000);
            exists = System.IO.File.Exists(testFile);
            Assert.IsTrue(exists);

        }

        [Test]
        public void ThermoX86DllTest()
        {
            string testFile = fileRoot + s + @"Testing_if_installed\TestThermoDLLx86_D2_Success.txt";
            bool exists = System.IO.File.Exists(testFile);
            if (exists)
            {
                System.IO.File.Delete(testFile);
            }

            string nameOfJob = "ThermoTestx86"; string nameTask = "x86_IO";

            string commandLine =
                fileRoot + s + @"ThermoIOX86\Release\IOTest.exe" + " " +
                fileRoot + s + @"Testing_if_installed\IOTest\Gly08_Velos4_Jaguar_200nL_D60A_1X_C1_2Sept12_1.raw" + " " +
                testFile + " " +
                " >> x86Test_%computername%_%CCP_JOBID%.txt";

            string workingDirectory = fileRoot + s + @"Testing_if_installed";



            JobToHPC sendMe = BasicTask(nameOfJob, nameTask, commandLine, workingDirectory);
            HPC_Submit.WindowsHPC2012 toScheduler = new WindowsHPC2012();
            toScheduler.Send(sendMe);

            Console.WriteLine("Waiting for 5 seconds");
            System.Threading.Thread.Sleep(5000);
            exists = System.IO.File.Exists(testFile);
            Assert.IsTrue(exists);

        }


        [Test]
        public void ThermoX64Runtest()
        {
            string testFile = fileRoot + s + @"ThermoRunx64\Runx64Works.txt";
            bool exists = System.IO.File.Exists(testFile);
            if (exists)
            {
                System.IO.File.Delete(testFile);
            }

            string nameOfJob = "ThermoTestRunX64"; string nameTask = "x64Run";

            string commandLine =
                fileRoot + s + @"ThermoRunx64\Release\IORun64Test.exe" + " " +
                fileRoot + s + @"Testing_if_installed\IOTest\Gly08_Velos4_Jaguar_200nL_D60A_1X_C1_2Sept12_1.raw" + " " +
                testFile + " " +
                " >> x64RUNTest_%computername%_%CCP_JOBID%.txt";

            string workingDirectory = fileRoot + s + @"ThermoRunx64";



            JobToHPC sendMe = BasicTask(nameOfJob, nameTask, commandLine, workingDirectory);
            HPC_Submit.WindowsHPC2012 toScheduler = new WindowsHPC2012();
            toScheduler.Send(sendMe);

            Console.WriteLine("Waiting for 10 seconds");
            System.Threading.Thread.Sleep(10000);
            exists = System.IO.File.Exists(testFile);
            Assert.IsTrue(exists);

        }


        [Test]
        public void IQ2Runtest()
        {
            string testFile = fileRoot + s + @"IQ2x64Test\Runx64IQ2Works.txt";
            bool exists = System.IO.File.Exists(testFile);
            if (exists)
            {
                System.IO.File.Delete(testFile);
            }

            string nameOfJob = "ThermoTestIQ2RunX64"; string nameTask = "IQ2x64Run";

            string commandLine =
                fileRoot + s + @"IQ2x64Test\Release\IQ2_UnitTests64.exe" + " " +
                fileRoot + s + @"Testing_if_installed\IOTest\Gly08_Velos4_Jaguar_200nL_D60A_1X_C1_2Sept12_1.raw" + " " +
                testFile + " " +
                " >> IQ2x64RUNTest_%computername%_%CCP_JOBID%.txt";

            string workingDirectory = fileRoot + s + @"IQ2x64Test";



            JobToHPC sendMe = BasicTask(nameOfJob, nameTask, commandLine, workingDirectory);
            HPC_Submit.WindowsHPC2012 toScheduler = new WindowsHPC2012();
            toScheduler.Send(sendMe);

            Console.WriteLine("Waiting for 10 seconds");
            System.Threading.Thread.Sleep(10000);
            exists = System.IO.File.Exists(testFile);
            Assert.IsTrue(exists);

        }


        [Test]
        public void GetPeaksLiteRuntest()
        {
            string testFile = fileRoot + s + @"GetPeaksDLLLiteTest\GetPeaksLitex64Works.txt";
            bool exists = System.IO.File.Exists(testFile);
            if (exists)
            {
                System.IO.File.Delete(testFile);
            }

            string nameOfJob = "ThermoTestGetPeaksDLLIQ2RunX64"; string nameTask = "GetPEaksDLLIQ2x64Run";

            string commandLine =
                fileRoot + s + @"GetPeaksDLLLiteTest\Release\IOTestGetPeaksDLLLiteX64.exe" + " " +
                fileRoot + s + @"Testing_if_installed\IOTest\Gly08_Velos4_Jaguar_200nL_D60A_1X_C1_2Sept12_1.raw" + " " +
                testFile + " " +
                " >> GetPeaksIQ2x64RUNTest_%computername%_%CCP_JOBID%.txt";

            string workingDirectory = fileRoot + s + @"GetPeaksDLLLiteTest";



            JobToHPC sendMe = BasicTask(nameOfJob, nameTask, commandLine, workingDirectory);
            HPC_Submit.WindowsHPC2012 toScheduler = new WindowsHPC2012();
            toScheduler.Send(sendMe);

            Console.WriteLine("Waiting for 10 seconds");
            System.Threading.Thread.Sleep(10000);
            exists = System.IO.File.Exists(testFile);
            Assert.IsTrue(exists);

        }


        [Test]
        public void DivideTargetLibraryx64Test()
        {
            string testFile = fileRoot + s + @"DivideTargetLibraryx64Test\DivideTargetLibraryx64_Works.txt";
            bool exists = System.IO.File.Exists(testFile);
            if (exists)
            {
                System.IO.File.Delete(testFile);
            }

            string nameOfJob = "DivideTargetLibraryx64Test"; string nameTask = "DivideTargetLibraryx64";

            string commandLine =
                fileRoot + s + @"DivideTargetLibraryx64Test\Release\IOTestDivideTargetslibraryx64.exe" + " " +
                fileRoot + s + @"Testing_if_installed\IOTest\Gly08_Velos4_Jaguar_200nL_D60A_1X_C1_2Sept12_1.raw" + " " +
                testFile + " " +
                " >> DivideTargetLibraryx64RUNTest_%computername%_%CCP_JOBID%.txt";

            string workingDirectory = fileRoot + s + @"DivideTargetLibraryx64Test";



            JobToHPC sendMe = BasicTask(nameOfJob, nameTask, commandLine, workingDirectory);
            HPC_Submit.WindowsHPC2012 toScheduler = new WindowsHPC2012();
            toScheduler.Send(sendMe);

            Console.WriteLine("Waiting for 10 seconds");
            System.Threading.Thread.Sleep(10000);
            exists = System.IO.File.Exists(testFile);
            Assert.IsTrue(exists);

        }


        [Test]
        public void GlyIQ3RunX64test()
        {
            string testFile = fileRoot + s + @"GlyQIQx64Test\GlyQIQx64Works.txt";
            bool exists = System.IO.File.Exists(testFile);
            if (exists)
            {
                System.IO.File.Delete(testFile);
            }

            string nameOfJob = "GlyIQ3RunX64"; string nameTask = "GlyQIQ3GetPEaksDLLIQ2x64Run";

            string commandLine =
                fileRoot + s + @"GlyQIQx64Test\Release\IOTestGlyQIQ3x64.exe" + " " +
                fileRoot + s + @"Testing_if_installed\IOTest\Gly08_Velos4_Jaguar_200nL_D60A_1X_C1_2Sept12_1.raw" + " " +
                testFile + " " +
                " >> GlyQIQ3GetPeaksIQ2x64RUNTest_%computername%_%CCP_JOBID%.txt";

            string workingDirectory = fileRoot + s + @"GlyQIQx64Test";



            JobToHPC sendMe = BasicTask(nameOfJob, nameTask, commandLine, workingDirectory);
            HPC_Submit.WindowsHPC2012 toScheduler = new WindowsHPC2012();
            toScheduler.Send(sendMe);

            Console.WriteLine("Waiting for 10 seconds");
            System.Threading.Thread.Sleep(10000);
            exists = System.IO.File.Exists(testFile);
            Assert.IsTrue(exists);

        }


        [Test]
        public void TestingReferences()
        {
            string testFile = fileRoot + s + @"TestingReferencesDLL\DllTest.txt";
            bool exists = System.IO.File.Exists(testFile);
            if (exists)
            {
                System.IO.File.Delete(testFile);
            }

            string nameOfJob = "TestingReferencesDLL"; string nameTask = "TestingReferencesDLLx64";

            string commandLine =
                fileRoot + s + @"TestingReferencesDLL\Release\TessingReferencesDLls.exe" + " " +
                fileRoot + s + @"Testing_if_installed\IOTest\Gly08_Velos4_Jaguar_200nL_D60A_1X_C1_2Sept12_1.raw" + " " +
                testFile + " " +
                " >> DivideTargetLibraryx64RUNTest_%computername%_%CCP_JOBID%.txt";

            string workingDirectory = fileRoot + s + @"TestingReferencesDLL";



            JobToHPC sendMe = BasicTask(nameOfJob, nameTask, commandLine, workingDirectory);
            HPC_Submit.WindowsHPC2012 toScheduler = new WindowsHPC2012();
            toScheduler.Send(sendMe);

            Console.WriteLine("Waiting for 3 seconds");
            System.Threading.Thread.Sleep(3000);
            exists = System.IO.File.Exists(testFile);
            Assert.IsTrue(exists);


        }

        private JobToHPC BasicTask(string nameOfJob, string nameTask, string commandLine, string workingDirectory)
        {
            string clusterName = "deception2.pnnl.gov";
            HPC_Connector.ParametersCluster clusterParams = new ParametersCluster(clusterName);

            
            HPC_Connector.ParametersJob jobParams = new ParametersJob(nameOfJob);
            jobParams.MaxNumberOfCores = 1;
            jobParams.MinNumberOfCores = 1;
            jobParams.PriorityLevel = PriorityLevel.BelowNormal;
            jobParams.ProjectName = "GlyQIQ";
            jobParams.TargetHardwareUnitType = HardwareUnitType.Node;
            jobParams.TemplateName = "PrePost";
            //jobParams.TemplateName = "GlyQIQ";
            jobParams.isExclusive = true;

            HPC_Connector.ParametersTask testTask = new ParametersTask(nameTask);

            testTask.CommandLine = commandLine;
            testTask.TaskTypeOption = HPCTaskType.ParametricSweep;
            testTask.ParametricIncrement = 1;
            testTask.ParametricStartIndex = 1;
            testTask.ParametricStopIndex = 1;
            testTask.WorkDirectory = workingDirectory;
            //testTask.StdOutFilePath = fileRoot + s + @"Testing_if_installed\ThermoX64_%CCP_JOBID%.txt";
            //testTask.StdOutFilePath = fileRoot + s + @"Testing_if_installed\ThermoX64_out.txt";

            JobToHPC sendMe = new JobToHPC(clusterName, nameOfJob, nameTask);
            sendMe.ClusterParameters = clusterParams;
            sendMe.JobParameters = jobParams;
            sendMe.TaskParameters = testTask;
            return sendMe;
        }
    }
}
