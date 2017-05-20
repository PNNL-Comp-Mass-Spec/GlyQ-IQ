using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Synthetic_Spectra_DLL_X64_Net_4.LoadFile
{
    /// <summary>
    /// this class loads file names from a text file for subsequent loading.  The order matters.
    /// </summary>
    public class FileListImporter
    {
        private string m_sourceFileName;  //location of filelist containng a list of spectra


        /// <summary>
        /// This mehod sets the file path
        /// </summary>
        /// <param name="filenamepath">
        /// the file path and name of file
        /// </param>
        public FileListImporter(string filenamepath)
        {
            m_sourceFileName = filenamepath;
            Console.WriteLine("SetFile Path and Name:\n" + filenamepath);
        }

        /// <summary>
        /// This method loads the text file containing locations for files to load
        /// </summary>
        /// <param name="fileList"></param>
        public void ImportFileList(List<string> fileList)
        {//empty list to store filenames to load is sent in

            //set up variables
            Int32 length;         //length of index
            Int32 i;                //counter

            //set up arrays
            string[] lines = { };

            //set up Lists
            List<String> mylistFiles = new List<String>();

            //needs to be set in parent class
            String indexpathSource = m_sourceFileName;

            //check for NULL.  Make sure this is set in parent class
            StreamReader streamReadFile;

            try
            {
                streamReadFile = new StreamReader(indexpathSource);
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n" + @"No source path included" + "\n");
                return;
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
                Console.WriteLine("File -->" + fileList[i] +"\n");
            }
            Console.WriteLine("\n");
        }
    }
}
