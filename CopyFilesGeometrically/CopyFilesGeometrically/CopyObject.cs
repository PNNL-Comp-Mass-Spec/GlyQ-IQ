using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CopyFilesGeometrically
{
    public class CopyObject
    {
        /// <summary>
        /// where data is located
        /// </summary>
        public string FolderLocation { get; set; }

        /// <summary>
        /// name of file without ending
        /// </summary>
        public string FileNameBase { get; set; }

        /// <summary>
        /// ending.  ".raw" or ".txt" etc
        /// </summary>
        public string FileNameEnding { get; set; }

        /// <summary>
        /// file to be copied
        /// </summary>
        public int MasterFileID { get; set; }

        /// <summary>
        /// This files ID
        /// </summary>
        public int ChildFileID { get; set; }
    }
}
