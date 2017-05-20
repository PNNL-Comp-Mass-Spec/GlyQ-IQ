using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using DeconTools.Backend.Core;
using DeconTools.Workflows.Backend.Core;
using GetPeaks_DLL.DataFIFO;
using IQGlyQ.Enumerations;
using IQGlyQ.FIFO;
using IQGlyQ.Functions;
using NUnit.Framework;
using PNNLOmics.Data.Constants;
using PNNLOmics.Data.Constants.Libraries;

namespace IQGlyQ.UnitTesting
{
    public class IQ_FullDataset_Test
    {
        [Test]
        public void ExecuteMultipleTargetsTest1()
        {
            bool isUnitTest = false;//true for module testing only
            EnumerationIsPic isPic;
            EnumerationDataset thisDataset;

            //thisDataset = EnumerationDataset.SPINExactive;
            thisDataset = EnumerationDataset.Diabetes;
            //thisDataset = EnumerationDataset.SPINExactiveMuddiman;
            //thisDataset = EnumerationDataset.IMS;

            isPic = EnumerationIsPic.IsNotPic;
            //isPic = EnumerationIsPic.IsPic;

            double deltaMassCalibrationMZ = -0.00289 / 2;
            double deltaMassCalibrationMono = 0;
            bool toMassCalibrate = false;

            Console.WriteLine(Environment.NewLine + "Speed Testing test program");

            DateTime starttime;
            Stopwatch stopWatch = StartClock(out starttime);

            //1.  set strings
            string testFile;
            string peaksTestFile;
            string resultsFolder;
            string targetsFile;
            string executorParameterFile;
            string factorsFile;
            string loggingFolder;
            string pathFragmentParameters;
            string filefolderPath;
            IQ_UnitTest_SetStrings.Set(out testFile, out peaksTestFile, out resultsFolder, out targetsFile, out executorParameterFile, out factorsFile, out loggingFolder, out pathFragmentParameters, out filefolderPath, thisDataset, isPic);


            string XYDataFolder = "XYDataWriter";
            string dataSetType = "raw";

            bool deleteFiles = false;
            bool singlecharge = false;
            string expectedResultsFilename;
            string expectedResultsFilenameSummary;
            //RunCode(loggingFolder, isUnitTest, executorParameterFile, factorsFile, thisDataset, deleteFiles, isPic, targetsFile, resultsFolder, resultsFolder, peaksTestFile, testFile, XYDataFolder, dataSetType, pathFragmentParameters, filefolderPath, deltaMassCalibrationMZ, deltaMassCalibrationMono, toMassCalibrate, out expectedResultsFilename, out expectedResultsFilenameSummary);

            //RunCode(loggingFolder, isUnitTest, executorParameterFile, factorsFile, deleteFiles, isPic, targetsFile, resultsFolder, resultsFolder, peaksTestFile, testFile, XYDataFolder, dataSetType, pathFragmentParameters, filefolderPath, deltaMassCalibrationMZ, deltaMassCalibrationMono, toMassCalibrate, out expectedResultsFilename, out expectedResultsFilenameSummary);
            RunCode(loggingFolder, isUnitTest, executorParameterFile, factorsFile, deleteFiles, isPic, targetsFile, resultsFolder, resultsFolder, peaksTestFile, testFile, XYDataFolder, dataSetType, pathFragmentParameters, filefolderPath, out expectedResultsFilename, out expectedResultsFilenameSummary, singlecharge);

            StopClock(starttime, stopWatch);

            Console.WriteLine(Environment.NewLine + "Speed Testing test program End");
        }

