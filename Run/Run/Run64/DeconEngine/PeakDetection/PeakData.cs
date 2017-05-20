using System.Collections.Generic;
using System.Linq;

namespace Run64.DeconEngine.PeakDetection
{
    public class PeakData
    {
		/// <summary>
		/// std::vector of peaks found in the data. It is recommended that this object not be touched by calling functions. 
		/// </summary>
        public List<Peak2> mvect_peak_tops { get; set; }

        /// <summary>
        /// pointer to the std::vector of temporary peaks that are used during the processing
        /// </summary>
        public List<Peak2> mvect_temp_peak_tops { get; set; }

        /// <summary>
	    /// multimap of indices of unprocessed peaks in PeakData::mvect_temp_peak_tops sorted in ascending m/z. This helps fast retrieval when looking for an unprocessed peak around an approximate m/z.
	    /// the idea here is the indexes are faster for searches
        /// </summary>
	    public SortedDictionary<double, int> mmap_pk_mz_index { get; set; } 

        /// <summary>
        /// remarks While the intensity of the peaks might actually be double, for the map, we only used the integral values.
        /// </summary>
	    public SortedDictionary<int, int> mmap_pk_intensity_index { get; set; }  

        /// <summary>
		/// multimap of indices of all peaks in PeakData::mvect_temp_peak_tops sorted in ascending m/z. This helps fast retrieval when looking for a peak(not only unprocessed ones) around an approximate m/z.
		/// </summary>
        public SortedDictionary<double, int> mmap_pk_mz_index_all { get; set; }
        
        /// <summary>
        /// pointer to std::vector of mz's in the raw data
        /// </summary>
        public List<double> mptr_vect_mzs { get; set; }
    
        /// <summary>
        /// pointer to std::vector of intensities in the raw data.
        /// </summary>
        public List<double> mptr_vect_intensities { get; set; }

        
        

        public PeakData()
		{
            mvect_peak_tops = new List<Peak2>();
            mmap_pk_mz_index = new SortedDictionary<double, int>();
            mmap_pk_intensity_index = new SortedDictionary<int, int>();
            mmap_pk_mz_index_all = new SortedDictionary<double, int>();
		}

        //PeakData(ref PeakData a)
        //{
        //    mdbl_tic(a.mdbl_tic), mptr_vect_intensities(a.mptr_vect_intensities), 
        //    mptr_vect_mzs(a.mptr_vect_mzs)
        //{
        //    mvect_peak_tops.insert(mvect_peak_tops.begin(),a.mvect_peak_tops.begin(), a.mvect_peak_tops.end()) ;
        //}

		

		public void GetPeak(int index, ref Peak2 pk)
		{
			pk = mvect_peak_tops[index] ; 
		}

		public void AddPeak(ref Peak2 peak)
		{
			mvect_peak_tops.Add(peak); 
		}

		public void AddPeakToProcessingList(ref Peak2 pk) 
		{
			// The assumption is that this peak already exists in the std::vector.
			// The peak was removed from the processing list, so we're going to add it in.
			// The map for all peaks is unaffected so we won't add to it.
			// Also the intensity is set to 0 when deletion happens. So lets copy
			// the peak back into our peak vector.
			int peak_index = pk.mint_peak_index ; 
			double mz = pk.mdbl_mz ; 
			int intensity = (int)pk.mdbl_intensity ; 
			mvect_peak_tops[peak_index] = pk ; 
			mmap_pk_mz_index.Add(mz, peak_index) ; 
			mmap_pk_intensity_index.Add(intensity, peak_index) ; 
		}

		public void InitializeUnprocessedPeakData()
		{
            mmap_pk_mz_index = new SortedDictionary<double, int>();
            mmap_pk_mz_index_all = new SortedDictionary<double, int>(); 
			mmap_pk_intensity_index = new SortedDictionary<int, int>(); 


			int num_peaks = mvect_peak_tops.Count ; 
			for (int i = 0 ; i < num_peaks ; i++)
			{
				double mz = mvect_peak_tops[i].mdbl_mz ; 
				int intensity = (int) mvect_peak_tops[i].mdbl_intensity ;
				mvect_peak_tops[i].mint_peak_index = i ; 
				// some peaks might not need to be added to the list because they were already removed.
				// check the intensity to see if this might be the case
				if (intensity != -1)
				{
					mmap_pk_mz_index.Add(mz, i); 
					mmap_pk_intensity_index.Add(intensity, i) ; 
				}
				mmap_pk_mz_index_all.Add(mz, i) ; 
			}
		}

