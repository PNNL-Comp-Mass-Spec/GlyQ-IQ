using System;
using System.Collections.Generic;
using System.IO;
using System.Data.Common;
using System.ComponentModel;
using Run64.Backend.Core;
using Run64.Utilities;


namespace IQ_X64.Workflows.FileIO.Importers
{
    public class PeakImporterFromSQLite : IPeakImporter
    {
        private string sqliteFilename;


        #region Constructors
        public PeakImporterFromSQLite(string sqliteFilename)
            : this(sqliteFilename, null)
        {

        }

        public PeakImporterFromSQLite(string sqliteFilename, BackgroundWorker bw)
        {
            this.backgroundWorker = bw;

            if (!File.Exists(sqliteFilename))
            {
				throw new System.IO.IOException("Peak import failed. File doesn't exist: " + DiagnosticUtilities.GetFullPathSafe(sqliteFilename));
            }
            this.sqliteFilename = sqliteFilename;
        }

        #endregion

        #region Properties
        #endregion

        #region Public Methods
        public override void ImportPeaks(List<Run64.Backend.DTO.MSPeakResult> peakList)
        {
            DbProviderFactory fact = DbProviderFactories.GetFactory("System.Data.SQLite");
            string queryString = "SELECT peak_id, scan_num, mz, intensity, fwhm FROM T_Peaks;";

            using (DbConnection cnn = fact.CreateConnection())
            {
                cnn.ConnectionString = "Data Source=" + this.sqliteFilename;
                try
                {
                    cnn.Open();
                }
                catch (Exception ex)
                {
                    throw new Exception("Peak import failed. Couldn't connect to SQLite database. \n\nDetails: " + ex.Message);
                }

                using (DbCommand command = cnn.CreateCommand())
                {
                    command.CommandText = "SELECT COUNT(*) FROM T_Peaks;";
                    numRecords = Convert.ToInt32(command.ExecuteScalar());
                }

                using (DbCommand command = cnn.CreateCommand())
                {
                    command.CommandText = queryString;
                    DbDataReader reader = command.ExecuteReader();


                    Run64.Backend.DTO.MSPeakResult peakresult;

                    int progressCounter = 0;
                    while (reader.Read())
                    {
                        peakresult = new Run64.Backend.DTO.MSPeakResult();

                        long test = (long)reader["peak_id"];

                        peakresult.PeakID = (int)(long)reader["peak_id"];
                        peakresult.Scan_num = (int)(long)reader["scan_num"];
                        peakresult.MSPeak = new MSPeak();
                        peakresult.MSPeak.XValue = (double)reader["mz"];
                        peakresult.MSPeak.Height = (float)(double)reader["intensity"];
                        peakresult.MSPeak.Width = (float)(double)reader["fwhm"];
                        peakList.Add(peakresult);

                        if (this.backgroundWorker != null)
                        {
                            if (backgroundWorker.CancellationPending)
                            {
                                return;
                            }
                        }

                        progressCounter++;
                        reportProgress(progressCounter);
                    }
                    reader.Close();
                }
            }
        }

 


        #endregion

        #region Private Methods
    

        #endregion



    }
}