        public void ExecuteMultipleTargetsTest2(string testFile, string peaksTestFile, string resultsFolder, string targetsFile, string executorParameterFile, string factorsFile, string loggingFolder, string XYDataFolder, string dataSetType, string pathFragmentParameters, string filefolderPath, string lockLocation, string coreID, bool singlecharge)
        {
            bool isUnitTest = false;//true for module testing only
            EnumerationIsPic isPic;
            //EnumerationDataset thisDataset;

            //thisDataset = EnumerationDataset.SPINExactive;
            //thisDataset = EnumerationDataset.Diabetes;
            //thisDataset = EnumerationDataset.SPINExactiveMuddiman;
            //thisDataset = EnumerationDataset.IMS;

            isPic = EnumerationIsPic.IsNotPic;
            //isPic = EnumerationIsPic.IsPic;

            //double deltaMassCalibrationMZ = -0.00289 / 2;
            //double deltaMassCalibrationMZ = -.001;//1ppm
            ////double deltaMassCalibrationMono = 1.00235;//1Dalton

            //double elementH1Mass = Constants.Elements[ElementName.Hydrogen].IsotopeDictionary["H1"].Mass;
            //double elementH2Mass = Constants.Elements[ElementName.Hydrogen].IsotopeDictionary["H2"].Mass;
            //deltaMassCalibrationMono = elementH2Mass - elementH1Mass;

            //bool toMassCalibrate = true;

            Console.WriteLine(Environment.NewLine + "Speed Testing test program");

            DateTime starttime;
            Stopwatch stopWatch = StartClock(out starttime);

            bool deleteFiles = false;

            //setup results folder summary.  this is the global repositroy
            char[] ending = coreID.ToCharArray();
            string resultsFolderSummary = resultsFolder.TrimEnd(ending);
            resultsFolderSummary = resultsFolderSummary.TrimEnd('_');

            string XYDataWriterFolder = resultsFolder + @"\XYDataWriter";
            Directory.CreateDirectory(XYDataWriterFolder);

            string expectedResultsFilename;
            string expectedResultsFilenameSummary;
            Console.WriteLine("RunCode...");
            //RunCode(loggingFolder, isUnitTest, executorParameterFile, factorsFile, thisDataset, deleteFiles, isPic, targetsFile, resultsFolder, resultsFolderSummary, peaksTestFile, testFile, XYDataFolder, dataSetType, pathFragmentParameters, filefolderPath, deltaMassCalibrationMZ, deltaMassCalibrationMono, toMassCalibrate, out expectedResultsFilename, out expectedResultsFilenameSummary);
            RunCode(loggingFolder, isUnitTest, executorParameterFile, factorsFile, deleteFiles, isPic, targetsFile, resultsFolder, resultsFolderSummary, peaksTestFile, testFile, XYDataFolder, dataSetType, pathFragmentParameters, filefolderPath, out expectedResultsFilename, out expectedResultsFilenameSummary, singlecharge);

            Console.WriteLine("start post run...");

            PostRun(deleteFiles, resultsFolder, expectedResultsFilename, expectedResultsFilenameSummary, lockLocation, coreID);

            StopClock(starttime, stopWatch);

            Console.WriteLine(Environment.NewLine + "Speed Testing test program End");
        }