		void SortPeaksByIntensity()
		{
		    List<Peak2> sortedmvect_peak_tops = mvect_peak_tops.OrderBy(p => p.mdbl_intensity).ToList();

			for (int i = 0 ; i < mvect_peak_tops.Count ; i++)
			{
			    mvect_peak_tops[i].mint_peak_index = i;
			}
		}

		bool GetNextPeak(double start_mz, double stop_mz, ref Peak2 pk)
		{
			pk.mdbl_mz = -1 ; 
			pk.mdbl_intensity = -1 ; 

			//using namespace std ; 
			//for (multimap<int, int, greater<int> >::iterator iter = mmap_pk_intensity_index.begin() ; iter != mmap_pk_intensity_index.end() ; iter++)
			foreach (var iter in mmap_pk_intensity_index)
			{
				int peak_index = iter.Value;//).second ; 
				double mz = mvect_peak_tops[peak_index].mdbl_mz ;

				if (mz > start_mz && mz <= stop_mz)
				{
					pk = mvect_peak_tops[peak_index] ; 
					RemovePeak(ref pk) ; 
					return true ; 
				}
			}
			return false ; 
		}

        //void RemovePeak(ref Peak2 pk) 
        //{
        //    // oddly enough this wierd way of using namespace with a template works.
        //    std::multimap<int, int, std::greater<int> >::iterator iter_intensity = mmap_pk_intensity_index.find((int)pk.mdbl_intensity) ; 
        //    bool found = false ; 
        //    for ( ; iter_intensity != mmap_pk_intensity_index.end() ; iter_intensity++)
        //    {
        //        int peak_index = (*iter_intensity).second ; 
        //        if (peak_index == pk.mint_peak_index)
        //        {
        //            found = true ; 
        //            break ; 
        //        }
        //        if ((*iter_intensity).first < (int) pk.mdbl_intensity)
        //            return ; 
        //    }
        //    if (!found)
        //        return ; 

        //    mmap_pk_intensity_index.erase(iter_intensity) ; 

        //    found = false ; 
        //    std::map<double, int>::iterator iter_mz = mmap_pk_mz_index.find(pk.mdbl_mz) ; 
        //    for ( ; iter_mz != mmap_pk_mz_index.end() ; iter_mz++)
        //    {
        //        int peak_index = (*iter_mz).second ; 
        //        if (peak_index == pk.mint_peak_index)
        //        {
        //            found = true ; 
        //            break ; 
        //        }
        //        if (mvect_peak_tops[peak_index].mdbl_mz != pk.mdbl_mz)
        //            return ; 

        //    }
        //    if (!found)
        //    {
        //        // so how did this happen ? 
        //        int i = 0 ; 
        //        return ; 
        //    }

        //    mvect_peak_tops[pk.mint_peak_index].mdbl_intensity = -1 ; 
			
        //    mmap_pk_mz_index.erase(iter_mz) ; 

        //    return ; 
        //}


        //void RemovePeaks(double start_mz, double stop_mz, bool debug) 
        //{
        //    for (std::map<double, int>::iterator iter_mz = mmap_pk_mz_index.lower_bound(start_mz) ;  
        //            iter_mz != mmap_pk_mz_index.end() ; )
        //    {
        //        double mz = (*iter_mz).first ; 

        //        if (mz > stop_mz)
        //            return ; 

        //        int peak_index = (*iter_mz).second ; 

        //        if (mz < start_mz)
        //            continue ; 
        //        iter_mz++ ; // move iter_mz ahead because we're going to delete this peak.
        //        if (debug)
        //            std::cerr<<"\tRemoving peak"<<mvect_peak_tops[peak_index].mdbl_mz<<std::endl ; 
        //        RemovePeak(mvect_peak_tops[peak_index]) ; 
        //    }
        //    return ; 
        //}

