//This software converts Xcalibur raw files to yafms database files
//Usage: raw2yafms rawfile [outfile]
//       If outfile is not specified, then outfile = rawfile_root.yafms
//       where rawfile_root is rawfile without extension
//Author: Yan Shi, PNNL
//        Aaron Robinson, PNNL, 2010
//Date: 6/17/2009

//#define TESTING
#define PRODUCTION

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using XRAWFILE2Lib;
using YafmsLibrary;
using System.IO.Compression;
using RawConvertToYafms_DLL;


namespace Converter
{
    class BatchConvert
    {
        //Be sure to set the build to X86
        static void Main(string[] args)
        {
            List<string> fileNames = new List<string>();
            //fileNames.Add(@"e:\YAFMS\20100722_glycan_SN24_NF.raw");
            //fileNames.Add(@"e:\YAFMS\SKrone_Glycan_2.raw");
            fileNames.Add(@"D:\Csharp\YAFMS\fresh copy\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.RAW");
            //fileNames.Add(@"V:\SKrone_SN25_CID.raw");
            
            for (int i = 0; i < fileNames.Count; i++)
            {
                RawConvertToYafms.Launch(fileNames[i]);
                string[] fileroot = fileNames[i].Split(new char[] { '.' });
                Console.WriteLine("The new YAFMS file was made: " + fileroot[0] + ".yafms");
            }
            Console.WriteLine("Sucess!!  Press return to end");
            Console.ReadKey();
        }
    }
}
