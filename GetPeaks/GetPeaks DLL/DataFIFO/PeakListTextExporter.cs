using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DeconTools.Backend;
using DeconTools.Backend.DTO;
using DeconTools.Backend.Utilities;
using DeconTools.Utilities;
using PNNLOmics.Data;

namespace GetPeaks_DLL.DataFIFO
{
    public class PeakListTextExporter : IPeakListExporter
    {
        private char m_delimiter;
        private Globals.MSFileType m_FileType;
        private int[] m_msLevelsToExport;
        private int m_triggerValue;


        #region Constructors
        public PeakListTextExporter(Globals.MSFileType fileType, string outputFileName)
            : this(fileType, 100000, outputFileName)      // default allowed MSLevels = 1  
        {
        }


        public PeakListTextExporter(Globals.MSFileType fileType, int triggerValue, string outputFileName)
        {

            this.FileName = outputFileName;
            this.TriggerToWriteValue = triggerValue;      //will write out peaks if trigger value is reached
            this.m_delimiter = '\t';
            this.m_FileType = fileType;    // need to know filetype so that frame_num can be outputted for UIMF data


            bool fileExists;
            try
            {
                fileExists = File.Exists(outputFileName);
            }
            catch (Exception ex)
            {
                Logger.Instance.AddEntry("PeakExporter failed. Details: " + ex.Message, Logger.Instance.OutputFilename);
                throw new Exception("Peak exporter failed.  Check to see if output file is already open.");
            }


            if (fileExists)
            {
                try
                {
                    File.Delete(outputFileName);
                }
                catch (Exception ex)
                {
                    Logger.Instance.AddEntry("PeakExporter failed. Details: " + ex.Message, Logger.Instance.OutputFilename);
                    throw new Exception("Peak exporter failed.  Check to see if output file is already open.");

                }
            }

            initializeAndWriteHeader();
        }



        #endregion

        #region Properties

        public string FileName { get; set; }

        public override int[] MSLevelsToExport
        {
            get { return m_msLevelsToExport; }
            set { m_msLevelsToExport = value; }
        }

        public override string Name { get; set; }

        public override int TriggerToWriteValue
        {
            get { return m_triggerValue; }
            set { m_triggerValue = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Append the peaks to the _peaks.txt file
        /// </summary>
        /// <param name="peakList">Peak list to write</param>
        public override void WriteOutPeaksOmics(List<ProcessedPeak> peakList, List<int> ids)
        {
            using (var sw = new StreamWriter(new FileStream(this.FileName, FileMode.Append, FileAccess.Write, FileShare.Read)))
            {
                WriteOutPeaksOmics(sw, peakList, ids);
                sw.Close();
            }
        }

        /// <summary>
        /// Append the peaks to the _peaks.txt file
        /// </summary>
        /// <param name="sw">Filestream object</param>
        /// <param name="peakList">Peak list to write</param>

        public override void WriteOutPeaksOmics(StreamWriter sw, List<ProcessedPeak> peakList, List<int> ids)
        {
            for(int i=0;i<peakList.Count;i++)

            {
                ProcessedPeak peak = peakList[i];
                int id = ids[i];
                string lineOfPeakData = buildPeakStringOmics(peak, id);
                sw.Write(lineOfPeakData);
            }
            sw.Flush();

        }

        public override void Cleanup()
        {
            base.Cleanup();
        }


        # endregion

        #region Private Methods
        private string buildPeakString(MSPeakResult peak)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(peak.PeakID);
            sb.Append(m_delimiter);

            if (this.m_FileType == Globals.MSFileType.PNNL_UIMF)
            {

                //TODO:  do we want to export 'frame_index' or 'frameNum' ??   arggh   I don't want to look it up everytime I write the peak!
                sb.Append(peak.FrameNum);
                sb.Append(m_delimiter);
            }
            else
            {

            }
            sb.Append(peak.Scan_num);
            sb.Append(m_delimiter);
            sb.Append(peak.MSPeak.XValue.ToString("0.#####"));
            sb.Append(m_delimiter);
            sb.Append(peak.MSPeak.Height);
            sb.Append(m_delimiter);
            sb.Append(peak.MSPeak.Width.ToString("0.####"));
            sb.Append(m_delimiter);
            sb.Append(peak.MSPeak.SignalToNoise.ToString("0.##"));
            sb.Append(m_delimiter);
            sb.Append(peak.MSPeak.MSFeatureID);
            sb.Append(Environment.NewLine);


            return sb.ToString();
        }

        private string buildPeakStringOmics(ProcessedPeak peak, int id)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(id);
            sb.Append(m_delimiter);

            if (this.m_FileType == Globals.MSFileType.PNNL_UIMF)
            {

                //TODO:  do we want to export 'frame_index' or 'frameNum' ??   arggh   I don't want to look it up everytime I write the peak!
                sb.Append(0);
                sb.Append(m_delimiter);
            }
            else
            {

            }
            sb.Append(peak.ScanNumber);
            sb.Append(m_delimiter);
            sb.Append(peak.XValue.ToString("0.#####"));
            sb.Append(m_delimiter);
            sb.Append(peak.Height);
            sb.Append(m_delimiter);
            sb.Append(peak.Width.ToString("0.####"));
            sb.Append(m_delimiter);
            sb.Append(peak.SignalToNoiseGlobal.ToString("0.##"));
            sb.Append(m_delimiter);
            sb.Append(peak.Charge);
            sb.Append(Environment.NewLine);


            return sb.ToString();
        }


        private string buildHeaderLine()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("peak_index");
            sb.Append(m_delimiter);
            if (this.m_FileType == Globals.MSFileType.PNNL_UIMF)
            {
                sb.Append("frame_num");
                sb.Append(m_delimiter);
            }
            sb.Append("scan_num");
            sb.Append(m_delimiter);
            sb.Append("mz");
            sb.Append(m_delimiter);
            sb.Append("intensity");
            sb.Append(m_delimiter);
            sb.Append("fwhm");
            sb.Append(m_delimiter);
            sb.Append("signal_noise");
            sb.Append(m_delimiter);
            sb.Append("MSFeatureID");
            sb.Append(Environment.NewLine);
            return sb.ToString();
        }
        protected void initializeAndWriteHeader()
        {

            Check.Assert(this.FileName != null && this.FileName.Length > 0, String.Format("{0} failed. Export file's FileName wasn't declared.", this.Name));

            try
            {
                if (File.Exists(this.FileName))
                {
                    File.Delete(this.FileName);
                }

            }
            catch (Exception ex)
            {
                Logger.Instance.AddEntry(String.Format("{0} failed. Details: " + ex.Message + "; STACKTRACE = " + ex.StackTrace, this.Name), Logger.Instance.OutputFilename);
                throw ex;
            }


            using (var writer = new StreamWriter(new FileStream(this.FileName, FileMode.Create, FileAccess.Write, FileShare.Read)))
            {
                string headerLine = buildHeaderLine();
                writer.Write(headerLine);
                writer.Flush();
                writer.Close();
            }
        }

        #endregion


    }
}