        void RemovePeak(ref Peak2 pk)
        {
            Peak2 currentPeak = pk;
            double keyToRemove = mmap_pk_mz_index.Where(x => x.Key >= currentPeak.mdbl_mz && x.Key <= currentPeak.mdbl_mz).Select(x => x.Key).FirstOrDefault();

            mmap_pk_mz_index.Remove(keyToRemove);
            
            mvect_peak_tops.Remove(pk);
        }

        void RemovePeaks(double start_mz, double stop_mz, bool debug) 
		{
            List<double> keysToRemove = mmap_pk_mz_index.Where(x => x.Key >= start_mz && x.Key <= stop_mz).Select(x => x.Key).ToList();

            foreach (var key in keysToRemove)
            {
                mmap_pk_mz_index.Remove(key);
            }
		}

		public void RemovePeaks(ref List<double> peak_mzs, double mz_tolerance, bool debug)
		{
			// in order to remove background peaks and calibration peaks use this function.
			int num_peaks = peak_mzs.Count ; 
			for (int i = 0 ; i < num_peaks ; i++)
				RemovePeaks(peak_mzs[i] - mz_tolerance, peak_mzs[i] + mz_tolerance, debug) ; 
		}


        //bool GetPeak(double Peak, ref Peak2 pk)
        //{		
        //    pk.mdbl_mz = 0 ; 
        //    pk.mdbl_intensity = 0 ;
        //    std::map<double, int>::iterator iter_mz = mmap_pk_mz_index.find(Peak) ;  
        //    if (iter_mz == mmap_pk_mz_index.end())
        //        return false ; 
        //    int peak_index = (*iter_mz).second ; 
        //    pk = mvect_peak_tops[peak_index] ; 
        //    return true ; 
        //}

        public bool GetPeak(double Peak, ref Peak2 pk)
		{
            //perhaps use indexes here
            pk = mvect_peak_tops.Where(x => x.mdbl_mz >= Peak && x.mdbl_mz <= Peak).Select(x => x).FirstOrDefault();
            
			return true ; 
		}


        //bool GetPeakFromAll(double start_mz, double stop_mz, ref Peak2 pk)
        //{
        //    pk.mdbl_intensity = -10 ; 
        //    pk.mdbl_mz = 0 ; 
        //    bool found = false ; 

        //    for (std::map<double, int>::iterator iter_mz = mmap_pk_mz_index_all.lower_bound(start_mz) ;  
        //                    iter_mz != mmap_pk_mz_index_all.end() ; iter_mz++)
        //    {
        //        int peak_index = (*iter_mz).second ; 
        //        double mz_val = (*iter_mz).first ;  
        //        if (mz_val > stop_mz)
        //            return found ; 
        //        if (mvect_peak_tops[peak_index].mdbl_intensity >= pk.mdbl_intensity && mz_val >= start_mz )
        //        {
        //            double this_mz = mvect_peak_tops[peak_index].mdbl_mz  ; 
        //            pk = mvect_peak_tops[peak_index] ; 
        //            found = true ; 
        //        }
        //    }
        //    return found ; 
        //}

        //bool GetPeakFromAll(double start_mz, double stop_mz, ref Peak2 pk, double exclude_mass)
        //{
        //    pk.mdbl_intensity = -10 ; 
        //    pk.mdbl_mz = 0 ; 
        //    bool found = false ; 

        //    for (std::map<double, int>::iterator iter_mz = mmap_pk_mz_index_all.lower_bound(start_mz) ;  
        //                    iter_mz != mmap_pk_mz_index_all.end() ; iter_mz++)
        //    {
        //        int peak_index = (*iter_mz).second ; 
        //        double mz_val = (*iter_mz).first ;  
        //        if (mz_val == exclude_mass)
        //            continue ; 
        //        if (mz_val > stop_mz)
        //            return found ; 
        //        if (mvect_peak_tops[peak_index].mdbl_intensity >= pk.mdbl_intensity && mz_val >= start_mz )
        //        {
        //            double this_mz = mvect_peak_tops[peak_index].mdbl_mz  ; 
        //            pk = mvect_peak_tops[peak_index] ; 
        //            found = true ; 
        //        }
        //    }
        //    return found ; 
        //}

        //bool GetPeakFromAllOriginalIntensity(double start_mz, double stop_mz, ref Peak2 pk, double exclude_mass)
        //{
        //    pk.mdbl_intensity = -10 ; 
        //    pk.mdbl_mz = 0 ; 
        //    bool found = false ; 

