using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.Go_Decon_Modules;

namespace GetPeaks_DLL.Objects
{
    public class TransformerObject
    {
        /// <summary>
        /// is the transformer being used
        /// </summary>
        public bool active { get; set; }

        /// <summary>
        /// allows for a finite number of transformers to exist
        /// </summary>
        public DeconToolsV2.HornTransform.clsHornTransform TransformEngine {get;set;}



        /// <summary>
        /// unique filename to write to
        /// </summary>
        public string SQLiteTranformerFileName { get; set; }
        public string SQLiteTranformerFolderPath { get; set; }

        /// <summary>
        /// lock for SQLite writing
        /// </summary>
        public Object DatabaseLock {get;set;}

        /// <summary>
        /// case log.  when if fails, record here
        /// </summary>
        public List<string> ErrorLog { get; set; }


        public TransformerObject()
        {
            this.active = true;
            this.TransformEngine = new DeconToolsV2.HornTransform.clsHornTransform();
            this.SQLiteTranformerFileName = "TransformerSQL";
            this.SQLiteTranformerFolderPath = "SQLiteFolder";
            this.DatabaseLock = new Object();
            this.ErrorLog = new List<string>();
        }

        public TransformerObject(GoTransformParameters transformerParameterSetup, string SQLiteName, string SQLiteFolder, Object databaseLock)
        {
            this.active = false;
            this.TransformEngine = new DeconToolsV2.HornTransform.clsHornTransform();
            
            this.TransformEngine.TransformParameters = transformerParameterSetup.loadDeconEngineHornParameters(2);

            this.SQLiteTranformerFileName = SQLiteName;
            this.SQLiteTranformerFolderPath = SQLiteFolder;

            this.DatabaseLock = databaseLock;

            this.ErrorLog = new List<string>();
            this.ErrorLog.Add("start");
        }
    }
}
