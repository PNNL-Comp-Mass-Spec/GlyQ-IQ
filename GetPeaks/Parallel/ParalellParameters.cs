using System;
using GetPeaks_DLL.Objects;

namespace Parallel
{
    public abstract class ParalellParameters: ICloneable
    {
        /// <summary>
        /// file information
        /// </summary>
        public InputOutputFileName FileInforamation { get; set; }

        public int ComputersToDivideOver { get; set; }
        public int CoresPerComputer { get; set; }

        //should we use multiple files
        public bool MultithreadedHardDriveMode { get; set; }

        public string UniqueFileName { get; set; }

        /// <summary>
        /// paralell ForEach or simple ForEach
        /// </summary>
        public bool MultithreadOperation { get; set; }

        /// <summary>
        /// max number of threads in for each multiplier
        /// </summary>
        public int ForEachCoreMultiplier { get; set; }

        public ParalellParameters()
        {
            FileInforamation = new InputOutputFileName();
            ComputersToDivideOver = 1;
            CoresPerComputer = 1;
            MultithreadOperation = false;
            ForEachCoreMultiplier = 1;
        }

        public ParalellParameters(int computersToDivideOver, int coresPerComputer, bool multiThreadOperation)
        {
            FileInforamation = new InputOutputFileName();
            ComputersToDivideOver = computersToDivideOver;
            CoresPerComputer = coresPerComputer;
            MultithreadOperation = multiThreadOperation;

        }

        #region ICloneable Members

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion
    }
}