        //    for (std::map<double, int>::iterator iter_mz = mmap_pk_mz_index_all.lower_bound(start_mz) ;  
        //                    iter_mz != mmap_pk_mz_index_all.end() ; iter_mz++)
        //    {
        //        int peak_index = (*iter_mz).second ; 
        //        double mz_val = (*iter_mz).first ;  
        //        if (mz_val == exclude_mass)
        //            continue ; 
        //        if (mz_val > stop_mz)
        //            return found ; 
        //        int data_index = mvect_peak_tops[peak_index].mint_data_index ; 
        //        if (mptr_vect_intensities->at(data_index) >= pk.mdbl_intensity && mz_val >= start_mz )
        //        {
        //            double this_mz = mvect_peak_tops[peak_index].mdbl_mz  ; 
        //            pk = mvect_peak_tops[peak_index] ; 
        //            found = true ; 
        //        }
        //    }
        //    return found ; 
        //}

        //bool GetPeakFromAllOriginalIntensity(double start_mz, double stop_mz, ref Peak2 pk)
        //{
        //    pk.mdbl_intensity = -10 ; 
        //    pk.mdbl_mz = 0 ; 
        //    bool found = false ; 


        //    for (std::map<double, int>::iterator iter_mz = mmap_pk_mz_index_all.lower_bound(stop_mz) ;  
        //        ; iter_mz--)
        //    {
        //        if(iter_mz == mmap_pk_mz_index_all.end())
        //            iter_mz-- ; 

        //        int peak_index = (*iter_mz).second ; 
        //        double mz_val = (*iter_mz).first ;  
        //        if (mz_val < start_mz)
        //            return found ; 
        //        if (mvect_peak_tops[peak_index].mdbl_intensity > pk.mdbl_intensity && mz_val >= start_mz && mz_val <= stop_mz )
        //        {
        //            double this_mz = mvect_peak_tops[peak_index].mdbl_mz  ; 
        //            pk = mvect_peak_tops[peak_index] ; 
        //            found = true ; 
        //        }
        //        if (iter_mz == mmap_pk_mz_index_all.begin())
        //            break ; 
        //    }
        //    return found ; 
        //}

        //bool GetPeak(double start_mz, double stop_mz, ref Peak2 pk)
        //{
        //    pk.mdbl_intensity = -10 ; 
        //    pk.mdbl_mz = 0 ; 
        //    bool found = false ; 

        //    for (std::map<double, int>::iterator iter_mz = mmap_pk_mz_index.lower_bound(start_mz) ;  
        //                    iter_mz != mmap_pk_mz_index.end() ; iter_mz++)
        //    {
        //        int peak_index = (*iter_mz).second ; 
        //        double mz_val = (*iter_mz).first ;  
        //        if (mz_val > stop_mz)
        //            return found ; 
        //        if (mvect_peak_tops[peak_index].mdbl_intensity > pk.mdbl_intensity && mz_val >= start_mz )
        //        {
        //            pk = mvect_peak_tops[peak_index] ; 
        //            found = true ; 
        //        }
        //    }
        //    return found ; 
        //}

        bool GetPeak(double start_mz, double stop_mz, ref Peak2 pk)
        {
            pk = mvect_peak_tops.Where(x => x.mdbl_mz >= start_mz && x.mdbl_mz <= stop_mz).Select(x => x).FirstOrDefault();
            
            return true ; 
        }

        //bool GetClosestPeak(double mz, double width, ref Peak2 pk)
        //{
        //    pk.mdbl_intensity = 0 ; 
        //    pk.mdbl_mz = 0 ; 
        //    bool found = false ; 

        //    double start_mz = mz-width ; 
        //    double stop_mz = mz+width ; 

        //    for (std::map<double, int>::iterator iter_mz = mmap_pk_mz_index.lower_bound(start_mz) ;  
        //                    iter_mz != mmap_pk_mz_index.end() ; iter_mz++)
        //    {
        //        int peak_index = (*iter_mz).second ; 
        //        double mz_val = (*iter_mz).first ;  
        //        if (mz_val > stop_mz)
        //            return found ; 
        //        if (mz_val >= start_mz && Helpers::absolute(mz_val - mz) < Helpers::absolute(pk.mdbl_mz - mz) )
        //        {
        //            pk = mvect_peak_tops[peak_index] ; 
        //            found = true ; 
        //        }
        //    }
        //    return found ; 
        //}

