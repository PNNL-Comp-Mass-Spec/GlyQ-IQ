using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;

namespace ConsoleApplication1
{
    public class FileIterator
    {
        #region Constructors
        #endregion

        #region Properties
        #endregion

        #region Public Methods

        //parameter1=file list with list of file paths
        //parameter2=a load method ("full" or "line")  load all at once or one line at a time
        public void IterateFiles(List<string> fileList, string selectedLoadMethod, List<DataSetSK> MZDataProject, PrintBuffer print)
		
        {
            print.AddLine("Iterate Now\n");

            //how many files to load
            int length=fileList.Count;

            //Instantiate classes so they are not inside the loop
            FileLoadXYTextFull loadSpectraF = null;
            FileLoadXYTextLine loadSpectraL = null;

            switch (selectedLoadMethod)
            {
                case "full":
                    loadSpectraF = new FileLoadXYTextFull();
                    break;
                case "line":
                    loadSpectraL = new FileLoadXYTextLine();
                    break;
                default:
                    print.AddLine("Unknown load method selected\n");
                    break;
            }
            
            for (int i = 0; i < length; i += 1)
            {
				print.AddLine("Load file: " + fileList[i] + "\n");
                DataSetSK newDataSet = new DataSetSK();
                MZDataProject.Add(newDataSet);

				List<XYData> DataMZ = new List<XYData>();
               
				List<SyntheticSpectraParameters> SynParameters = new List<SyntheticSpectraParameters>();
                //Load spectra
                switch (selectedLoadMethod)
                    {
                        case "full":
							loadSpectraF.SingleFileAtOnce(fileList[i], DataMZ, print);
                            break;
                        case "line":
                            loadSpectraL.SingleFileByLine(fileList[i], DataMZ, print);
                            break;
                        default:
                            print.AddLine("Unknown load method selected\n");
                            break;
                    }

                MZDataProject[i].AddSpectra(DataMZ);
                MZDataProject[i].DataSetName=fileList[i];
               

                //check spectra
                //if(DataMZ[0].Mass>=0)
                if(DataMZ[0].X>=0)
                {

                }
			}
        }
        
        #endregion

        #region Private Methods Empty

        
        #endregion
    }
}