        //private static void RunCode(string loggingFolder, bool isUnitTest, string executorParameterFile, string factorsFile, EnumerationDataset thisDataSet, bool deleteFiles, EnumerationIsPic isPic, string targetsFile, string resultsFolder, string resultsFolderSummary, string peaksTestFile, string testFile, string XYDataFolder, string dataSetType, string pathFragmentParameters, string filefolderPath, double deltaMassCalibrationMZ, double deltaMassCalibrationMono, bool toMassCalibrate,  out string expectedResultsFilename, out string expectedResultsFilenameSummary)
        private static void RunCode(string loggingFolder, bool isUnitTest, string executorParameterFile, string factorsFile, bool deleteFiles, EnumerationIsPic isPic, string targetsFile, string resultsFolder, string resultsFolderSummary, string peaksTestFile, string testFile, string XYDataFolder, string dataSetType, string pathFragmentParameters, string filefolderPath, out string expectedResultsFilename, out string expectedResultsFilenameSummary, bool singlecharge)
        {
            //Step 1.  Setup Parameters
            FragmentedTargetedWorkflowParametersIQ fragmentedTargetedWorkflowParameters;
            IqExecutor executor;
            Run run;
            //targetsFile = IQGlyQTestingUtilities.SetExecutorAndFragmentsAndParameters(testFile, peaksTestFile, resultsFolder, resultsFolderSummary, targetsFile, executorParameterFile, factorsFile, loggingFolder, XYDataFolder, pathFragmentParameters, filefolderPath, out fragmentedTargetedWorkflowParameters, out expectedResultsFilename, out expectedResultsFilenameSummary, out executor, out run, isUnitTest, thisDataset, isPic, deltaMassCalibrationMZ, deltaMassCalibrationMono, toMassCalibrate);
            targetsFile = IQGlyQTestingUtilities.SetExecutorAndFragmentsAndParameters(testFile, peaksTestFile, resultsFolder, resultsFolderSummary, targetsFile, executorParameterFile, factorsFile, loggingFolder, XYDataFolder, pathFragmentParameters, filefolderPath, out fragmentedTargetedWorkflowParameters, out expectedResultsFilename, out expectedResultsFilenameSummary, out executor, out run, isUnitTest, isPic);

            //Step 2 Calibrate Targets

            if (fragmentedTargetedWorkflowParameters.MSParameters.IsoParameters.ToMassCalibrate)
            {
                double deltaMassCalibrationMZ = fragmentedTargetedWorkflowParameters.MSParameters.IsoParameters.DeltaMassCalibrationMZ;
                double deltaMassCalibrationMono = fragmentedTargetedWorkflowParameters.MSParameters.IsoParameters.DeltaMassCalibrationMono;
                executor.Targets = CalibrateMS.CalibrateTargets(executor.Targets, deltaMassCalibrationMZ, deltaMassCalibrationMono);
            }

            //3.  set workflows
            IQGlyQTestingUtilities.SetWorkflows(fragmentedTargetedWorkflowParameters, executor, run);

            //if you only want to use one charge state.  mainly for debugging
            //bool singlecharge = false; //1 of 3
            List<IqTarget> existingTargets;
            if (singlecharge)
            {
                existingTargets = executor.Targets[0].ChildTargets().ToList();
                
                
                executor.Targets[0].RemoveTarget(existingTargets[0]);
                //executor.Targets[0].RemoveTarget(existingTargets[1]);//removes +1 and starts with +2

                //kill all higher charge states first
                int startindex = 2;
                for (int i = startindex; i < existingTargets.Count; i++)
                {
                    executor.Targets[0].RemoveTarget(existingTargets[i]);
                }


                //charges before
                executor.Targets[0].RemoveTarget(existingTargets[0]);
                //executor.Targets[0].RemoveTarget(existingTargets[1]);
                
                //executor.Targets[0].RemoveTarget(existingTargets[3]);
                //executor.Targets[0].RemoveTarget(existingTargets[4]);
                //executor.Targets[0].RemoveTarget(existingTargets[5]);
                //executor.Targets[0].RemoveTarget(existingTargets[6]);
            }

            Console.WriteLine("Ready to Execute... on " + executor.Targets[0].GetChildCount() + " charge states");
            //Console.ReadKey();
            //Main line for executing IQ:
            executor.Execute();

            //Test the results...

            
        }

        

