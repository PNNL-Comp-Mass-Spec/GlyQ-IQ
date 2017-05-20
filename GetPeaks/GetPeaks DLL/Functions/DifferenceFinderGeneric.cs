using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.Objects;
using System.Linq.Expressions;

namespace GetPeaks_DLL.Functions
{
    public class DifferenceFinder<T>
    {
        public void FindDifferences(ref List<T> differences, ref List<T> data, T tolerance)
        {
            List<DifferenceObject<T>> results = new List<DifferenceObject<T>>();

            T holdValue;
            T testValue;
            T dataValue;
            T differenceValue;
            T minimumValue;
            T maximumValue;
            for (int i = 0; i < differences.Count; i++)
            {
                for (int j = 0; j < data.Count; j++)
                {
                    holdValue = data[j];
                    
                    dataValue = holdValue;
                    differenceValue = differences[i];
                    
                    testValue = Add<T>(dataValue, differenceValue);

                    T dataPoint = default(T);
                    minimumValue = Subtract<T>(testValue, tolerance);
                    maximumValue = Add<T>(testValue,tolerance);

                    dataPoint = data.FirstOrDefault(c => And(GreatherThan<T>(c,minimumValue),GreatherThan<T>(c,maximumValue));

                    if (dataPoint > 0)
                    {
                        DifferenceObject<T> newMatch = new DifferenceObject<T>();
                        int index1 = data.BinarySearch(holdValue);
                        //int index1 = data.IndexOf(holdValue);
                        int index2 = data.IndexOf(testValue);
                        newMatch.Index1 = index1;
                        newMatch.Index2 = index2;
                        newMatch.Value1 = data[j];
                        newMatch.Value2 = dataPoint;
                        newMatch.difference = differences[i];
                        results.Add(newMatch);
                    }
                }
            }
        }

        public static T Add<T>(T a, T b)
        {
            //TODO: re-use delegate!
            // declare the parameters
            ParameterExpression paramA = Expression.Parameter(typeof(T), "a"),
                paramB = Expression.Parameter(typeof(T), "b");
            // add the parameters together
            BinaryExpression body = Expression.Add(paramA, paramB);
            // compile it
            Func<T, T, T> add = Expression.Lambda<Func<T, T, T>>(body, paramA, paramB).Compile();
            // call it
            return add(a, b);
        }

        public static T Subtract<T>(T a, T b)
        {
            //TODO: re-use delegate!
            // declare the parameters
            ParameterExpression paramA = Expression.Parameter(typeof(T), "a"),
                paramB = Expression.Parameter(typeof(T), "b");
            // add the parameters together
            BinaryExpression body = Expression.Subtract(paramA, paramB);
            // compile it
            Func<T, T, T> add = Expression.Lambda<Func<T, T, T>>(body, paramA, paramB).Compile();
            // call it
            return add(a, b);
        }

        public static T GreatherThan<T>(T a, T b)
        {
            //TODO: re-use delegate!
            // declare the parameters
            ParameterExpression paramA = Expression.Parameter(typeof(T), "a"),
                paramB = Expression.Parameter(typeof(T), "b");
            // add the parameters together
            BinaryExpression body = Expression.Subtract(paramA, paramB);
            // compile it
            Func<T, T, T> add = Expression.Lambda<Func<T, T, T>>(body, paramA, paramB).Compile();
            // call it
            return add(a, b);
        }

        public static T And<T>(T a, T b)
        {
            //TODO: re-use delegate!
            // declare the parameters
            ParameterExpression paramA = Expression.Parameter(typeof(T), "a"),
                paramB = Expression.Parameter(typeof(T), "b");
            // add the parameters together
            BinaryExpression body = Expression.And(paramA, paramB);
            // compile it
            Func<T, T, T> add = Expression.Lambda<Func<T, T, T>>(body, paramA, paramB).Compile();
            // call it
            return add(a, b);
        }

    }
}
