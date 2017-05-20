using System;
using System.Collections.Generic;

namespace GetPeaks_DLL.Parallel
{
    public abstract class ParalellEngine
    {
        /// <summary>
        /// is the transformer being used
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// lock for inside each engine
        /// </summary>
        public Object EngineLock {get;set;}//global lock for database

        /// <summary>
        /// lock for SQLite writing or other such things
        /// </summary>
        public Object DatabaseLock { get; set; }

        /// <summary>
        /// case log.  when if fails, record here
        /// </summary>
        public List<string> ErrorLog { get; set; }

        //this is a problem because it uses decon tools
        /// <summary>
        /// for summing data
        /// </summary>
        public DeconTools.Backend.Core.ScanSet CurrentScanSet { get; set; }

        /// <summary>
        /// abstracted place to hold parameters
        /// </summary>
        public ParalellParameters Parameters { get; set; }

        public int Scan { get; set; }

        public int EngineNumber { get; set; }

        public ParalellEngine()
        {
            this.Active = false;
            this.DatabaseLock = new Object();
            this.ErrorLog = new List<string>();
            this.ErrorLog.Add("start (scan offset of +1)");
        }

        public abstract ParalellEngineStation SetupEngines(ParalellParameters parameters);

    }
}