        private static void PostRun(bool deleteFiles, string resultsFolder, string expectedResultsFilename, string expectedResultsFilenameSummary, string lockLocation, string coreID)
        {
            //step 1 make sure datafile exists
            Assert.IsTrue(File.Exists(expectedResultsFilename), "results file doesn't exist");
            int numResultsInResultsFile = 0;
            bool outputToConsole = true;

            using (StreamReader reader = new StreamReader(expectedResultsFilename))
            {
                while (reader.Peek() != -1)
                {
                    string line = reader.ReadLine();
                    numResultsInResultsFile++;

                    if (outputToConsole)
                    {
                        Console.WriteLine(line);
                    }
                }
            }

            //step 2 copy charge state from fragment charge to charge
            IQGlyQ.FIFO.ImportGlyQResult importer = new ImportGlyQResult();
            List<GlyQIqResult> writtenResults = importer.Import(expectedResultsFilename);
            List<string> reWriteResults = new List<string>();
            bool firstLine = true;
            foreach (GlyQIqResult writtenResult in writtenResults)
            {
                if (firstLine)
                {
                    writtenResult.ChargeState = writtenResult.ChargeState;
                    firstLine = false;
                }
                else
                {
                    writtenResult.ChargeState = writtenResult.FragmentCharge;
                }
                reWriteResults.Add(writtenResult.GlyQIqResultToString("\t"));
            }
            StringListToDisk writer = new StringListToDisk();
            writer.toDiskStringList(expectedResultsFilename, reWriteResults);


            //step 3. make data file specific results folder so we don't overwrite the file withanother process
            List<char> endingFolder = new List<char>();
            endingFolder.Add('_');
            for (int i = 0; i < coreID.Length;i++)
            {
                endingFolder.Add(coreID[i]);
            }
            
            char[] endingFolderArray = endingFolder.ToArray();

            string generalResultsFolder = Regex.Replace(resultsFolder, @"_" +coreID + @"$", String.Empty);

            string generalResultsFolder2 = resultsFolder.TrimEnd(endingFolderArray);

            Console.WriteLine(generalResultsFolder2  + "doesnotwork");

            Console.WriteLine("Creating a General Results Folder at: " + generalResultsFolder + Environment.NewLine);

            if (!Directory.Exists(generalResultsFolder))
            {
                System.IO.Directory.CreateDirectory(generalResultsFolder);
            }

            if (numResultsInResultsFile <= 1)
            {
                Console.WriteLine("We have only one result and may cause problems..." + Environment.NewLine);
                Console.ReadKey();
                Assert.IsTrue(numResultsInResultsFile > 1, "No results in output file");
            }
            //here we want to read in the data and write it out again with a new file name denoted as done
            List<char> ending = new List<char>();
            ending.Add('.'); ending.Add('t'); ending.Add('x'); ending.Add('t');
            char[] endingArray = ending.ToArray();
            string futureName = expectedResultsFilenameSummary.Trim(endingArray) + "_" + coreID + ".txt";

            Console.WriteLine("Lets look and see if file exits: " + futureName);

            if (File.Exists(futureName))
            {
                //this needs to be taken out for running on VMs
                Console.WriteLine("Delete old file" + Environment.NewLine);
                File.Delete(futureName);
            }

            Console.WriteLine("Copy " + expectedResultsFilename + " to " +futureName + Environment.NewLine);



            //try catch loop to make sure the file will copy
            int lockTolken = 0;
            int tryCount = 0;
            int tryMax = 20;//=100 seconds
            while (lockTolken==0)
            {
                try
                {
                    File.Copy(expectedResultsFilename, futureName);//this is all we really care about
                    lockTolken = 1;
                }
                catch (Exception)
                {
                    lockTolken = 0;
                    int seconds = 5;
                    Thread.Sleep(seconds*1000);
                    tryCount++;
                    Console.WriteLine("Write Conflict on " + futureName);
                    if(tryCount>tryMax)
                    {
                        lockTolken = 1;//simmple exit or throw
                        throw;
                    }
                }
            }

            

            Console.WriteLine("Lets look and see if old file exits: " + expectedResultsFilename);

            //clean up old file
            if(File.Exists(expectedResultsFilename))
            {
                Console.WriteLine("Delete old file" + Environment.NewLine);
                //this needs to be taken out for running on VMs
                File.Delete(expectedResultsFilename);
            }


            bool zipdata = false;
            if (zipdata)
            {
                string folderToZip = resultsFolder + @"\XYDataWriter";
                string zippedFolderDestination = resultsFolder + @"\XYDataResults.zip";
                using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile())
                {
                    zip.AddDirectory(folderToZip);
                    zip.Save(zippedFolderDestination);
                }

                if (Directory.Exists(folderToZip) && File.Exists(zippedFolderDestination))
                {
                    Console.WriteLine("Delete XYDataWriterFolder");
                    //deleteFiles = true; this needs to be false for multi core mode
                    if (deleteFiles) System.IO.Directory.Delete(folderToZip, true);
                }
            }



