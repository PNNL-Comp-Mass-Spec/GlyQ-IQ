using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GetPeaks_DLL.DataFIFO;

namespace CopyFilesGeometrically
{
    class Program
    {

        static void Main(string[] args)
        {
            //args = new string[1];
            //args[0] = @"D:\PNNL CSharp2\SVN CrazyFileCopy\CopyFilesGeometrically\ParametarFileCopyExponential.txt";
            
            

           

            ParametersExponentialCopy parameters = new ParametersExponentialCopy();
            parameters.SetParameters(args[0]);

            //parameters.MultiSleepExecutionPath = @"F:\ScottK\ToPIC\GlyQ-IQ MultiSleep\Release\MultiSleep.exe";
            //parameters.CheckFileExecutionPath = @"F:\ScottK\ToPIC\GlyQ-IQ CheckFile\Release\WriteCheckFile.exe";
            //parameters.DeletefilesFileExecutionPath = @"F:\ScottK\ToPIC\GlyQ-IQ DeleteFiles\Release\DeleteFiles.exe";
            //parameters.WriteLocation = @"D:\CrazyFileCopy";
            //parameters.DataFileNameWithoutEnding = "Gly09_Velos3_Jaguar_230nL30_C15_DB01_30uL1X_30Dec12";
            //parameters.FileExtension = ".raw";

            string multiSleepExecutionPath = parameters.MultiSleepExecutionPath;
            string checkFilePath = parameters.CheckFileExecutionPath;
            string deletefilesFilePath = parameters.DeletefilesFileExecutionPath;
            string writeLocation = parameters.WriteLocation;//@"D:\CrazyFileCopy";
            string fileExtension = parameters.FileExtension;
            string dataFile = parameters.DataFileNameWithoutEnding;
            int numCopies = parameters.NumberOfCopies;

            //setup initial delete log
            List<string> initialFilesToBeDeleted = new List<string>();//we store all files created so we can delete them at the end
            
            //setup initial check file
            StringListToDisk writer = new StringListToDisk();
            List<string> blankcheckFile = new List<string>();
            string initialCheckFile = writeLocation + @"\" + dataFile + ".txt";
            writer.toDiskStringList(initialCheckFile, blankcheckFile); initialFilesToBeDeleted.Add(initialCheckFile);
                
            //compile all levels here
            List<Level> levelList = new List<Level>();

            //setup first level
            CopyObject template = new CopyObject();
            template.FolderLocation = writeLocation;
            template.FileNameBase = parameters.DataFileNameWithoutEnding;
            template.FileNameEnding = fileExtension;
            template.MasterFileID = -1;//-1 for no ending
            template.ChildFileID = 0;

            Level level0 = new Level();
            level0.FileList.Add(template);
            level0.ConvertToStrings();

            Console.WriteLine(level0.LinesToWrite[0]);

            levelList.Add(level0);
           

            //iniital condition for setting up next levels
            List<int> existingFiles = new List<int>();
            List<int> createdFiles = new List<int>();
            existingFiles.Add(-1);
            existingFiles.Add(0);
            createdFiles.Add(0);

            while (createdFiles.Count>0)
            {
                Level level = CalculateLevel(existingFiles, out createdFiles, template, numCopies);
                if (createdFiles.Count > 0)
                {
                    levelList.Add(level);

                    existingFiles.AddRange(createdFiles);
                }
                //createdFiles[createdFiles.Count - 1] <= numCopies && 
            }

            List<string> fullListOfCopiedCheckFiles = new List<string>();
            foreach (Level selectLevel in levelList)
            {
                foreach (var copyObject in selectLevel.FileList)
                {
                    fullListOfCopiedCheckFiles.Add(copyObject.FileNameBase + "_" +copyObject.ChildFileID +  ".txt");//all check files
                }
            }

            //write batch files
            List<string> cumulativeFilesWritten;
            WriteData(writeLocation, multiSleepExecutionPath, checkFilePath, deletefilesFilePath, initialFilesToBeDeleted, fullListOfCopiedCheckFiles, out cumulativeFilesWritten, levelList);

            
        }

        //TODO
        //we need an overall "FilesNeeded_Level_All" that has all the written files at end of run
        //this way, we can run a MultiTimer and when all the files are there, launch x_Delete

        //we also need something to write and delete the initial check file (and delete the delete.text file