        bool GetClosestPeak(double mz, double width, ref Peak2 pk)
        {
            bool found = false;
            double start_mz = mz - width;
            double stop_mz = mz + width ;

            List<Peak2> candidates = mvect_peak_tops.Where(x => x.mdbl_mz >= start_mz && x.mdbl_mz <= stop_mz).Select(x => x).ToList();
            
            Peak2 bestPeak;
            if(candidates.Count>0)
            {
                found = true;
                bestPeak = candidates[0];
                double bestDifference = PeakProcessorHelpers.absolute(candidates[0].mdbl_mz - mz);
                foreach (var candidate in candidates)
                {
                    double difference = PeakProcessorHelpers.absolute(candidate.mdbl_mz - mz);
                    if(difference < bestDifference)
                    {
                        bestPeak = candidate;
                        bestDifference = difference;
                    }
                }
            }
            else
            {
                bestPeak = new Peak2();

            }

            pk = bestPeak;

            return found;
        }

        //bool GetClosestPeakFromAll(double mz, double width, ref Peak2 pk)
        //{
        //    pk.mdbl_intensity = 0 ; 
        //    pk.mdbl_mz = 0 ; 
        //    bool found = false ; 

        //    double start_mz = mz-width ; 
        //    double stop_mz = mz+width ; 

        //    for (std::map<double, int>::iterator iter_mz = mmap_pk_mz_index_all.lower_bound(start_mz) ;  
        //                    iter_mz != mmap_pk_mz_index_all.end() ; iter_mz++)
        //    {
        //        int peak_index = (*iter_mz).second ; 
        //        double mz_val = (*iter_mz).first ;  
        //        if (mz_val > stop_mz)
        //            return found ; 
        //        if (mz_val >= start_mz && Helpers::absolute(mz_val - mz) < Helpers::absolute(pk.mdbl_mz - mz) )
        //        {
        //            pk = mvect_peak_tops[peak_index] ; 
        //            found = true ; 
        //        }
        //    }
        //    return found ; 
        //}

        //void SetTIC(double tic)
        //{
        //    mdbl_tic = tic ; 
        //}

        //double GetTIC()
        //{
        //    return mdbl_tic ; 
        //}

		public void Clear()
		{
			mvect_peak_tops = new List<Peak2>() ; 
			mptr_vect_intensities = null ; 
			mptr_vect_mzs = null ; 
			mmap_pk_intensity_index = new SortedDictionary<int, int>() ;
            mmap_pk_mz_index = new SortedDictionary<double, int>();
            mmap_pk_mz_index_all = new SortedDictionary<double, int>(); 

		}

		int GetNumPeaks() 
		{ 
			return mvect_peak_tops.Count; 
		} 

		public void FilterList(double tolerance)
		{
			int num_peaks = mvect_peak_tops.Count ; 
			// peaks are supposed to be sorted in mz, but 
			// we'll just use the maps for this here. 

			mvect_temp_peak_tops = new List<Peak2>();
			InitializeUnprocessedPeakData() ; 

			for (int i = 0 ; i < num_peaks ; i++)
			{
				Peak2 next_peak = new Peak2() ; 
				bool infront = false ; 
				bool inback = false ; 
				// look for a peak behind. 
				double mz = mvect_peak_tops[i].mdbl_mz ; 
				inback = GetPeak(mz-tolerance, mz - 0.00001, ref next_peak) ; 
				infront = GetPeak(mz + 0.00001, mz+tolerance, ref next_peak) ; 

				if (inback || infront)
					mvect_temp_peak_tops.Add(mvect_peak_tops[i]) ; 
			}
			mvect_peak_tops = new List<Peak2>() ;
		    mvect_peak_tops.Add(mvect_peak_tops[0]);
            mvect_peak_tops.Add(mvect_temp_peak_tops[0]);
            mvect_peak_tops.Add(mvect_temp_peak_tops[mvect_temp_peak_tops.Count-1]); 
		}

