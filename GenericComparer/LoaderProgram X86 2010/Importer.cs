//Scott Kronewitter, 3-17-2010
//Loads a fixed index file containing several paths to spectra text files. 
//Classes:  Program.cs, Importer.cs, LoadTextFull.cs, LoadTextLine.cs

using System;
using System.Collections.Generic;   //List
using System.IO;                    //Stream Reader

namespace ConsoleApplication1
{
    public class Importer
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //Importer Class with out parameter
        public Importer()
        {
            Console.WriteLine("New Importer class created\n");
        }//end Importer

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //Importer Class with out parameter with index file name parameter
        public Importer(string filenamepath)
        {
            this.SourceFileName = filenamepath;
            Console.WriteLine("SetFile Path and Name:\n" + filenamepath);
        }//end Importer(file)

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //Source Filename property  //where loading inde file is located
        public string SourceFileName { get; set; }

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //load index file containing other file paths
        public void LoadFilesIndex(List<string> LinesToLoad)
        {//empty list to store filenames to load is sent in

            Console.WriteLine("LoadFilesIndex\n");
           
            //needs to be set in parent class
            String IndexpathSource = this.SourceFileName;

            //check for NULL.  Make sure this is set in parent class
            try
            {
                //Console.WriteLine("Try\n");
                //if (IndexpathSource == null)
                //{
                   StreamReader test = new StreamReader(IndexpathSource);
                   test.Close();
                //}
            }//end try

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message+"\n"+@"No source path included"+"\n");
                return; //we are missing information so exit class
            }//end catch

            finally
            {
                Console.WriteLine("Try/Catch End\n");
                //Console.ReadKey();  
            }//end finally

            //Create succesfull streamReader
            StreamReader streamReadFile = new StreamReader(IndexpathSource);

            //Read in entire file to fileContents
            string fileContents = streamReadFile.ReadToEnd();

            //close file
            streamReadFile.Close();

            //__________________________________________________________________________________
            //Convert loaded file into memory

            //set up variables
            Int32 m_length;         //length of index
            Int32 i;                //counter
            string m_myLine;

            //set up arrays
            string[] m_lines = { };

            //set up Lists
            List<String> mylistFiles = new List<String>();
            
            //__________________________________________________________________________________
            //parse file by lines
           
            m_lines = fileContents.Split(
                Environment.NewLine.ToCharArray(),  //'\n', '\r'
                StringSplitOptions.RemoveEmptyEntries);//does not include empty lines
                    
            //calculate number of lines in file
            m_length = m_lines.Length;
            Console.WriteLine("There are " + m_length + " lines total\n");

            //__________________________________________________________________________________
            //parse lines and store into list arrays
            for (i = 0; i < m_length; i += 1)
            {
                //removed extra \ from end
                m_myLine = m_lines[i].Remove(m_lines[i].Length - 1);  
                //add to list
                LinesToLoad.Add(m_myLine);
            }//end for

            //set new length
            m_length = LinesToLoad.Count;
            Console.WriteLine("There are now " + m_length + " lines\n");

            //print full file to console
            for (i = 0; i < m_length; i += 1)
            {
                Console.WriteLine("File --> {0}\n", LinesToLoad[i]);
            }//end for
            Console.WriteLine("\n");

        }//end LoadFilesIndex

    }//End Importer Class
}
