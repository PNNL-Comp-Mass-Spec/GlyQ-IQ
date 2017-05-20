//Scott Kronewitter, 3-17-2010
//Loads a fixed index file containing several paths to spectra text files. 
//Classes:  Program.cs, Importer.cs, LoadTextFull.cs, LoadTextLine.cs

using System;
using System.Collections.Generic;   //List
using System.IO;                    //Stream Reader

namespace ConsoleApplication1
{
    public class FileListImporter
    {
        private string m_sourceFileName;  //location of filelist containng a list of spectra

        #region Constructors

        public FileListImporter(string filenamepath, PrintBuffer print)
        {
            m_sourceFileName = filenamepath;
            //print.AddLine("SetFile Path and Name:\n" + filenamepath);
            print.AddLine("SetFile Path and Name:\n" + filenamepath);
        }

        #endregion

        #region Properties Empty

        #endregion


        #region Public Methods

        public void ImportFileList(List<string> fileList, PrintBuffer print)
        {//empty list to store filenames to load is sent in
            
            print.AddLine("LoadFilesList\n");
            
            print.AddLine("LoadFilesList\n");
            
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
                print.AddLine(ex.Message + "\n" + @"No source path included" + "\n");
                return;
            }

            string fileContents = streamReadFile.ReadToEnd();    //  Read in entire file to fileContents
            streamReadFile.Close();

            //parse lines and store into list arrays
            lines = fileContents.Split(
                Environment.NewLine.ToCharArray(),  //'\n', '\r'
                StringSplitOptions.RemoveEmptyEntries);//does not include empty lines

            length = lines.Length;
            print.AddLine("There are " + length + " lines total\n");

            for (i = 0; i < length; i++)
            {
                fileList.Add(lines[i]);
            }

            //set new length
            length = fileList.Count;
            print.AddLine("There are now " + length + " lines\n");

            //print full file to console
            for (i = 0; i < length; i++)
            {
                print.AddLine("File -->" + fileList[i] +"\n");
            }
            print.AddLine("\n");
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
