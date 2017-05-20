using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Backend.Core;
using GetPeaks_DLL.Objects;
using DeconTools.Backend.Runs;

namespace GetPeaks_DLL.Go_Decon_Modules
{
    public static class GoCreateRun
    {
        public static Run CreateRun(InputOutputFileName newFile)
        {
            RunFactory rf = new RunFactory();
            Run run;
            //Console.WriteLine("RunFactory Setup, press enter to continue");
            //Console.ReadKey();

            string fileTypeFromFile = GoGetFileType.GetFileExtension(newFile.InputFileName);

            switch (fileTypeFromFile)
            {
                case "raw":
                    {
                        run = rf.CreateRun(DeconTools.Backend.Globals.MSFileType.Finnigan, newFile.InputFileName);
                    }
                    break;
                case "yafms":
                    {
                        run = rf.CreateRun(DeconTools.Backend.Globals.MSFileType.YAFMS, newFile.InputFileName);
                    }
                    break;
                case "d":
                    {
                        run = rf.CreateRun(DeconTools.Backend.Globals.MSFileType.Agilent_D, newFile.InputFileName);
                    }
                    break;
                default:
                    {
                        run = rf.CreateRun(DeconTools.Backend.Globals.MSFileType.YAFMS, newFile.InputFileName);
                    }
                    break;

            }

            return run;
        }

        public static Run CreateRun(InputOutputFileName newFile, out string fileTypeFromFile)
        {
            RunFactory rf = new RunFactory();
            Run run;
            //Console.WriteLine("RunFactory Setup, press enter to continue");
            //Console.ReadKey();

            fileTypeFromFile = GoGetFileType.GetFileExtension(newFile.InputFileName);

            switch (fileTypeFromFile)
            {
                case "raw":
                    {
                        run = rf.CreateRun(DeconTools.Backend.Globals.MSFileType.Finnigan, newFile.InputFileName);
                    }
                    break;
                case "yafms":
                    {
                        run = rf.CreateRun(DeconTools.Backend.Globals.MSFileType.YAFMS, newFile.InputFileName);
                    }
                    break;
                case "d":
                    {
                        run = rf.CreateRun(DeconTools.Backend.Globals.MSFileType.Agilent_D, newFile.InputFileName);
                    }
                    break;
                default:
                    {
                        run = rf.CreateRun(DeconTools.Backend.Globals.MSFileType.YAFMS, newFile.InputFileName);
                    }
                    break;

            }

            return run;
        }

    }
}