        private static void WriteData(string writeLocation, string multiSleepExecutionPath, string checkFilePath, string deletefilesFilePath, List<string> initialFilesToBeDeleted, List<string> fullListOfCopiedFiles, out List<string> cumulativeFilesWritten, List<Level> levelList)
        {
            StringListToDisk writer = new StringListToDisk();

            //files required to advance levels
            
            cumulativeFilesWritten = new List<string>();
            cumulativeFilesWritten.AddRange(initialFilesToBeDeleted);
            
            
            List<string> parameterFileNames_B = new List<string>();
            for (int i = 0; i < levelList.Count; i++)
            {
                List<string> parameterFileLines = new List<string>();
                
                Level currentLevel = levelList[i];

                string baseName = "FileCopy_Level_" + i;

                List<string> miniNameList = new List<string>();
                for (int j = 0; j < currentLevel.LinesToWrite.Count; j++)
                {
                    //this runs the indifidual file copy.  one file per batch file
                    string batchName = "D_" + baseName +"_" + j +".bat";
                    miniNameList.Add("Start " + writeLocation + @"\" + batchName);
                    //this is really bad here
                    //int next = i + 1;
                    //parameterFileLines.Add(@"Start D:\CrazyFileCopy\B_FileCopy_Level_" + next + ".bat");
                    List<string> singleCopy = new List<string>();
                    singleCopy.Add(currentLevel.LinesToWrite[j]);

                    //check file
                    string checkFilename = currentLevel.FileList[0].FolderLocation + @"\" + currentLevel.FileList[0].FileNameBase + "_" + currentLevel.FileList[j].ChildFileID + ".txt";

                    //write check file 
                    singleCopy.Add("\"" + checkFilePath + "\"" + " " + "\"" + checkFilename + "\""); cumulativeFilesWritten.Add(checkFilename);

                    singleCopy.Add("Exit");
                    writer.toDiskStringList(writeLocation + @"\" + batchName, singleCopy); cumulativeFilesWritten.Add(writeLocation + @"\" + batchName);
                }

                string controllerBatchName = "C_" + baseName +".bat";
                //batchFileNames.Add("Call " + controllerBatchName);
                //batchFileNames.Add("Pause");
                //batchFileNames.Add(@"D:\CrazyFileCopy\" + controllerBatchName);
                miniNameList.Add("Exit");
                writer.toDiskStringList(writeLocation + @"\" + controllerBatchName, miniNameList); cumulativeFilesWritten.Add(writeLocation + @"\" + controllerBatchName);

                //here is a list of all files needed prior to copying
                string parameterFilePath;
                SetListOfRequiredFilesForALevel(i, currentLevel, writeLocation, currentLevel.FileList[0].FolderLocation, controllerBatchName, out parameterFilePath, ref cumulativeFilesWritten);
                
                //this needs to go into a batch file B

                string controllerBatchName_LebelB = "B_" + baseName + ".bat";
                parameterFileNames_B.Add("Start " + writeLocation + @"\" + controllerBatchName_LebelB);
                //we need to delay the execution so the renadom numbers dont hit.  apps can make calls faster than 1milisecond so you can have the same random numbers
                int bonusSeconds = 6;//0-10
                int timedelayinseconds = levelList.Count * 3 - 3 * i + bonusSeconds;
                //timedelayinseconds = 0;
                parameterFileLines.Add("timeout /T " + timedelayinseconds  +  @" /NOBREAK > NUL");
                parameterFileLines.Add("\"" + multiSleepExecutionPath + "\"" + " " + "\"" + parameterFilePath + "\"");
                //int next = i + 1;
                //parameterFileLines.Add(@"Start D:\CrazyFileCopy\B_FileCopy_Level_" + next + ".bat");
                parameterFileLines.Add("Exit");
                writer.toDiskStringList(writeLocation + @"\" + controllerBatchName_LebelB, parameterFileLines); cumulativeFilesWritten.Add(writeLocation + @"\" + controllerBatchName_LebelB);


                
            }

            //finally write the launch batch file
            //string launchBatchName = "A_" + "Launch" + ".bat";
            //writer.toDiskStringList(writeLocation + @"\" + launchBatchName, batchFileNames);

           


          


            //write out full copy list so we can auto execute the delete
            string autoCheckAndDeleteName = "x_" + "FullCheckList" + ".txt";
            writer.toDiskStringList(writeLocation + @"\" + autoCheckAndDeleteName, fullListOfCopiedFiles); cumulativeFilesWritten.Add(writeLocation + @"\" + "x_" + "FullCheckList" + ".txt");

            string finalCheckListBatchName = "x_" + "FinalCheckList" + ".bat";
            string autoCheckAndDeleteNameBatch = finalCheckListBatchName; cumulativeFilesWritten.Add(writeLocation + @"\" + finalCheckListBatchName);
            List<string> linesForCheckAndDelete = new List<string>();

            //make parameter file for FUllCheckList
            string parameterFileForMultiSleep = writeLocation + @"\" + "MultiSleepParameter_FullCheckList.txt";
            List<string> multiSleepData = new List<string>();
            multiSleepData.Add("FileToWaitFor," + writeLocation + @"\" + autoCheckAndDeleteName);
            multiSleepData.Add("BatchFileToRunAfterLoop," + writeLocation + @"\" + "x_" + "DeleteFiles" + ".bat");//nothing needs to be run
            //multiSleepData.Add("WorkingFolder," + writeLocation + @"\" + "BaseFile");
            multiSleepData.Add("WorkingFolder," + writeLocation);
            multiSleepData.Add("Seconds," + 5);
            writer.toDiskStringList(parameterFileForMultiSleep, multiSleepData); cumulativeFilesWritten.Add(parameterFileForMultiSleep);


            string finalParameterFilePath = parameterFileForMultiSleep;
            linesForCheckAndDelete.Add("\"" + multiSleepExecutionPath + "\"" + " " + "\"" + finalParameterFilePath + "\"");
            linesForCheckAndDelete.Add("Exit");
            writer.toDiskStringList(writeLocation + @"\" + autoCheckAndDeleteNameBatch, linesForCheckAndDelete); cumulativeFilesWritten.Add(writeLocation + @"\" + finalCheckListBatchName);



            //write execution parameters and multisleep
            string launchBatchNameTimer = "A_" + "Launch" + ".bat";
            parameterFileNames_B.Add("Start " + writeLocation + @"\" + finalCheckListBatchName);
            parameterFileNames_B.Add("Exit");

            writer.toDiskStringList(writeLocation + @"\" + launchBatchNameTimer, parameterFileNames_B); //cumulativeFilesWritten.Add(writeLocation + @"\" + launchBatchNameTimer);





            //write files to delete
            string deleteFilesParametersName = "x_" + "DeleteFiles" + ".txt"; cumulativeFilesWritten.Add(writeLocation + @"\" + deleteFilesParametersName);
            string deleteFilesBatchName = "x_" + "DeleteFiles" + ".bat"; cumulativeFilesWritten.Add(writeLocation + @"\" + deleteFilesBatchName);

            //lines for deleteing
            List<string> deleteData = new List<string>();
            deleteData.Add("\"" + deletefilesFilePath + "\"" + " " + "\"" + writeLocation + @"\" + deleteFilesParametersName + "\"");
            //deleteData.Add("DEL "+ "\"" + "%~f0" + "\"");
            deleteData.Add("Exit");

            writer.toDiskStringList(writeLocation + @"\" + deleteFilesBatchName, deleteData);
            writer.toDiskStringList(writeLocation + @"\" + deleteFilesParametersName, cumulativeFilesWritten);

            //Console.WriteLine(cumulativeFilesWritten.Count);
            //int counterTODelete = 0;
            //foreach (string fileToBeDeleted in cumulativeFilesWritten)
            //{

            //    if (File.Exists(fileToBeDeleted))
            //    {
            //        Console.WriteLine("LetsDeleteTHis:" + fileToBeDeleted);
            //        counterTODelete++;
            //        File.Delete(fileToBeDeleted);
            //    }

            //}

            //Console.WriteLine("we have " + counterTODelete + "out of " + cumulativeFilesWritten.Count + " files");
            //Console.ReadKey();
        }

        private static void SetListOfRequiredFilesForALevel(int i, Level currentLevel, string writeLocation, string folderLocation, string controllerBatchName, out string parameterFilePath, ref List<string> cumulativeFilesWritten)
        {
            StringListToDisk writer = new StringListToDisk();
            
            List<string> filesRequired = new List<string>();
            foreach (CopyObject item in currentLevel.FileList)
            {
                string requiredFile = "";

                //string ending = item.FileNameEnding;
                string ending = ".txt";//check files
                if (item.MasterFileID < 0)
                {
                    requiredFile = item.FileNameBase + ending;
                    //requiredFile = "\"" + item.FolderLocation + @"\" + item.FileNameBase + item.FileNameEnding + "\"";
                }
                else
                {
                    requiredFile = item.FileNameBase + "_" + item.MasterFileID + ending;
                    //requiredFile = "\"" + item.FolderLocation + @"\" + item.FileNameBase + "_" + item.MasterFileID + item.FileNameEnding + "\"";
                }

                filesRequired.Add(requiredFile);
            }

            string filesParameterFileNames = writeLocation + @"\" + "FilesNeeded_Level_" + i + ".txt";
            writer.toDiskStringList(filesParameterFileNames, filesRequired); cumulativeFilesWritten.Add(filesParameterFileNames);

            //Set up multi sleep calls
            //write parameter files
            //F:\ScottK\ToPIC\GlyQ-IQ MultiSleep\Release

            //FileToWaitFor,F:\ScottK\ToPIC\Results\LocksFolder\LockController.txt
            //BatchFileToRunAfterLoop,E:\ScottK\Shared_PICFS\ToPIC\testBatch.bat
            //WorkingFolder,F:\ScottK\ToPIC\Results\LocksFolder
            //Seconds,5

            string parameterFileForMultiSleep = writeLocation + @"\" + "MultiSleepParameter_" + i + ".txt";
            List<string> multiSleepData = new List<string>();
            multiSleepData.Add("FileToWaitFor," + filesParameterFileNames);
            multiSleepData.Add("BatchFileToRunAfterLoop," + writeLocation + @"\" + controllerBatchName);//nothing needs to be run
            //multiSleepData.Add("WorkingFolder," + writeLocation + @"\" + "BaseFile");
            multiSleepData.Add("WorkingFolder," + folderLocation);
            multiSleepData.Add("Seconds," + 5);
            writer.toDiskStringList(parameterFileForMultiSleep, multiSleepData); cumulativeFilesWritten.Add(parameterFileForMultiSleep);

            parameterFilePath = parameterFileForMultiSleep;
        }

        private static Level CalculateLevel(List<int> existingFiles, out List<int> newFilesIntegers, CopyObject template, int copyLimit)
        {
            Level newLevel = new Level();

            newFilesIntegers = new List<int>();

            existingFiles.Sort();
            int maxEnding = existingFiles[existingFiles.Count - 1];

            int newFilecounter = 1;
            //thuis needs to go in reverse order so the last set corresponds to the last files from previous level
            //foreach (int fileEnding in existingFiles)
            for(int k=existingFiles.Count-1;k>=0;k--)
            {
                int fileEnding = existingFiles[k];
                
                CopyObject newFile = new CopyObject();
                newFile.MasterFileID = fileEnding;
                int newEnding = maxEnding + newFilecounter;
                newFile.ChildFileID = newEnding;

                //copy template
                newFile.FileNameBase = template.FileNameBase;
                newFile.FileNameEnding = template.FileNameEnding;
                newFile.FolderLocation = template.FolderLocation;

                if (newEnding <= copyLimit)
                {
                    newFilesIntegers.Add(newEnding);
                    newLevel.FileList.Add(newFile);
                    newFilecounter++;
                }
                else
                {
                    //we have reached the limit
                    break;
                }
            }

            newLevel.ConvertToStrings();

            return newLevel;
        }

        //private static void duplicate(Level level, int levelNum, int oldNumCopies, int nextNumCopies)
        //{
        //    Level levelNext = new Level();

        //    //if there is less than a full level
        //    if (nextNumCopies > (numCopies - 1))
        //        nextNumCopies = numCopies - 1;

        //    int fileIndex = 0;
        //    int copyNumber = oldNumCopies + 1;

        //    while (fileIndex < level.FileList.Count && (copyNumber - oldNumCopies) <= nextNumCopies)
        //    {
        //        CopyObject copy = new CopyObject();
        //        copy.FolderLocation = level.FileList[fileIndex].FolderLocation;
        //        copy.FileNameBase = level.FileList[fileIndex].FileNameBase;
        //        copy.FileNameEnding = level.FileList[fileIndex].FileNameEnding;
        //        copy.MasterFileID = level.FileList[fileIndex].ChildFileID;
        //        copy.ChildFileID = copyNumber;

        //        levelNext.FileList.Add(copy);

        //        copyNumber++;
        //        fileIndex++;
        //        if (fileIndex == level.FileList.Count && (copyNumber - oldNumCopies) <= nextNumCopies)
        //            fileIndex = 0;
        //    }

        //    levelNext.ConvertToStrings();

        //    for (int i = 0; i < levelNext.LinesToWrite.Count; i++)
        //    {
        //        Console.WriteLine(levelNext.LinesToWrite[i]);
        //        Console.ReadKey();
        //    }

        //    if (!(nextNumCopies >= (numCopies - 1)))
        //    {
        //        levelList.Add(levelNext);
        //        levelNum++;
        //        oldNumCopies = nextNumCopies;
        //        nextNumCopies *= 2;
        //        duplicate(levelNext, levelNum, oldNumCopies, nextNumCopies);
        //    }
        //}

    }
}


