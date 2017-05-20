using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetPeaks_DLL.Objects.ParameterObjects
{
    public class BatchFileGetPeaksParameterObject
    {
        public string version {get;set;}
        public string serverDataFileName  {get;set;}
        public string folderID  {get;set;}
        public int startScan {get;set;}
        public int endScan { get; set; }
        public int serverBlockTotal { get; set; }
        public int serverBlock { get; set; }
        public double DataSpecificMassNeutron { get; set; }
        public double part1SN { get; set; }
        public double part2SN { get; set; }
        public string DeconType { get; set; }
        public int numberOfDeconvolutionThreads { get; set; }
        public int part1ScansToSum { get; set; }
        public string sumMethod { get; set; }

        public BatchFileGetPeaksParameterObject()
        {
            version = "default";
            serverDataFileName = @"V:\\GlycoCell01_29Jun11_Cougar_150_60cm_5uL_PO4_100_60_2.raw";
            folderID = "SN343536_150_5uL_S3";
            startScan = 100;
            endScan = 999999;
            serverBlockTotal = 1;
            serverBlock = 0;
            DataSpecificMassNeutron = 1.002149286;
            part1SN = 1;
            part2SN = 3;
            DeconType = "Thrash";
            numberOfDeconvolutionThreads = 1;
            part1ScansToSum = 3;
            sumMethod = "SumScan";
        }
    }
}
