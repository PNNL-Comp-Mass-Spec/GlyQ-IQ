using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GetPeaks_DLL.DataFIFO
{
    public class FileListNameImporter
    {
        private string m_sourceFileName;  //location of filelist containng a list of spectra

        public FileListNameImporter(string filenamepath)
        {
            m_sourceFileName = filenamepath;
            Console.WriteLine("SetFile Path and Name:\n" + filenamepath);
        }

        #region Public Methods

        public List<string> ImportFileList()
        {//empty list to store filenames to load is sent in

            Console.WriteLine("LoadFilesList\n");

            //set up Lists
            List<string> fileList = new List<string>();

            //set up variables
            int length;         //length of index
            int i;                //counter

            //set up arrays
            string[] lines = { };

            //needs to be set in parent class
            string indexpathSource = m_sourceFileName;

            //check for NULL.  Make sure this is set in parent class
            StreamReader streamReadFile;

            try
            {
                streamReadFile = new StreamReader(indexpathSource);
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n" + @"No source path included" + "\n");
                return fileList;
            }

            string fileContents = streamReadFile.ReadToEnd();    //  Read in entire file to fileContents
            streamReadFile.Close();

            //parse lines and store into list arrays
            lines = fileContents.Split(
                Environment.NewLine.ToCharArray(),  //'\n', '\r'
                StringSplitOptions.RemoveEmptyEntries);//does not include empty lines

            
            length = lines.Length;
            Console.WriteLine("There are " + length + " lines total\n");

            for (i = 0; i < length; i++)
            {
                fileList.Add(lines[i]);
            }

            //set new length
            length = fileList.Count;
            Console.WriteLine("There are now " + length + " lines\n");

            //print full file to console
            for (i = 0; i < length; i++)
            {
                Console.WriteLine("File -->" + fileList[i] + "\n");
            }
            Console.WriteLine("\n");
            return fileList;
        }

        #endregion

        #region stringBuilder region
        public void ImporteFileList(List<string> fileslist, System.Text.StringBuilder sb)
        {
            sb.Append(12);
            sb.Append("555");

            Console.Write(sb.ToString());
        }
        #endregion
    }
}
