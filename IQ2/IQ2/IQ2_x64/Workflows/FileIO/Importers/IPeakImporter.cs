using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace IQ_X64.Workflows.FileIO.Importers
{
    public abstract class IPeakImporter
    {
        protected int numRecords;
        protected BackgroundWorker backgroundWorker;

        #region Constructors
        #endregion

        #region Properties
        #endregion

        #region Public Methods

        public abstract void ImportPeaks(List<Run64.Backend.DTO.MSPeakResult> peakList);

        #endregion

        protected virtual void reportProgress(int progressCounter)
        {
            if (numRecords == 0) return;
            if (progressCounter % 10000 == 0)
            {

                int percentProgress = (int)((double)progressCounter / (double)numRecords * 100);

                if (this.backgroundWorker != null)
                {
                    backgroundWorker.ReportProgress(percentProgress);
                }
                else
                {
                    if (progressCounter % 50000 == 0) Console.WriteLine("Peak importer progress (%) = " + percentProgress);

                }

                return;
            }
        }


        #region Private Methods
        #endregion
    }
}
