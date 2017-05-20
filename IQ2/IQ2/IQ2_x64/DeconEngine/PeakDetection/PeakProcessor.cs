using System;
using System.Collections.Generic;

namespace IQ_X64.DeconEngine.PeakDetection
{
    public class PeakProcessor
    {
		public PeakIndex mobj_pk_index  { get; set; } 
		//! tolerance width used while searching in attention list in the peaks. 
		public double mdbl_mz_tolerance_attention_list  { get; set; } 
		//! flag to check attention list for peaks that we want to necessarily look for.
		public bool mbln_chk_attention_list  { get; set; } 
		//! map of attention list of m/z's. 
		/*!
			\remark This was implemented as a map because it would allow for direct sorted lookup for an m/z value.
		*/
		
		public Dictionary<double, int> mmap_attention_list  { get; set; } 
		


		//! background intensity. When user sets min signal to noise, and min intensity, 
		// this value is set as min intensity / min signal to noise. 
		public double mdbl_background_intensity  { get; set; } 

		//! minimum intensity for a point to be considered a peak. 
		public double mdbl_peak_intensity_threshold  { get; set; }
		//! signal to noise threshold for a Peak to be considered as a peak.
		public double mdbl_signal_2_noise_threshold  { get; set; } 
		//! fit type used in the PeakFit class to detect peaks in the raw data object. 
        public PEAK_FIT_TYPE menm_peak_fit_type { get; set; }

        public PEAK_PROFILE_TYPE menm_profile_type { get; set; }

        public PeakData mobj_peak_data { get; set; }

        //! Is the data centroided ? 
        public bool mbln_centroided_data  { get; set; } 

        // if the data is thresholded, the ratio is taken as the ratio to background intensity.
       public bool mbln_thresholded_data { get; set; }  

        public PeakStatistician mobj_peak_statistician { get; set; }  

        PeakFit mobj_peak_fit ; 

        public PeakProcessor()
		{
            PEAK_FIT_TYPE defaultPeakFitType= PEAK_FIT_TYPE.QUADRATIC;
            mdbl_mz_tolerance_attention_list = 5.0 ; 
			mbln_chk_attention_list = false ;
            SetPeakFitType(defaultPeakFitType); 
			mobj_peak_data = new PeakData() ; 
			mbln_centroided_data = false ; 
			menm_profile_type = PEAK_PROFILE_TYPE.PROFILE ; 
            mobj_peak_statistician = new PeakStatistician();

            mobj_pk_index = new PeakIndex();
            mobj_peak_fit = new PeakFit(defaultPeakFitType);
		}

        public void PeakFitSetOptions(PEAK_FIT_TYPE type)
		{
			menm_peak_fit_type = type ; 
		}

       //PeakProcessor::~PeakProcessor()
        //{
        //    delete mobj_peak_data ; 
        //}

        //void LoadAttentionList(char file_name)
        //{
        //    const int BUFFER_SIZE = 512 ;
        //    std::fstream input_file(file_name, std::ios::binary) ; 

        //    char tolerance_str [BUFFER_SIZE] ;
        //    mmap_attention_list = new Dictionary<double, int>(); 

        //    while (!input_file.eof())
        //    {
        //        input_file.getline(tolerance_str, BUFFER_SIZE, '\n') ; 
        //        char *ptr = strstr(tolerance_str, "Tolerance=") ; 
        //        if (ptr != null)
        //        {
        //            double tolernce = atof(ptr+1) ; 
        //            mdbl_mz_tolerance_attention_list = tolernce ;
        //        }
        //        else
        //        {
        //            double mz_val_attention_list ; 
        //            mz_val_attention_list = atof(tolerance_str) ; 
        //            if (mz_val_attention_list != 0)
        //                mmap_attention_list.Add(mz_val_attention_list,1) ; 
        //        }
        //    }
        //    input_file.close() ; 
        //}




		void SetSignal2NoiseThreshold(double s2n)
		{
			mdbl_signal_2_noise_threshold = s2n ;
			if (mbln_thresholded_data)
			{
				if (mdbl_signal_2_noise_threshold != 0)
					mdbl_background_intensity = mdbl_peak_intensity_threshold / mdbl_signal_2_noise_threshold ;
				else
					mdbl_background_intensity = 1 ; 
			}
		}		
	
