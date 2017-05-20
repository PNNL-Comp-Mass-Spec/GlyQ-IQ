using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using ConsoleApplication1;
using PNNLOmics.Data;

namespace LoadProgram.UnitTests
{

    public class SynSpectraCreatorTests
    {

        [Test]
        public void test1()//test file iterator
        {
            //create test fileslist
            List<string> fileslist = new List<String>();
            fileslist.Add(@"d:\Csharp\CSV Glycan Test3 10K.csv");
            fileslist.Add(@"d:\Csharp\CSV FTICR 9T.csv");          

            //create string
            string loadMethod = "line"; //load the file at once or line by line

            //Create print buffer
            PrintBuffer printer = new PrintBuffer();

            //New File iterator to load several mass spectra.  the first one is the data, the second one is the noise
            FileIterator iterateList = new FileIterator();

            //create date project to hold the data
            List<DataSetSK> DataProject = new List<DataSetSK>();  //a list of MZ data files

            //TODO:  write unit test for reading in peptide data 
            //loads the data from the files listed in fileList and appends them to the DataProject as XYDataLists
            iterateList.IterateFiles(fileslist, loadMethod, DataProject, printer);

            Assert.AreEqual(81, DataProject[0].XYList.Count);
            //Assert.AreEqual(211061, DataProject[1].XYList.Count);
        }
        

    }
}
//for names  detailBase
//ionMZ the ion is redundant
//unit test basic operations
//add more specific name to fileiterator if it does more than iterate files
//pull out synthetic parameters
//watch for the word feature
//create a parameter object
//get rid of underlines in names

// add aunit test with a class library
//add reference to NUnit.Framework