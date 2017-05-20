using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Backend.Core;
using DeconTools.Backend.Utilities;
using DeconTools.Backend.ProcessingTasks;
using GetPeaks_DLL.Objects;
using GetPeaks_DLL.DataFIFO;
using GetPeaks_DLL.Functions;
using PNNLOmics.Data;
using GetPeaks_DLL.Summing;

namespace GetPeaks_DLL
{  
    //this class returns the XYdata from the disk file
    public class GoDeconToolsControllerA: IDisposable
    {
 //       public MSGeneratorFactory msgenFactory { get; set; }//= new MSGeneratorFactory();
 //       public Task msGenerator { get; set; }
//        public SimpleWorkflowParameters Parameters { get; set; }

        GoDeconToolsControllerA()
        {
  //          this.msgenFactory = new MSGeneratorFactory();
        }

        public GoDeconToolsControllerA(Run run, SimpleWorkflowParameters parameters)
            : this()
        {
   //         GOinitializeTasks(run, parameters);
        }

        private void GOinitializeTasks(Run run, SimpleWorkflowParameters parameters)
        {
 //           this.msGenerator = msgenFactory.CreateMSGenerator(run.MSFileType);
 //           this.Parameters = parameters;
        }

        /// <summary>
        /// This is isolated because it loads the file from the disk (which is not threadable) and then since it has the raw data, summs it via decon msGenerator
        /// used with an array of scan sets.  This returns the summed XYData stored in run.ResultsCollection.XYData
        /// </summary>
        /// <param name="run"></param>
        /// <param name="parameters"></param>
        //public void GoLoadDataAndSumIt(Run run, SimpleWorkflowParameters parameters)
        public void GoLoadDataAndSumIt(Run run, ScanSumSelectSwitch switchMaxScanOrSum, int peakFirstScan, int peakLastScan)
        {
            switch (switchMaxScanOrSum)
            {
                case ScanSumSelectSwitch.AlignScan://need one or more scans in a scan set
                    {
                        //this has problems at high m/z because the spacing changes in an odd way
                        Console.WriteLine("You had better check that the file says .raw and SumScan or max scan is selected");
                        Console.ReadKey();
                        AllignAndSum4 letsAllignAndSum = new AllignAndSum4();
                        letsAllignAndSum.AllignXAxisAndSum(run, peakFirstScan, peakLastScan);
                        break;
                    }
                case ScanSumSelectSwitch.MaxScan:
                    {
                        GetXYData.GetData(run);//needs one scan in a scan set
                        break;
                    }
                case ScanSumSelectSwitch.SumScan:
                    {
                        GetXYData.GetData(run);//needs one or more scans in a scan set
                        break; 
                    }
                default:
                    {
                        Console.Write("Default ControllerA");
                        Console.ReadKey();
                        GetXYData.GetData(run);//needs one or more scans in a scan set
                        break;
                    }
            }
        }

        public void GoLoadDataAndSumIt(Run run)
        {
            GetXYData.GetData(run);//needs one or more scans in a scan set      
        }        

        #region IDisposable Members

        public void Dispose()
        {
            
   //         this.msgenFactory = null;
   //         this.msGenerator.Cleanup();
   //         this.msGenerator = null;
 //           this.Parameters.Dispose();
 //           this.Parameters = null;
        }

        #endregion
    }
}