		void SetPeakIntensityThreshold(double threshold) 
		{
			mdbl_peak_intensity_threshold = threshold ; 
			if (mbln_thresholded_data)
			{
				if (mdbl_signal_2_noise_threshold != 0)
					mdbl_background_intensity = threshold / mdbl_signal_2_noise_threshold ;
				else if (threshold != 0)
					mdbl_background_intensity = threshold ; 
				else
					mdbl_background_intensity = 1 ; 
			}
		}
		void SetAttentionListTolerance(double mz_tol)
		{
			mdbl_mz_tolerance_attention_list = mz_tol ; 
		}

		void SetPeakFitType(PEAK_FIT_TYPE type)
		{
			menm_peak_fit_type = type ; 
			//mobj_peak_fit.SetOptions(type) ; 
            PeakFitSetOptions(type);
		}

		void SetPeaksProfileType(bool profile)
		{
			if(profile)
			{
			    menm_profile_type = PEAK_PROFILE_TYPE.PROFILE;
			}
			else
			{
			    menm_profile_type = PEAK_PROFILE_TYPE.CENTROIDED;
			}
		}

		void SetCheckAttentionList(bool set)
		{
			mbln_chk_attention_list = set ;
		}

		void SetOptions(double s2n, double thresh, bool thresholded, PEAK_FIT_TYPE type)
		{
			mbln_thresholded_data = thresholded ; 			
			// signal to noise should ideally be set before PeakIntensityThreshold
			SetSignal2NoiseThreshold(s2n) ; 
			SetPeakIntensityThreshold(thresh) ; 
			SetPeakFitType(type) ; 			
		}


