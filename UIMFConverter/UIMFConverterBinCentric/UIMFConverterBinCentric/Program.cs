using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UIMFLibrary;

namespace UIMFConverterBinCentric
{
    class Program
    {
        //args "Gly09_SN130_8Mar13_Cheetah_C14_220nL_IMS6_2700V_130C_Multi_" "D:\Csharp\ConosleApps\LocalServer\UIMF" ""S:" ".uimf"
        static void Main(string[] args)
        {

            //input
            string targetFolder = @"D:\Csharp\ConosleApps\LocalServer\UIMF";
            string destinationProcessingTempFolder = @"S:";
            string fileName = "Gly09_SN130_8Mar13_Cheetah_C14_220nL_IMS6_2700V_130C_Multi_";
            string fileNameEnding = ".uimf";

            string addOn = "BC";

            if(args.Length == 4)
            {
                fileName = args[0];//the UIMF file of interest
                targetFolder = args[1];//the folder where the UIMF is
                destinationProcessingTempFolder = args[2];// where we will process the file
                fileNameEnding = args[3];
            }

            //set up locations
            string targetfileName = fileName + fileNameEnding;
            string desinationFileName = fileName + "_" + addOn + fileNameEnding;
            string target = targetFolder + "\\" + targetfileName;
            string destination = destinationProcessingTempFolder + "\\" + desinationFileName;


            //copy file to working location
            XCopyFX.CopyFile.ProcessXcopyFile(target, destination);

            FileInfo uimfFile = new FileInfo(destination);

            DataWriter uimfWriter = new DataWriter();
            uimfWriter.OpenUIMF(uimfFile.FullName);

            uimfWriter.CreateBinCentricTables();

            uimfWriter.CloseUIMF();

            Console.WriteLine("Copy Back");

            XCopyFX.CopyFile.ProcessXcopyFile(destination, targetFolder + "\\" + desinationFileName);

            Console.WriteLine("Delete");

            if (uimfFile.Exists)
            {
                uimfFile.Delete();
            }
        }
    }
}
