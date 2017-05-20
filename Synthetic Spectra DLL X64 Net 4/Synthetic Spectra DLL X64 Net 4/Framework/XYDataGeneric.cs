using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Synthetic_Spectra_DLL_X64_Net_4.Framework
{
    public class XYDataGeneric<T,U> 
        where T : struct
        where U : struct
    {
        private T m_x;
        private U m_y;

        //public XYDataGeneric()
        //{
        //    m_x = 0;
        //    m_y = 0;
        //}

        public XYDataGeneric(T newX, U newY)
        {
            m_x = newX;
            m_y = newY;
        }

        public T X
        {
            get { return m_x; }
            set { m_x = value; }
        }

        public U Y
        {
            get { return m_y; }
            set { m_y = value; }
        }
    }
}