		public int DiscoverPeaks (List<double> vect_mz, List<double> vect_intensity, double start_mz, double stop_mz) 
		{

			if(vect_intensity.Count < 1)
				return 0 ; 

			mobj_peak_data = new PeakData();//Clear 
			int num_data_pts = vect_intensity.Count ; 
			int ilow ; 
			int ihigh ; 

			

		    int peaksDetectedCount = 0;

			int start_index = mobj_pk_index.GetNearestBinary(ref vect_mz, start_mz, 0, num_data_pts-1) ; 
			int stop_index = mobj_pk_index.GetNearestBinary(ref vect_mz, stop_mz, start_index, num_data_pts-1) ; 
			if (start_index <= 0)
				start_index = 1 ; 
			if (stop_index >= vect_mz.Count -2)
				stop_index = vect_mz.Count - 2 ; 

			for (int index = start_index ; index <= stop_index ; index++)
			{
				double FWHM = -1 ;
				double current_intensity = vect_intensity[index] ; 
				double last_intensity = vect_intensity[index-1] ; 
				double next_intensity = vect_intensity[index+1]  ; 
				double current_mz = vect_mz[index] ;

                Peak2 peak = new Peak2();

				if (menm_profile_type == PEAK_PROFILE_TYPE.CENTROIDED)
				{
					if (current_intensity >= mdbl_peak_intensity_threshold)
					{
						double mass_ = (vect_mz)[index] ; 
						double SN = current_intensity / mdbl_peak_intensity_threshold ; 
						FWHM = 0.6 ; 
						//peak.Set(mass_, current_intensity, SN, mobj_peak_data->GetNumPeaks(), index, FWHM) ;	
                        int peakIndexToFigureOut = peaksDetectedCount;
                        peak.Set(mass_, current_intensity, SN, peakIndexToFigureOut, index, FWHM) ;	
						
                        //mobj_peak_data->AddPeak(peak) ;
                        mobj_peak_data.mvect_peak_tops.Add(peak);
                        peaksDetectedCount++;
					}
				}	
				else if (menm_profile_type == PEAK_PROFILE_TYPE.PROFILE)
				{
					 if(current_intensity > last_intensity && current_intensity >= next_intensity
							&& current_intensity >= mdbl_peak_intensity_threshold)
					{
						//we are sitting on an apex right here
					     
                         //See if the peak meets the conditions.
						//The peak data will be found at _transformData->begin()+i+1.
						double SN = 0 ; 
						
						if (!mbln_thresholded_data)
							SN = mobj_peak_statistician.FindSignalToNoise(current_intensity, ref vect_intensity, index);
						else
							SN = current_intensity / mdbl_background_intensity ; 

						// Run Full-Width Half-Max algorithm to try and squeak out a higher SN
						if(SN < mdbl_signal_2_noise_threshold)
						{
							double mass_ = (vect_mz)[index] ; 
							FWHM = mobj_peak_statistician.FindFWHM(ref vect_mz, ref vect_intensity, index, SN);
							if(FWHM > 0 && FWHM < 0.5)
							{
								ilow = mobj_pk_index.GetNearestBinary(ref vect_mz, current_mz  - FWHM, 0, index);
								ihigh = mobj_pk_index.GetNearestBinary(ref vect_mz, current_mz + FWHM, index, stop_index);

								double low_intensity = vect_intensity[ilow] ; 
								double high_intensity = vect_intensity[ihigh] ;

								double sum_intensity = low_intensity + high_intensity; 
								if(sum_intensity > 0)
									SN = (2.0 * current_intensity) / sum_intensity ;
								else
									SN = 10;
							}
						}
						// Found a peak, make sure it is in the attention list, if there is one.
                        //if(SN >= mdbl_signal_2_noise_threshold && ( !mbln_chk_attention_list || IsInAttentionList(current_mz))) 
						if(SN >= mdbl_signal_2_noise_threshold && ( !mbln_chk_attention_list)) 
						{
							// Find a more accurate m/z location of the peak.
							double fittedPeak = mobj_peak_fit.Fit(index, ref vect_mz, ref vect_intensity); 
							if (FWHM == -1)
							{
								FWHM = mobj_peak_statistician.FindFWHM(ref vect_mz, ref vect_intensity, index, SN);
							}

							if (FWHM > 0)
							{
                                int peakIndexToFigureOut = Convert.ToInt32(peaksDetectedCount);//mobj_peak_data->GetNumPeaks()
                                peak.Set(fittedPeak, current_intensity, SN, peakIndexToFigureOut, index, FWHM) ;	
								mobj_peak_data.mvect_peak_tops.Add(peak); //mobj_peak_data->AddPeak(peak) ; 
                                peaksDetectedCount++;
							}
							// move beyond peaks have the same intensity.
							bool incremented = false ; 
							while( index < num_data_pts && vect_intensity[index] == current_intensity)
							{
								incremented = true ; 
								index++ ;
							}
							if(index > 0 && index < num_data_pts 
								&& incremented)
								index-- ; 
						}
					}
				}
			}
			mobj_peak_data.mptr_vect_mzs = vect_mz ; 
			mobj_peak_data.mptr_vect_intensities = vect_intensity ; 

			return mobj_peak_data.mvect_peak_tops.Count;//mobj_peak_data->GetNumPeaks() ; 
		}

		double GetClosestPeakMz(double peakMz, ref Peak2 Peak)
		{
			//looks through the peak list and finds the closest peak to peakMz
			double min_score  = 1.00727638; //enough for one charge away
			Peak2 finalPeak = new Peak2(); 
			Peak2 thisPeak = new Peak2();
			finalPeak.mdbl_mz = 0.0 ; 
			double score;			
         
			try
			{
				int numberPeaks = mobj_peak_data.mvect_peak_tops.Count;

				for (int peakCount = 0; peakCount < numberPeaks; peakCount++)
				{
					thisPeak = mobj_peak_data.mvect_peak_tops[peakCount];
					score = Math.Pow((peakMz - thisPeak.mdbl_mz),2);// pow((peakMz - thisPeak.mdbl_mz), 2);
					if (score < min_score)
					{
						min_score = score;
						finalPeak = mobj_peak_data.mvect_peak_tops[peakCount];
					}
				}			
				Peak = finalPeak;
			}
			catch(SystemException mesg) 
			{
				finalPeak.mdbl_mz = 0.0 ; 
				Peak.mdbl_mz = 0.0 ; 
				Peak.mdbl_intensity = 0.0; 
			}
			
			return finalPeak.mdbl_mz;
		}

        
		int DiscoverPeaks (List<double> vect_mz, List<double> vect_intensity) 
		{
			if (vect_mz.Count == 0)
				return 0 ; 
			double min_mz = vect_mz[0];//->at(0) ; 
			int num_elements = vect_mz.Count-1;//->size()-1 ; 
			double max_mz = vect_mz[num_elements];//->at(num_elements) ; 
			return DiscoverPeaks (vect_mz, vect_intensity, min_mz, max_mz) ;  
		}

