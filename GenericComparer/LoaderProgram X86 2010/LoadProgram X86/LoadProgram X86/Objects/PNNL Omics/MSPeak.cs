using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    public class MSPeak: Peak
    {
        private int m_scanNumber;

        public int ScanNumber
        {
            get { return m_scanNumber; }
            set { m_scanNumber = value; }
        }
        /// <summary>
        /// Gets
        /// </summary>
        public double MZ
        {
            get
            {
                return XValue;
            }
            set
            {
                XValue = value;
            }
        }

        /// <summary>
        /// Gets or sets the intensity of the peak
        /// </summary>
        public int Intensity
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

    }
}
