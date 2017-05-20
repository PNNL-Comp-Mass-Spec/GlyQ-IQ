using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GetPeaks_DLL.ConosleUtilities
{
    public class WriteParameterFiles
    {
        public static void writeInputFile(string ServerFileName, string ID)
        {


            //string ID = "SN09";
            int scanend = 3700;
            string outputFileDestination = "";
            //write data to file
            StringBuilder sb = new StringBuilder();
            outputFileDestination = @"D:\PNNL CSharp\0_BatchFiles\ServerParameterFile" + ID + @"a.txt";
            using (StreamWriter writer = new StreamWriter(outputFileDestination))
            {
                #region part 1a

                sb = new StringBuilder();
                sb.Append("data file," + ServerFileName + ", raw yafms data file\n");
                sb.Append("folderID," + ID + ", identifier that is added into the folder name and onto the files\n");
                sb.Append("startScan,0, first scan to start with\n");
                sb.Append("endScan," + scanend + ", last scan to use\n");
                sb.Append("number of serverblocks in total,4, unique block of data to run 4\n");
                sb.Append("serverblock,0, unique block of data to run 0,1,2,3\n");
                sb.Append("Mass neutron data specific,1.002149286, the difference between monoisotopic peak and c13 etc");
                writer.WriteLine(sb.ToString());

                #endregion
            }

            StringBuilder sb1 = new StringBuilder();
            outputFileDestination = @"D:\PNNL CSharp\0_BatchFiles\ServerParameterFile" + ID + @"b.txt";
            using (StreamWriter writer = new StreamWriter(outputFileDestination))
            {
                #region part 2b

                sb1 = new StringBuilder();
                sb1.Append("data file," + ServerFileName + ", raw yafms data file\n");
                sb1.Append("folderID," + ID + ", identifier that is added into the folder name and onto the files\n");
                sb1.Append("startScan,0, first scan to start with\n");
                sb1.Append("endScan," + scanend + ", last scan to use\n");
                sb1.Append("number of serverblocks in total,4, unique block of data to run 4\n");
                sb1.Append("serverblock,1, unique block of data to run 0,1,2,3\n");
                sb1.Append("Mass neutron data specific,1.002149286, the difference between monoisotopic peak and c13 etc");
                writer.WriteLine(sb1.ToString());

                #endregion
            }

            StringBuilder sb2 = new StringBuilder();
            outputFileDestination = @"D:\PNNL CSharp\0_BatchFiles\ServerParameterFile" + ID + @"c.txt";
            using (StreamWriter writer = new StreamWriter(outputFileDestination))
            {
                #region part 3c

                sb2 = new StringBuilder();
                sb2.Append("data file," + ServerFileName + ", raw yafms data file\n");
                sb2.Append("folderID," + ID + ", identifier that is added into the folder name and onto the files\n");
                sb2.Append("startScan,0, first scan to start with\n");
                sb2.Append("endScan," + scanend + ", last scan to use\n");
                sb2.Append("number of serverblocks in total,4, unique block of data to run 4\n");
                sb2.Append("serverblock,2, unique block of data to run 0,1,2,3\n");
                sb2.Append("Mass neutron data specific,1.002149286, the difference between monoisotopic peak and c13 etc");
                writer.WriteLine(sb2.ToString());

                #endregion
            }

            StringBuilder sb3 = new StringBuilder();
            outputFileDestination = @"D:\PNNL CSharp\0_BatchFiles\ServerParameterFile" + ID + @"d.txt";
            using (StreamWriter writer = new StreamWriter(outputFileDestination))
            {
                #region part 4d

                sb3 = new StringBuilder();
                sb3.Append("data file," + ServerFileName + ", raw yafms data file\n");
                sb3.Append("folderID," + ID + ", identifier that is added into the folder name and onto the files\n");
                sb3.Append("startScan,0, first scan to start with\n");
                sb3.Append("endScan," + scanend + ", last scan to use\n");
                sb3.Append("number of serverblocks in total,4, unique block of data to run 4\n");
                sb3.Append("serverblock,3, unique block of data to run 0,1,2,3\n");
                sb3.Append("Mass neutron data specific,1.002149286, the difference between monoisotopic peak and c13 etc");
                writer.WriteLine(sb3.ToString());

                #endregion
            }

        }
    }
}
