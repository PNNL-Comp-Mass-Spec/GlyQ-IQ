using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Synthetic_Spectra_DLL_X64_Net_4.Framework
{
    public class PeakGeneric<T,U> 
        where T : struct
        where U : struct
    {
        public T Mass { get; set; }
        public U Intensity { get; set; }

        public PeakGeneric(T mass, U intensity)
        {
            this.Mass = mass;
            this.Intensity = intensity;
        }

        public PeakGeneric()
        { }

    }

    public class PeakDecimal
    {
        public decimal Mass { get; set; }
        public decimal Intensity { get; set; }

        public PeakDecimal(decimal mass, decimal intensity)
        {
            this.Mass = mass;
            this.Intensity = intensity;
        }

        public PeakDecimal()
        { }
    }

    public class PeakTopGeneric<T,U> 
        where T : struct 
        where U :struct     
	{
        public T apexCenterMZ { get; set; }//calculated inside
        public U apexIntensity { get; set; }//calculated inside
        public T peakSpacing { get; set; }//input
        public T peakStartMZ { get; set; }//input
        public int peakIndex { get; set; }//input

        //List<double> m_ionMZ { get; set; }
        //List<double> m_ionIntensity { get; set; }

        public List<T> ionMZ = new List<T>();
        public List<U> ionIntensity = new List<U>();

        public void Add_MZ_Intensity(T MZ, U Intensity)
        {
            ionMZ.Add(MZ);
            ionIntensity.Add(Intensity);
        }

    }
}
