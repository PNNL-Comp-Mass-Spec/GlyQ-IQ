using System.Collections.Generic;
using System;


namespace ConsoleApplication1
{
    public class XYData
    {
        private double m_x;
        private double m_y;

        public XYData()
        {
            m_x = 0;
            m_y = 0;
        }

        public XYData(double newX, double newY)
        {
            m_x = newX;
            m_y = newY;
        }

        public double X
        {
            get { return m_x; }
            set { m_x = value; }
        }

        public double Y
        {
            get { return m_y; }
            set { m_y = value; }
        }

    }
}