using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    public abstract class Peak: BaseData
    {
        /// <summary>
        /// Height of the peak.
        /// </summary>
        private int       m_height;
        /// <summary>
        /// Width of the peak.
        /// </summary>
        private double    m_width;
        /// <summary>
        /// X-value can be time, scan, m/z
        /// </summary>
        private double    m_xValue;

        /// <summary>
        /// Gets or the height of the peak.
        /// </summary>
        public virtual int Height
        {
            get { return m_height; }
            set { m_height = value; }
        }
        /// <summary>
        /// Gets or sets the width of the peak.
        /// </summary>
        /// <remarks>This is not always FWHM, but can be.</remarks>
        public virtual double Width
        {
            get { return m_width; }
            set { m_width = value; }
        }
        /// <summary>
        /// Gets or sets the X-Value of the peak (e.g. time, scan, m/z)
        /// </summary>        
        public virtual double XValue
        {
            get { return m_xValue; }
            set { m_xValue = value; }
        }

        public override void Clear()
        {
            throw new NotImplementedException();
        }
    }
}
