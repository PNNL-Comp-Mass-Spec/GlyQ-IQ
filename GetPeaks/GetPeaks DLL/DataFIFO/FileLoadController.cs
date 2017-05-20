using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.Objects;

namespace GetPeaks_DLL.DataFIFO
{
    public class FileLoadController
    {
        public List<DataSet> GetData(string fileName, GetPeaks_DLL.DataFIFO.FileIterator.deliminator fileDelimination )
        {

            #region loads a list of files to load

            FileListNameImporter newFileList = new FileListNameImporter(fileName);
            List<string> newFileNameList = newFileList.ImportFileList();

            #endregion

            #region iterate thhrough the list loading loads an XYData and an associated info file

            FileIterator newFileIterator = new FileIterator();
            List<DataSet> DataProject = newFileIterator.IterateFiles(newFileNameList, fileDelimination);

            #endregion
            return DataProject;
        }
    }
}