            //delete core specific results folder now that we have copied the data out
            if (Directory.Exists(resultsFolder)) { Directory.Delete(resultsFolder,true); }


            //TODO set lock location to finished
            bool toggleLocks = false;//this is needed for the Vms
            if (toggleLocks)
            {
                if (File.Exists(lockLocation))
                {
                    GetPeaks_DLL.DataFIFO.StringLoadTextFileLine reader = new StringLoadTextFileLine();
                    List<string> statusCheck = reader.SingleFileByLine(lockLocation);
                    string status = statusCheck.FirstOrDefault();
                    if (status == "running")
                    {
                        status = "completed";
                        List<string> newStatus = new List<string>();
                        newStatus.Add(status);
                        newStatus.Add(expectedResultsFilenameSummary);
                        newStatus.Add(futureName);
                        //newStatus.Add(expectedResultsFilename);
                        //GetPeaks_DLL.DataFIFO.StringListToDisk writer = new StringListToDisk();
                        writer.toDiskStringList(lockLocation, newStatus);
                    }

                    string letters = lockLocation;
                    string coreName = lockLocation.Remove(letters.Length - 4, 4);

                    if (File.Exists(coreName + "_Done.txt"))
                    {
                        File.Delete(coreName + "_Done.txt");
                    }
                    File.Copy(lockLocation, coreName + "_Done.txt");
                    //File.Delete(lockLocation);//this will delete the old file

                }
            }




            #region old stuff

           
            //SPIN notes
            //1.  SetExecutorAndFragmentsAndParameters --> BasicTargetedWorkflowExecutorParametersSetIq
            //  fragmentedTargetedWorkflowParameters.ChromSmootherNumPointsInSmooth = 9;

            //2.  SetStrings
            //  peaksTestFile = @"S:\Gly09_SN130_4Mar13_Cheetah_C14_220nL_SPIN_1900V_1600mlmin_22Torr_100C_100kHDR2M2mbar_peaks.txt";
            //  testFile = @" E:\PNNL Data\2013_02_18 SPIN Exactive04\Gly09_SN130_4Mar13_Cheetah_C14_220nL_SPIN_1900V_1600mlmin_22Torr_100C_100kHDR2M2mbar.raw";

            //3.  FragmentedTargetedIQWorkflow.Execute
            //  slashout result.Target.TheorIsotopicProfile = Utiliites.GenerateCombinedIsotopicProfile(result, 1, 1);


            //Switches
            //1.  RunMeIQGlyQ BasicTargetedWorkflowExecutorParameters BasicTargetedWorkflowExecutorParametersSetIq (lc parameters)

            //2.  FragmentedTargetedIQWorkflow.Execute() (isotope profile deuterated)

            //3.  IQGlyQTestingUtilities.ExecutorAndFragmentsAndParameters()  executorParameters.MaxMzForDefiningChargeStateTargets = 2500;

            //4.  FragmentResultsObjectHolderIQ yesParentResult in RemoveInsourceFramgentationIQ
            //

            #endregion
        }




        private static Stopwatch StartClock(out DateTime starttime)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            starttime = DateTime.Now;
            return stopWatch;
        }

        private static void StopClock(DateTime starttime, Stopwatch stopWatch)
        {
            System.DateTime stoptime = DateTime.Now;
            Console.WriteLine("This started at " + starttime + " and ended at" + stoptime);
            Console.WriteLine("This took " + stopWatch.Elapsed + " seconds to find " + 4 + " eluting peaks");
            Console.WriteLine("");
        }
    }
}
