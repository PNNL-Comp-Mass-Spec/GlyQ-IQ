namespace Parallel
{
    public static class ParalellLog
    {
        public static void LogPeakCount(ParalellEngine currentEngine, int scan, DeconToolsV2.Peaks.clsPeak[] mspeakList)
        {
            int scanOffest = ModifyScanNumber(scan);
            currentEngine.ErrorLog.Add("Scan " + scanOffest.ToString() + " has " + mspeakList.Length.ToString() + " peaks before transform");
        }

        public static void LogMass(ParalellEngine currentEngine, int scan, double mass)
        {
            int scanOffest = ModifyScanNumber(scan);
            currentEngine.ErrorLog.Add("Scan " + scanOffest.ToString() + " has " + mass.ToString());
        }

        public static void LogMonoCount(ParalellEngine currentEngine, int scan, DeconToolsV2.HornTransform.clsHornTransformResults[] transformResults2)
        {
            int scanOffest = ModifyScanNumber(scan);
            currentEngine.ErrorLog.Add("Scan " + scanOffest.ToString() + " has " + transformResults2.Length.ToString() + " monos added after transform");
        }

        public static void LogPeaks(ParalellEngine currentEngine, int scan, double mass)
        {
            int scanOffest = ModifyScanNumber(scan);
            currentEngine.ErrorLog.Add("Scan " + scanOffest.ToString() + " has " + mass.ToString());
        }

        public static void LogCentroidPeaks(ParalellEngine currentEngine, int scan, double mass, double intensity)
        {
            int scanOffest = ModifyScanNumber(scan);
            currentEngine.ErrorLog.Add("Scan " + scanOffest.ToString() + " has " + mass.ToString() + " " + intensity.ToString() + " prior to Deconvolution");
        }

        //public static void LogPeaksCentroid(ParalellEngine currentEngine, int scan, double mass)
        //{
        //    int scanOffest = ModifyScanNumber(scan);
        //    currentEngine.ErrorLog.Add("Scan " + scanOffest.ToString() + " has " + mass.ToString() + " centroid");
        //}

        public static void LogPeaksMZ(ParalellEngine currentEngine, int scan, double mass, int chargestate)
        {
            int scanOffest = ModifyScanNumber(scan);
            currentEngine.ErrorLog.Add("Scan " + scanOffest.ToString() + " has " + mass.ToString() + " MassToCharge at charge " + chargestate.ToString());
        }

        public static void LogAddScan(ParalellEngine currentEngine, int scan)
        {
            int scanOffest = ModifyScanNumber(scan);
            currentEngine.ErrorLog.Add("Scan " + scanOffest.ToString() + " has been added to Engine " + currentEngine.EngineNumber.ToString());
        }

        public static void LogAddEngine_ScanSet(ParalellEngine currentEngine)
        {
            currentEngine.ErrorLog.Add("A scanset has been added to Engine " + currentEngine.EngineNumber.ToString());
        }

        public static void LogAddEngine_Run(ParalellEngine currentEngine)
        {
            currentEngine.ErrorLog.Add("A run has been added to Engine " + currentEngine.EngineNumber.ToString());
        }

        public static void LogAddEngine_AddToStation(ParalellEngine currentEngine)
        {
            currentEngine.ErrorLog.Add("Engine number " + currentEngine.EngineNumber.ToString() + " has been added to the station");
        }

        public static void LogAddClusterMassDifference(ParalellEngine currentEngine, int scan, double massDifference, double massWindow, int iterationCuttoff, int iterationValue)
        {
            currentEngine.ErrorLog.Add("Scan " + scan.ToString() + " has " + massDifference.ToString() + " delta with a window of +-" + massWindow.ToString() + " iteration " + iterationValue + "/" + iterationCuttoff);
        }

        public static void LogAddText(ParalellEngine currentEngine, int scan, string comment)
        {
            currentEngine.ErrorLog.Add("Scan " + scan.ToString() + " has " + comment);
        }

        private static int ModifyScanNumber(int scan)
        {
            return (scan + 1);
        }
    }
}