        //void PrintPeaks()
        //{
        //    int num_peaks = (int) mvect_peak_tops.size() ; 
        //    std::cerr.precision(12); 
        //    std::cerr<<"\n Peak Tops for the "<<num_peaks<<" peaks found:"<<std::endl ; 
        //    for (int i = 0 ; i < num_peaks ; i++)
        //    {
        //        Peak pk = mvect_peak_tops[i] ; 
        //        std::cerr<<pk.mdbl_mz<<"\t\t"<<pk.mdbl_intensity<<"\t"<<pk.mdbl_FWHM<<"\t"<<pk.mdbl_SN<<std::endl ; 
        //    }
        //    return ; 
        //}

        //void PeakData::PrintUnprocessedPeaks(char *file_name)
        //{
        //    std::ofstream fout(file_name) ; 

        //    fout.precision(12); 
        //    fout<<"\n "<<(int)mmap_pk_intensity_index.size()<<" Unprocessed Peak Tops:"<<std::endl ; 
        //    for (std::multimap<int, int, std::greater<int> >::iterator iter = mmap_pk_intensity_index.begin() ; iter != mmap_pk_intensity_index.end() ; iter++)
        //    {
        //        int index = (*iter).second ; 
        //        Peak pk = mvect_peak_tops[index] ; 
        //        fout<<pk.mdbl_mz<<"\t\t"<<pk.mdbl_intensity<<"\t"<<pk.mdbl_FWHM<<"\t"<<pk.mdbl_SN<<std::endl ; 
        //    }
        //    fout.close() ; 
        //    return ; 
        //}

        //void PeakData::PrintUnprocessedPeaks()
        //{
        //    std::cerr.precision(12); 
        //    std::cerr<<"\n "<<(int)mmap_pk_intensity_index.size()<<" Unprocessed Peak Tops:"<<std::endl ; 
        //    for (std::multimap<int, int, std::greater<int> >::iterator iter = mmap_pk_intensity_index.begin() ; iter != mmap_pk_intensity_index.end() ; iter++)
        //    {
        //        int index = (*iter).second ; 
        //        Peak pk = mvect_peak_tops[index] ; 
        //        std::cerr<<pk.mdbl_mz<<"\t\t"<<pk.mdbl_intensity<<"\t"<<pk.mdbl_FWHM<<"\t"<<pk.mdbl_SN<<std::endl ; 
        //    }
        //    return ; 
        //}

        //void GetMzIntVectors(void **mzs, void **intensities)
        //{
        //    *mzs = (void *) mptr_vect_mzs ; 
        //    *intensities = (void *) mptr_vect_intensities ; 
        //}

		int GetNumUnprocessedPeaks()
		{
			return mmap_pk_mz_index.Count ; 
		}

        //void FindPeakAbsolute(double start_mz, double stop_mz, ref Peak2 peak) 
        //{
        //    // Anoop : modified from original FindPEak so as to return peaks only
        //    // and not shoulders, eliminates all the +ve Da DelM regions
        //    peak.mdbl_mz = -1 ; 
        //    peak.mdbl_intensity = 0 ; 
        //    double width = (stop_mz-start_mz)/2 ; 
        //    bool found_existing_peak = GetClosestPeak(start_mz+width, width, ref peak) ; 
        //    if (found_existing_peak)
        //    {
        //        // peak already exists. Send it back.
        //        return ; 
        //    }
        //    else
        //    {
        //        // peak doesn't exist. Lets find a starting index to start looking at. 
        //        // perhaps there was a peak there.
        //        bool found_peak = GetClosestPeakFromAll(start_mz+width, width, ref peak) ; 
        //        int num_pts = (int)mptr_vect_mzs->size() ; 
        //        if (found_peak)
        //        {
        //            int index = peak.mint_data_index ; 
        //            while(index > 0 && (*mptr_vect_mzs)[index] >= start_mz)
        //            {
        //                double intensity = (*mptr_vect_intensities)[index] ;
        //                double mz = (*mptr_vect_mzs)[index] ;
        //                if (intensity >  peak.mdbl_intensity && mz <= stop_mz)
        //                {
        //                    peak.mdbl_mz = mz ; 
        //                    peak.mdbl_intensity = intensity ; 
        //                    peak.mint_data_index = index ; 
        //                }
        //                index-- ; 
        //            }
        //            index = peak.mint_data_index ; 
        //            while(index < num_pts && (*mptr_vect_mzs)[index] <= stop_mz)
        //            {
        //                double intensity = (*mptr_vect_intensities)[index] ;
        //                if (intensity >  peak.mdbl_intensity)
        //                {
        //                    double mz = (*mptr_vect_mzs)[index] ;
        //                    peak.mdbl_mz = mz ; 
        //                    peak.mdbl_intensity = intensity ; 
        //                    peak.mint_data_index = index ; 
        //                }
        //                index++ ; 
        //            }
        //            if (peak.mdbl_intensity <= 0)
        //                peak.mdbl_mz = 0 ; 
        //            return ; 
        //        }
        //        else
        //        {
        //            return ; 
        //        }
        //    }
        //}
				

