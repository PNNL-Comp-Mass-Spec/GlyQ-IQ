using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetPeaks_DLL.Functions
{
    public class DictionaryReplacement<T>
    {
        public List<Tuple<int, T>> Data { get; set; }
        private List<int> Keys { get; set; }
        private List<T> Values { get; set; }

        public DictionaryReplacement()
        {
            InitializeDictionary();
        }

        public void InitializeDictionary()
        {
            Data = new List<Tuple<int, T>>();
            Keys = new List<int>();
            Values = new List<T>();
        }

        public void AddElement(int key, T value)
        {
            //1.  add data
            Tuple<int, T> newItem = new Tuple<int, T>(key, value);
            Data.Add(newItem);
            
            //2 sort data
            SortDictionaryByKey();

            //3 re sync keys
            Keys = new List<int>();
            Values = new List<T>();
            foreach (Tuple<int,T> tuple in Data)
            {
                Keys.Add(tuple.Item1);
                Values.Add(tuple.Item2);
            }
        }

        public bool ContainsKey(int key)
        {
            bool containsKey = Keys.Contains(key);

            return containsKey;
        }

        public T FetchValue(int key)
        {
            T returnValue = default(T);

            if (Keys.Contains(key))
            {
                int index = Keys.BinarySearch(key);
                returnValue = Values[index];
            }
            else
            {
                return returnValue;
            }

            return returnValue;
        }

        public int FetchKey(T value)
        {
            int returnValue = -1;

            if (Values.Contains(value))
            {
                int index = Values.BinarySearch(value);
                returnValue = Keys[index];
            }
            else
            {
                return returnValue;
            }

            return returnValue;
        }

        public void UpdateValue(int key, T newValue)
        {
            if (Keys.Contains(key))
            {
                int index = Keys.BinarySearch(key);
                Values[index] = newValue;
                Data[index]= new Tuple<int, T>(key,newValue);
            }
            
        }

        public Tuple<int,T> FetchTuple(int key)
        {
            Tuple<int,T> returnValue = new Tuple<int, T>(0,default(T));

            if (Keys.Contains(key))
            {
                int index = Keys.BinarySearch(key);
                returnValue = Data[index];
            }
            else
            {
                return returnValue;
            }

            return returnValue;
        }

        public void DeleteElementByKey(int key)
        {
            if (Keys.Contains(key))
            {
                int index = Keys.BinarySearch(key);
                Keys.RemoveAt(index);
                Values.RemoveAt(index);
                Data.RemoveAt(index);
            }
        }

        public void DeleteElementByValue(T value)
        {
            if (Values.Contains(value))
            {
                int index = Values.BinarySearch(value);
                Keys.RemoveAt(index);
                Values.RemoveAt(index);
                Data.RemoveAt(index);
            }
        }

        public void SortDictionaryByKey()
        {
            List<Tuple<int, T>> sortedPile = Data.OrderBy(c => c.Item1).ToList();
            Data = sortedPile;
        }

        public void SortReverseDictionaryByKey()
        {
            List<Tuple<int, T>> sortedPile = Data.OrderByDescending(c => c.Item1).ToList();
            Data = sortedPile;
        }



        //newMatch.IndexData = data.BinarySearch(holdValue);
        //                newMatch.IndexMatch = data.FindIndex(c => c > testValue - tolerance & c < testValue + tolerance);



        // dataPoint = data.FirstOrDefault(c => c > testValue - tolerance & c < testValue + tolerance);

        //            if (dataPoint > 0)
        //            {
        //                DifferenceObject<double> newMatch = new DifferenceObject<double>();
        //                newMatch.IndexData = data.BinarySearch(holdValue);
        //                newMatch.IndexMatch = data.FindIndex(c => c > testValue - tolerance & c < testValue + tolerance);


    }


}
