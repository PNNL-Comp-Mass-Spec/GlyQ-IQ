using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.PNNLOmics_Modules;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using GetPeaks_DLL.Objects;

namespace GetPeaks_DLL
{
    [Serializable]
    public class SimpleWorkflowParameters:IDisposable
    {
        #region Constructors

            public SimpleWorkflowParameters()
            {
                this.FileNameUsed = "Unknown";
                this.Part1Parameters = new ElutingPeakFinderParametersPart1();
                this.Part2Parameters = new ElutingPeakFinderParametersPart2();
                
                this.ConsistancyCrossErrorPPM = 15;//ppm
                this.ConsistancyCrossErrorCorrectedMass = 17;//ppm
                this.SummingMethod = ScanSumSelectSwitch.MaxScan;
               }

        #endregion

        #region Properties
            public string FileNameUsed {get;set;}

            public ElutingPeakFinderParametersPart1 Part1Parameters { get; set; }
            
            public ElutingPeakFinderParametersPart2 Part2Parameters { get; set; }
           
            /// <summary>
            /// when we are checking to see if the eluting peak yields a monoisotopic peak with the same mass.
            /// If we are witin the error, the eluting peak is a monoisotopic mass.  otherwise, the eluting peak is likely an isotope
            /// </summary>
            public double ConsistancyCrossErrorPPM { get; set; }

            /// <summary>
            /// allowed deviation of returned monoisotopic mass from summed spectra from the weighted corrected mass
            /// </summary>
            public double ConsistancyCrossErrorCorrectedMass { get; set; }

            /// <summary>
            /// this will switch betweem summing the scans or selecting the max value.  This is mainly for part 2 prior to deisotoping
            /// </summary>
            public ScanSumSelectSwitch SummingMethod { get; set; }

        #endregion

            #region IDisposable Members

            public void Dispose()
            {
                this.FileNameUsed = null;
                this.Part2Parameters.MemoryDivider = null;
                this.Part1Parameters.ParametersOrbitrap.Dispose();
                this.Part2Parameters.ParametersOrbitrap.Dispose();
                this.Part1Parameters.ParametersOrbitrap = null;
                this.Part2Parameters.ParametersOrbitrap = null;
                this.Part1Parameters = null;
                this.Part2Parameters = null;
            }

            #endregion
    }

    public enum DeconvolutionType
    { 
        Thrash,
        Rapid
    }

    public static class ObjectCopier
    {     
        /// <summary>     
        /// Perform a deep Copy of the object.     
        /// </summary>     
        /// <typeparam name="T">The type of object being copied.</typeparam>     
        /// <param name="source">The object instance to copy.</param>     
        /// <returns>The copied object.</returns>     
        public static T Clone<T>(T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", "source");  
            }          
            
            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))         
            {             
                return default(T);         
            }          
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)         
            {             
                formatter.Serialize(stream, source);             
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);         
            }     
        } 
    }    
}