		bool DiscoverPeak(ref List<double> vect_mz, ref List<double> vect_intensity, double start_mz, double stop_mz, ref Peak2 pk, bool find_fwhm = false, bool find_sn =false, bool fit_peak = false )
		{
			int start_index = mobj_pk_index.GetNearest(ref vect_mz, start_mz, 0) ; 
			int stop_index = mobj_pk_index.GetNearest(ref vect_mz, stop_mz, start_index) ; 

			pk.mdbl_mz = 0 ; 
			pk.mdbl_intensity = 0 ; 
			pk.mint_data_index = -1 ; 
			pk.mdbl_FWHM = 0 ; 
			pk.mdbl_SN = 0 ;

			double max_intensity = 0 ; 
			bool found = false ; 
			for (int i = start_index ; i < stop_index ; i++)
			{
				double intensity = vect_intensity[i] ; 
				if (intensity > max_intensity)
				{
					max_intensity = intensity ; 
					pk.mdbl_mz = vect_mz[i] ; 
					pk.mdbl_intensity = intensity ; 
					pk.mint_data_index = i ; 
					found = true ; 
				}
			}
			if (found)
			{
				if (find_fwhm)
					pk.mdbl_FWHM = mobj_peak_statistician.FindFWHM(ref vect_mz, ref vect_intensity, pk.mint_data_index) ; 
				if (find_sn)
					pk.mdbl_SN = mobj_peak_statistician.FindSignalToNoise(pk.mdbl_intensity, ref vect_intensity, pk.mint_data_index) ; 
				if (fit_peak)
					pk.mdbl_mz = mobj_peak_fit.Fit(pk.mint_data_index, ref vect_mz, ref vect_intensity); 

			}
			return found ; 
		}

        //bool PeakProcessor::IsInAttentionList(double mz_val)
        //{
        //    if (mmap_attention_list.empty())
        //        return false ; 
        //    std::map<double, int>::iterator closest_iterator = mmap_attention_list.lower_bound(mz_val - mdbl_mz_tolerance_attention_list) ; 
        //    if ((*closest_iterator).first <= mz_val + mdbl_mz_tolerance_attention_list)
        //        return true ; 
        //    return false ; 
        //}


		void Clear()
		{
			mmap_attention_list = new Dictionary<double, int>() ; 
			mobj_peak_data.Clear() ; 
		}
		public void FilterPeakList(double tolerance)
		{
			mobj_peak_data.FilterList(tolerance) ; 
		}
		double GetFWHM (ref List<double> vect_mz, ref List<double> vect_intensity, int data_index, double SN)
		{
			return mobj_peak_statistician.FindFWHM(ref vect_mz, ref vect_intensity, data_index, SN) ; 
		}
		double GetFWHM (ref List<double> vect_mz, ref List<double> vect_intensity, double peak)
		{
			int index = mobj_pk_index.GetNearest(ref vect_mz, peak, 0) ;  
			return mobj_peak_statistician.FindFWHM(ref vect_mz, ref vect_intensity, index) ; 
		}

		double GetSN (ref List<double> vect_intensity, int data_index)
		{
			return mobj_peak_statistician.FindSignalToNoise(vect_intensity[data_index], ref vect_intensity, data_index) ; 
		}
		double GetSN (ref List<double> vect_mz, ref List<double> vect_intensity, double peak)
		{
			int index = mobj_pk_index.GetNearest(ref vect_mz, peak, 0) ;  
			return mobj_peak_statistician.FindSignalToNoise(vect_intensity[index], ref vect_intensity, index) ; 
		}

	}
}
