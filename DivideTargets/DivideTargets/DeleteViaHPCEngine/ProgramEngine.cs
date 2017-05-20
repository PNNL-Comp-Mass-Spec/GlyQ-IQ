using System;
using System.Collections.Generic;
using ParalellTargetsLibrary;

namespace DeleteViaHPCEngine
{
    class ProgramEngine
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting up Delete Engine...");
            //string filePath = @"\\picfs\projects\DMS\PIC_HPC\Hot\FO_Peptide_Merc_08_2Feb11_Sphinx_11-01-17_1\WorkingParameters\HPC_DeleteFilesAndFolders.txt";
            //int rank = 0;

            string filePath = args[0];

            int cores = Convert.ToInt32(args[1]);// +1;//the +1 is so we don't verflow. we have a -1 on the parameter sweep
            int rank = Convert.ToInt32(args[2])-1;//-1 is nedded to get to first rank

            Console.WriteLine("file is " + filePath);
            Console.WriteLine("rank is " + rank);


            Console.WriteLine("Load File and folder names");
            List<ItemOnDisk> files = new List<ItemOnDisk>();
            

            StringLoadTextFileLine reader = new StringLoadTextFileLine();
            List<string> lines = reader.SingleFileByLine(filePath);
            //parse lines
            char[] splitter = new char[] { ',' };

            int totalFilesCount = 0;
            string[] wordsForCount = lines[0].Split(splitter, StringSplitOptions.None);
            if (wordsForCount.Length == 2)
            {
                totalFilesCount = Convert.ToInt32(wordsForCount[1]);
            }



            for(int i=1;i<lines.Count;i++)
            {
                var dataToSplit = lines[i];
                string[] words = dataToSplit.Split(splitter, StringSplitOptions.None);
                if (words.Length == 2)
                {
                    ItemOnDisk newItem = new ItemOnDisk();
                    
                    switch (words[0])
                    {
                        case "File":
                            {
                                newItem.Type = itemType.File;
                            }
                            break;
                        case "Directory":
                            {
                                newItem.Type = itemType.Directory;
                            }
                            break;
                    }
                    newItem.path = words[1];
                    files.Add(newItem);
                }
            }


            //step 2, break into ranks based on totalFilesCount
            List<int> targetsIDs = new List<int>();
            for(int i=0;i<files.Count;i++)
            {
                targetsIDs.Add(i);
            }

            List<List<int>> pileOfSelectedTargets = GetValue(targetsIDs, cores);


            if (rank < pileOfSelectedTargets.Count)
            {
                List<int> filesForOneCoreAtRank = pileOfSelectedTargets[rank];
            
                foreach (var i in filesForOneCoreAtRank)
                {
                    Console.WriteLine("Delete file at idex " + i);
                }
            

                //step 3  select items corresponding to rank

                List<ItemOnDisk> rankedItems = new List<ItemOnDisk>();
                foreach (var i in filesForOneCoreAtRank)
                {
                    rankedItems.Add(files[i]);
                }
            

            foreach (var fileToDelete in rankedItems)
            {
                Console.WriteLine("Delete file  " + fileToDelete.path + " and it is a " + fileToDelete.Type);

                switch (fileToDelete.Type)
                {
                    case itemType.File:
                        {
                            try
                            {
                                System.IO.File.Delete(fileToDelete.path);
                            }
                            catch
                            {
                            }
                        }
                        break;
                    case itemType.Directory:
                        {
                            try
                            {
                                System.IO.Directory.Delete(fileToDelete.path,true);
                            }
                            catch
                            {
                            }
                        }
                        break;
                }
            }
            }
            else
            {
                Console.WriteLine("Delete did not work");
                Console.WriteLine("The Rank is " + rank + " out of " + pileOfSelectedTargets.Count);
            }
            //Console.ReadKey();
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
    }
}
