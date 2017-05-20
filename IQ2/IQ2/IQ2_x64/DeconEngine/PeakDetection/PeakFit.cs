using System.Collections.Generic;

namespace IQ_X64.DeconEngine.PeakDetection
{
    public class PeakFit
    {
        private PEAK_FIT_TYPE menm_peak_fit_type { get; set; }

        private PeakStatistician mobj_peak_statistician { get; set; }

        private PeakIndex mobj_peak_index { get; set; }
    

        public PeakFit()
		{
            menm_peak_fit_type = PEAK_FIT_TYPE.QUADRATIC;
            mobj_peak_statistician = new PeakStatistician();
		}

        public PeakFit(PEAK_FIT_TYPE type)
        {
            mobj_peak_statistician = new PeakStatistician();
            SetOptions(type);
        }

        public void SetOptions(PEAK_FIT_TYPE type)
		{
			menm_peak_fit_type = type ; 
		}

		public double Fit(int index, ref List<double> mzs, ref List<double>intensities)
		{
			switch(menm_peak_fit_type)
			{
			    case PEAK_FIT_TYPE.APEX:
			        {
			            return mzs[index];
			        }
			        break;
			    case PEAK_FIT_TYPE.QUADRATIC:
			        {
			            return QuadraticFit(ref mzs, ref intensities, index);
			        }
			        break;
			    case PEAK_FIT_TYPE.LORENTZIAN:
			        {
			            double FWHM = mobj_peak_statistician.FindFWHM(ref mzs, ref intensities, index);
			            if (FWHM != 0)
			            {
			                return LorentzianFit(ref mzs, ref intensities, index, FWHM);
			            }
			            else
			            {
			                return mzs[index];
			            }
			        }
			        break;
			    default:
			        {
			            return 0.0;
			        }
			        break;
			}
        }

		double QuadraticFit(ref List<double>mzs, ref List<double> intensities, int index)
		{
			double x1,x2,x3;
			double y1,y2,y3;
			double d;

			if (index <1)
				return mzs[0] ; 
			if (index >= mzs.Count-1)
				return mzs[mzs.Count-1] ; 
               // return mzs.back() ; 

			x1 = mzs[index-1];
			x2 = mzs[index];
			x3 = mzs[index+1];
			y1 = intensities[index-1];
			y2 = intensities[index];
			y3 = intensities[index+1];

			d = (y2-y1)*(x3-x2) - (y3-y2)*(x2-x1);
			if(d==0)
				return x2;  // no good.  Just return the known peak
			d = ((x1 + x2) - ((y2 - y1) * (x3 - x2) * (x1 - x3)) / d) / 2.0;
			return d;	// Calculated new peak.  Return it.
		}

		double LorentzianFit(ref List<double>mzs, ref List<double>intensities, int index, double FWHM)
		{
			double A = intensities[index] ; 
			double Vo = mzs[index] ; 
			int lstart, lstop ; 

			double E, CE, le ; 

			E = (Vo - mzs[index+1])/100 ; 
			E = PeakProcessorHelpers.absolute(E) ; 

			if (index <1)
				return mzs[index] ; 
			if (index == mzs.Count)
				return mzs[index] ; 

			lstart = mobj_peak_index.GetNearest(ref mzs, Vo + FWHM, index) + 1 ; 
			lstop = mobj_peak_index.GetNearest(ref mzs, Vo - FWHM, index) - 1 ; 

			CE = LorentzianLS(ref mzs, ref intensities, A, FWHM, Vo, lstart, lstop) ; 
			for (int i = 0 ; i < 50 ; i++)
			{
				le = CE ;
				Vo = Vo + E ; 
				CE = LorentzianLS(ref mzs, ref intensities, A, FWHM, Vo, lstart, lstop) ;
				if (CE > le) 
					break ; 
			}

			Vo = Vo - E ; 
			CE = LorentzianLS(ref mzs, ref intensities, A, FWHM, Vo, lstart, lstop) ; 
			for (int i = 0 ; i < 50 ; i++)
			{
				le = CE ;
				Vo = Vo - E ; 
				CE = LorentzianLS(ref mzs, ref intensities, A, FWHM, Vo, lstart, lstop) ; 
				if (CE > le) 
					break ; 
			}
			Vo = Vo + E ; 
			return Vo ; 
		}

		double LorentzianLS(ref List<double> mzs, ref List<double> intensities, double A, double FWHM, double Vo, int lstart, int lstop) 
		{
			double u ; 
			double Y1, Y2 ;
			double RMSerror = 0 ; 

			for (int index = lstart ; index <= lstop ; index++)
			{
				u = 2 / FWHM * (mzs[index]-Vo) ; 
				Y1 = (int) (A / (1 + u * u)) ;
				Y2 = intensities[index] ; 
				RMSerror = RMSerror + (Y1-Y2)*(Y1-Y2) ; 
			}
			return RMSerror ;
		}
	}
}