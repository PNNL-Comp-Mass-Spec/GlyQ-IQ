using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.Objects;
using System.IO;
using DeconTools.Backend.Runs;
using GetPeaks_DLL.Go_Decon_Modules;
using PNNLOmics.Data;
using DeconTools.Backend.Core;

namespace GetPeaks_DLL.TandemSupport
{
    public static class GatherDatasetInfo
    {
        public static void GetMSLevelandSize(InputOutputFileName newFile, int limitScansTo, out int sizeOfDatabase, out List<PrecursorInfo> precursorMetaData)
        {
            FileInfo fi = new FileInfo(newFile.InputFileName);
            bool exists = fi.Exists;

            Console.WriteLine("CreateRun: " + newFile.InputFileName + " and its existance is " + exists.ToString());

            RunFactory rf = new RunFactory();

            Run run = GoCreateRun.CreateRun(newFile);

            //Console.WriteLine("after run was created");

            sizeOfDatabase = run.GetNumMSScans() - 1;//this is a crude way to get the size of the database.  -1 is need or the scan set creation will fail

            //this will limit to part of the file if the scans are enumerated
            if (sizeOfDatabase > limitScansTo)
            {
                sizeOfDatabase = limitScansTo;
            }

            Console.WriteLine("Working on caching precursors for {0} scans...", sizeOfDatabase);
            precursorMetaData = new List<PrecursorInfo>(); 
            for (int i = 0; i < sizeOfDatabase; i++)
            {
                //Console.WriteLine("Scan " + i.ToString());
                //if (i > 3611)
                //{
                //    i = i++;
                //    i = i--;
                //}
                
                PrecursorInfo precursor = run.GetPrecursorInfo(i);
                precursorMetaData.Add(precursor);
                //MSLevel.Add(run.GetMSLevel(i));
            }
            run.Dispose();
            //Console.WriteLine("DeleteRun now that it is done");
            //end
        }
    }
}
