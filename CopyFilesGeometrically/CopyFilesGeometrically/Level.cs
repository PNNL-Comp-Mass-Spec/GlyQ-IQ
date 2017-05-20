using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CopyFilesGeometrically
{
    public class Level
    {
        public List<CopyObject> FileList { get; set; }

        public List<string> LinesToWrite { get; set; } 
 
        public Level()
        {
            FileList = new List<CopyObject>();
            LinesToWrite = new List<string>();
        }

        public void ConvertToStrings()
        {
            foreach (var copyObject in FileList)
            {

                string line = CopyLine(copyObject);
                LinesToWrite.Add(line);
            }
        }


        private static string CopyLine(CopyObject firstCopy)
        {
            string copyLine = "";
            if (firstCopy.MasterFileID < 0)
            {
                copyLine = "echo f | xcopy /Y " +
                    "\"" + firstCopy.FolderLocation + @"\" + firstCopy.FileNameBase + firstCopy.FileNameEnding + "\"" + " " +
                    "\"" + firstCopy.FolderLocation + @"\" + firstCopy.FileNameBase + "_" + firstCopy.ChildFileID + firstCopy.FileNameEnding + "\"";
            }
            else
            {
                copyLine = "echo f | xcopy /Y " +
                    "\"" + firstCopy.FolderLocation + @"\" + firstCopy.FileNameBase + "_" + firstCopy.MasterFileID + firstCopy.FileNameEnding + "\"" + " " +
                    "\"" + firstCopy.FolderLocation + @"\" + firstCopy.FileNameBase + "_" + firstCopy.ChildFileID + firstCopy.FileNameEnding + "\"";
            }

            return copyLine;
        }
    }
}
