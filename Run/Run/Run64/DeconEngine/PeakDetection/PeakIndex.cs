﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Run64.DeconEngine.PeakDetection
{
    public class PeakIndex
    {
        //! Used to search std::vector of points for closest point. 
		/*!
			Can look for the closest point either
			\arg \c By looking in a binary search between specified indices
			\arg \c linear search around a given index. 
		*/

		//! does a search for the given value by doing a linear scan to the left of the given index
		/*!
			\param vec is the std::vector of the points. 
			\param mz_val is the value we are looking for.
			\param start_index index of the peak to the left of which we are scanning.
			\return returns the index of the point that is closest to the specified value. 
		*/
		public int LookLeft(ref List<double> vec, double mz_val, int start_index)
		{
			// mv_val <= vec[start_index] so start moving index further left.
			int nearest_index = start_index ;
			int next_index = start_index ; 

			if (next_index == 0)
				return 0 ; 

			double next_val = vec[next_index] ; 
			double best_distance = PeakProcessorHelpers.absolute(mz_val - next_val) ; 

			while (next_val > mz_val)
			{
				next_index-- ; 
				next_val = vec[next_index] ; 
				double dist = PeakProcessorHelpers.absolute(next_val - mz_val) ; 
				if (dist < best_distance)
				{
					best_distance = dist ; 
					nearest_index = next_index ; 
				}
				if (next_index == 0)
					break ;
			}
			return nearest_index ; 
		}

		//! does a search for the given value by doing a linear scan to the right of the given index
		/*!
			\param vec is the std::vector of the points. 
			\param mz_val is the value we are looking for.
			\param start_index index of the peak to the right of which we are scanning.
			\return returns the index of the point that is closest to the specified value. 
		*/
		public int LookRight(ref List<double> vec, double mz_val, int start_index)
		{
			// mv_val >= vec[start_index] so start moving index further right.
			int nearest_index = start_index ;
			int next_index = start_index ; 
			int num_pts = (int) vec.Count() ; 

			if (next_index >= num_pts -1)
				return num_pts - 1 ; 

			double next_val = vec[next_index] ; 
			double best_distance = PeakProcessorHelpers.absolute(mz_val - next_val) ; 

			// we've gone back too far, posibly. Move pas the mz_val and return that value. 
			while (next_val < mz_val)
			{
				next_index++ ; 

				next_val = vec[next_index] ; 
				double dist = PeakProcessorHelpers.absolute(next_val - mz_val) ; 
				if (dist < best_distance)
				{
					best_distance = dist ; 
					nearest_index = next_index ; 
				}

				if (next_index == num_pts-1)
					break ;
			}
			return nearest_index ; 
		}

		//! Gets the index of the point nearest to the specified point, between the specified indices.
		/*!
			\param vec is the std::vector of the points. 
			\param mz_val is the value we are looking for.
			\param start_index minimum index of the point.
			\param stop_index maximum index of the point.
			\return returns the index of the point that is closest to the specified value. 
		*/
		public int GetNearestBinary(ref List<double> vec, double mz_val, int start_index, int stop_index)
		{
			double min_val, max_val, mid_val, mid_next_val ; 
			if (vec[start_index] > mz_val)
				return start_index ; 
			if (vec[stop_index] < mz_val)
				return stop_index ; 

			int mid_index ; 
			while (true)
			{
				min_val = vec[start_index] ; 
				max_val = vec[stop_index] ;
				if (Math.Abs(stop_index - start_index) <= 1 && mz_val >= min_val && mz_val <= max_val)
				{
					//return closer value.
					if (PeakProcessorHelpers.absolute(min_val - mz_val) < PeakProcessorHelpers.absolute(max_val - mz_val))
						return start_index ; 
					return stop_index ; 
				}

				double ratio = ((max_val - mz_val) * 1.0) / (max_val - min_val) ; 
				mid_index = (int) (start_index * ratio + stop_index * (1-ratio) + 0.5) ; 
				if (mid_index == start_index)
					mid_index = start_index + 1 ; 
				else if (mid_index == stop_index)
					mid_index = stop_index - 1 ; 
					
				mid_val = vec[mid_index] ; 
				if (mid_val >= mz_val)
				{
					stop_index = mid_index ; 
				}
				else if (mid_index +1 == stop_index)
				{
					if (PeakProcessorHelpers.absolute(mid_val - mz_val) < PeakProcessorHelpers.absolute(max_val - mz_val))
						return mid_index ; 
					return stop_index ; 			
				}
				else
				{
					mid_next_val = vec[mid_index+1] ; 
					if (mz_val >= mid_val && mz_val <= mid_next_val)
					{
						if (mz_val - mid_val < mid_next_val - mid_val)
						return mid_index ; 
						return mid_index+1 ; 
					}
					start_index =  mid_index + 1 ; 
				}
			}
		}


		//! Gets the index of the point nearest to the specified point.
		/*!
			\param vec is the std::vector of the points. 
			\param mz_val is the value we are looking for.
			\param start_index index around which we are looking for the specified point.
			\return returns the index of the point that is closest to the specified value. 
		*/
		public int GetNearest(ref List<double> vec, double mz_val, int start_index)
		{
			// we're going to use continuity here, look at the difference 
			// between consecutive points and estimate how much further we have to 
			// go and start there. 
			int num_pts = (int)vec.Count - 1 ; 

			if (mz_val >= vec[num_pts])
				return num_pts ; 
			if (mz_val < vec[0])
				return 0 ; 

			double distance_to_go = mz_val - vec[start_index] ; 
			double step ; 
			if (start_index < num_pts)
				step = vec[start_index+1] - vec[start_index] ;
			else 
				step = vec[start_index] - vec[start_index-1] ;


			int move_by = Convert.ToInt32(distance_to_go /step) ; 
			int next_index = start_index + move_by ; 

			if (next_index < 0)
				next_index = 0; 
			if (next_index > num_pts)
				next_index = num_pts - 1 ; 

			if (mz_val >= vec[next_index])
				return LookRight(ref vec, mz_val, next_index) ; 
			else
				return LookLeft(ref vec, mz_val, next_index) ; 
		}
		
    }
}
