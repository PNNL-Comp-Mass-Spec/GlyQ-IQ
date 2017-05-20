using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DivideTargetsLibraryX64.FromGetPeaks;
using IQ.Workflows.FileIO;
using IQ_X64.Workflows.Core;
using IQ_X64.Workflows.FileIO;
using IQ_X64.Workflows.FileIO.DTO;
using IQ_X64.Workflows.FileIO.Importers;
using ParalellTargetsLibraryX64;

namespace DivideTargetsLibraryX64
{
    public static class DivideTargetsFile
    {
        public static void TargetsFolderSetup(string fullTargetsPath, string copyToFolder, int cores, string baseTargetsFile, string textFileEnding, out List<string> dividedTargetsFile)
        {
            dividedTargetsFile = new List<string>();

            Console.WriteLine("TheFullTargetsPath is: " + fullTargetsPath);

            SortedDictionary<int, TargetedResultDTO> targetsDictionary = SetupTargetsDictionary(fullTargetsPath);
            SortedDictionary<int, IqTarget> targetsDictionarySimple = SetupTargetsDictionarySimple(fullTargetsPath);
            List<int> targetsIDs = targetsDictionary.Keys.ToList();

            Console.WriteLine("targetsDictionary is " + targetsDictionary.Count + " long");
            Console.WriteLine("targetsDictionarySimple is " + targetsDictionarySimple.Count + " long");

            if(targetsDictionary.Count!=targetsDictionarySimple.Count)
            {
                Console.WriteLine("There is likely a duplicate entry in the targets library");
                Console.Read();
            }

            //Break problem down into separate computers
            List<List<int>> pileOfSelectedTargets = GetValue(targetsIDs, cores);

            StringListToDisk writer = new StringListToDisk();

            Console.WriteLine("Setting Up targets");
         
            for (int i = 0; i < cores; i++)
            {
                //string coreName = "_" + i;
                string coreName = "_" + (i + 1);//needed to shift i by 1 since we are 1 indexing not 0
                string copiedName = copyToFolder + "\\" + baseTargetsFile + coreName + textFileEnding;

                //Console.WriteLine("We are looking for " + copiedName);

                if (File.Exists(copiedName))
                {
                    File.Delete(copiedName);
                }

                //Console.WriteLine("The Pile is " + pileOfSelectedTargets.Count + " Tall");
                List<int> currentTargetID = pileOfSelectedTargets[i];

                List<TargetedResultDTO> currentTargets = new List<TargetedResultDTO>();
                List<IqTarget> currentTargetsSimple = new List<IqTarget>();
                foreach (int iD in currentTargetID)
                {
                    //Console.WriteLine("The ID is " + iD);
                    currentTargets.Add(targetsDictionary[iD]);
                    currentTargetsSimple.Add(targetsDictionarySimple[iD]);
                }

                List<string> lines = new List<string>();
                string header = "";

                for (int j = 0; j < currentTargets.Count; j++)
                {
                    TargetedResultDTO selectTarget = currentTargets[j];
                    IqTarget selectTargetSimple = currentTargetsSimple[j];

                    string targetString = TargetToString.SetString(selectTarget, selectTargetSimple, out header);

                    lines.Add(targetString);
                }

                //Console.WriteLine("We Are going to write "  + lines.Count + "Lines");

                //runLines[i].TargetsFile = baseTargetsFile + coreName + textFileEnding;
                dividedTargetsFile.Add(baseTargetsFile + coreName + textFileEnding);
                writer.toDiskStringList(copiedName, lines, header);
            }
         

            Console.WriteLine(cores + " targets are setup");
        }

        private static List<List<int>> GetValue(List<int> targets, int cores)
        {
            List<List<int>> pileOfSelectedScans = new List<List<int>>();
            List<int> allScans = new List<int>();
            for (int rank = 0; rank < cores; rank++)
            {
                List<int> selectedScans = ParalellSelectRange.Range(targets, cores, rank);
                pileOfSelectedScans.Add(selectedScans);
                allScans.AddRange(selectedScans);
            }

            if (allScans.Count == targets.Count)
            {
                Console.WriteLine("Correct Process Divider");
            }
            return pileOfSelectedScans;
        }

        private static SortedDictionary<int, TargetedResultDTO> SetupTargetsDictionary(string targetsFilePath)
        {
            //IqTargetImporter targetImporter = new BasicIqTargetImporter(targetsFilePath);
            UnlabelledTargetedResultFromTextImporter targetImporter = new UnlabelledTargetedResultFromTextImporter(targetsFilePath);
            TargetedResultRepository loadedTargets = targetImporter.Import();

            SortedDictionary<int, TargetedResultDTO> targetsDictionary = new SortedDictionary<int, TargetedResultDTO>();

            int counter = 0;
            foreach (TargetedResultDTO target in loadedTargets.Results)
            {
                targetsDictionary.Add(counter, target);
                counter++;
            }
            return targetsDictionary;
        }

        private static SortedDictionary<int, IqTarget> SetupTargetsDictionarySimple(string targetsFilePath)
        {
            IqTargetImporter targetImporter = new BasicIqTargetImporter(targetsFilePath);
            //UnlabelledTargetedResultFromTextImporter targetImporter = new UnlabelledTargetedResultFromTextImporter(targetsFilePath);
            List<IqTarget> loadedTargets = targetImporter.Import();

            SortedDictionary<int, IqTarget> targetsDictionary = new SortedDictionary<int, IqTarget>();

            int counter = 0;
            foreach (IqTarget target in loadedTargets)
            {
                targetsDictionary.Add(counter, target);
                counter++;
            }
            return targetsDictionary;
        }

    }
}