        //void FindPeak(double start_mz, double stop_mz, ref Peak2 peak) 
        //{
        //    peak.mdbl_mz = -1 ; 
        //    peak.mdbl_intensity = 0 ; 
        //    double width = (stop_mz-start_mz)/2 ; 
        //    bool found_existing_peak = GetClosestPeak(start_mz+width, width, peak) ; 
        //    if (found_existing_peak)
        //    {
        //        // peak already exists. Send it back.
        //        return ; 
        //    }
        //    else
        //    {
        //        // peak doesn't exist. Lets find a starting index to start looking at. 
        //        // perhaps there was a peak there.
        //        bool found_peak = GetClosestPeakFromAll(start_mz+width, width, peak) ; 
        //        int num_pts = (int)mptr_vect_mzs->size() ; 
        //        if (found_peak)
        //        {
        //            int index = peak.mint_data_index ; 
        //            while(index > 0 && mptr_vect_mzs[index] >= start_mz)
        //            {
        //                double intensity = mptr_vect_intensities[index] ;
        //                double mz = mptr_vect_mzs[index] ;
        //                if (intensity >  peak.mdbl_intensity && mz <= stop_mz)
        //                {
        //                    peak.mdbl_mz = mz ; 
        //                    peak.mdbl_intensity = intensity ; 
        //                    peak.mint_data_index = index ; 
        //                }
        //                index-- ; 
        //            }
        //            index = peak.mint_data_index ; 
        //            while(index < num_pts && mptr_vect_mzs[index] <= stop_mz)
        //            {
        //                double intensity = mptr_vect_intensities[index] ;
        //                if (intensity >  peak.mdbl_intensity)
        //                {
        //                    double mz = mptr_vect_mzs[index] ;
        //                    peak.mdbl_mz = mz ; 
        //                    peak.mdbl_intensity = intensity ; 
        //                    peak.mint_data_index = index ; 
        //                }
        //                index++ ; 
        //            }
        //            if (peak.mdbl_intensity <= 0)
        //                peak.mdbl_mz = 0 ; 
        //            return ; 
        //        }
        //        else
        //        {
        //            int start_index = mobj_pk_index.GetNearestBinary(mptr_vect_mzs, start_mz, 0, num_pts-1) ; 

        //            if (start_index > num_pts - 1)
        //                start_index = num_pts - 1 ;
        //            if (start_index < 0)
        //                start_index = 0 ; 

        //            if (mptr_vect_mzs[start_index] > start_mz)
        //            {
        //                while(start_index > 0 && mptr_vect_mzs[start_index] > start_mz)
        //                    start_index-- ; 
        //            }
        //            else
        //            {
        //                while(start_index < num_pts && mptr_vect_mzs[start_index] < start_mz)
        //                    start_index++ ; 
        //                start_index-- ; 
        //            }

        //            for (int i = start_index ; i < num_pts ; i++)
        //            {
        //                double mz = mptr_vect_mzs[i] ;
        //                double intensity = mptr_vect_intensities[i] ;
        //                if (mz > stop_mz)
        //                    break ; 						
        //                if (intensity >  peak.mdbl_intensity) 
        //                {
        //                    peak.mdbl_mz = mz ; 
        //                    peak.mdbl_intensity = intensity ; 
        //                    peak.mint_data_index = i ; 
        //                }
        //            }
        //        }//if(found_peak)
        //    }
        //}
	}
}