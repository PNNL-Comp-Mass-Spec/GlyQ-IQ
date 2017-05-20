using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//http://www.publicjoe.f9.co.uk/csharp/sort05.html

//typical call
//IQuickSortIndex sorter = new QuickSort();
//sorter.SortInt(ref dataL, ref indexL);
//starts to slow a bit with lists with 100,000 points

//test//
//List<decimal> dataDe = new List<decimal>();
//List<int> indexDe = new List<int>();
//for (int z = 0; z < 1000000; z++)
//{
//    //dataDe.Add(20 - (z * 2));
//    dataDe.Add(rand.Next());
//    indexDe.Add(z);
//}
//IQuickSortIndex sorterDe = new QuickSort();
//sorterDe.SortDecimal(ref dataDe, ref indexDe);

//test to see if each is larger than the next

namespace ConsoleApplication1
{
    //where T = System.ValueType;
    interface IQuickSortIndex
    {
        void SortInt(ref List<int> dataList, ref List<int> indexList);//this does not work
        void SortInt2(ref List<int> dataList, ref List<int> indexList);//uses a different algorithm than int
        void SortDouble(ref List<double> dataList, ref List<int> indexList);//uses a different algorithm than int
        void SortDecimal(ref List<decimal> dataList, ref List<int> indexList);//uses a different alforithm than int
    }


    public class QuickSort : IQuickSortIndex
    {
        #region interface calling appropriate quick sort
          
            void IQuickSortIndex.SortInt(ref List<int> dataList, ref List<int> indexList)
            {
                QuickSortInt sortInt = new QuickSortInt();
                sortInt.runQuickSortInt(ref dataList, ref indexList);
            }

            void IQuickSortIndex.SortInt2(ref List<int> dataList, ref List<int> indexList)
            {
                QuickSortInt2 sortInt2 = new QuickSortInt2();
                sortInt2.runQuickSortInt2(ref dataList, ref indexList);
            }

            void IQuickSortIndex.SortDouble(ref List<double> dataList, ref List<int> indexList)
            {
                QuickSortDouble sortDouble = new QuickSortDouble();
                sortDouble.runQuickSortDouble(ref dataList, ref indexList);
            }

            void IQuickSortIndex.SortDecimal(ref List<decimal> dataList, ref List<int> indexList)
            {
                QuickSortDecimal sortDecimal = new QuickSortDecimal();
                sortDecimal.runQuickSortDecimal(ref dataList, ref indexList);
            }

        #endregion

        #region three versions of quicksort class for int, double and decimal
            private class QuickSortInt
            {
                //array of integers to hold values
                private int[] a; //= new int[10];
                private int[] b;//= new int[10];

                // number of elements in array
                private int x;

                //set up data as arrays
                public void runQuickSortInt(ref List<int> data, ref List<int> index)
                {
                    a = data.ToArray();
                    b = index.ToArray();

                    sortArray();

                    for (int i = 0; i < data.Count; i++)
                    {
                        data[i] = a[i];
                        index[i] = b[i];
                    }
                }
            
                // Quick Sort Algorithm
                public void sortArray()
                {
                    x = a.GetLength(0);//number of elements in array
                    q_sortInt(0, x - 1);
                }

                //recursive part
                public void q_sortInt(int left, int right)
                {
                    int pivot, pivotB, l_hold, r_hold;

                    l_hold = left;
                    r_hold = right;
                    pivot = a[left];
                    pivotB = b[left];//index
                    while (left < right)
                    {
                        while ((a[right] >= pivot) && (left < right))
                        {
                            right--;
                        }

                        if (left != right)
                        {
                            a[left] = a[right];
                            b[left] = b[right];//index
                            left++;
                        }

                        while ((a[left] <= pivot) && (left < right))
                        {
                            left++;
                        }

                        if (left != right)
                        {
                            a[right] = a[left];
                            b[right] = b[right];//index
                            right--;
                        }
                    }

                    a[left] = pivot;
                    b[left] = pivotB;//index

                    pivot = left;
                    left = l_hold;
                    right = r_hold;

                    if (left < pivot)
                    {
                        q_sortInt(left, pivot - 1);
                    }

                    if (right > pivot)
                    {
                        q_sortInt(pivot + 1, right);
                    }

                }
            }

            private class QuickSortInt2
            {
                //array of integers to hold values
                private int[] a;
                private int[] b;

                // number of elements in array
                private int x;

                //set up data as arrays
                public void runQuickSortInt2(ref List<int> data, ref List<int> index)
                {
                    a = data.ToArray();
                    b = index.ToArray();

                    sortArray();

                    for (int i = 0; i < data.Count; i++)
                    {
                        data[i] = a[i];
                        index[i] = b[i];
                    }
                }

                // Quick Sort Algorithm
                public void sortArray()
                {
                    x = a.GetLength(0);//number of elements in array
                    q_sortInt2(0, x - 1);
                }

                //recursive part
                //http://www.eggheadcafe.com/community/aspnet/2/23006/how-to-modify-quicksort-to-handle-string.aspx

                public void q_sortInt2(int left, int right)
                {
                    int i = left;
                    int j = right;
                    int leftString = a[j];
                    int rightString = a[j];
                    int pivotValue = (left + right) / 2;
                    int middle = a[(int)pivotValue];
                    int temp = 0;
                    int tempIndex = 0;

                    while (i <= j)
                    {
                        while (a[i].CompareTo(middle) < 0)
                        {
                            i++;
                            leftString = a[j];
                        }

                        while (a[j].CompareTo(middle) > 0)
                        {
                            j--;
                            rightString = a[j];
                        }

                        if (i <= j)
                        {
                            temp = a[i];
                            tempIndex = b[i];

                            a[i] = a[j];
                            b[i] = b[j];
                            i++;

                            a[j] = temp;
                            b[j] = tempIndex;
                            j--;
                        }
                    }

                    if (left < j)
                    {
                        q_sortInt2(left, j);
                    }
                    if (i < right)
                    {
                        q_sortInt2(i, right);
                    }
                }
            }

            private class QuickSortDouble
            {
                //array of integers to hold values
                private double[] a;
                private int[] b;

                // number of elements in array
                private int x;

                //set up data as arrays
                public void runQuickSortDouble(ref List<double> data, ref List<int> index)
                {
                    a = data.ToArray();
                    b = index.ToArray();

                    sortArray();

                    for (int i = 0; i < data.Count; i++)
                    {
                        data[i] = a[i];
                        index[i] = b[i];
                    }
                }

                // Quick Sort Algorithm
                public void sortArray()
                {
                    x = a.GetLength(0);//number of elements in array
                    q_sortDouble(0, x - 1);
                }

                //recursive part
                //http://www.eggheadcafe.com/community/aspnet/2/23006/how-to-modify-quicksort-to-handle-string.aspx

                public void q_sortDouble(int left, int right)
                {
                    int i = left;
                    int j = right;
                    double leftString = a[j];
                    double rightString = a[j];
                    double pivotValue = (left + right) / 2;
                    double middle = a[(int)pivotValue];
                    double temp= 0;
                    int tempIndex = 0;

                    while (i <= j)
                    {
                        while (a[i].CompareTo(middle)<0)
                        {
                            i++;
                            leftString = a[j];
                        }

                        while (a[j].CompareTo(middle) > 0)
                        {
                            j--;
                            rightString = a[j];
                        }

                        if (i <= j)
                        {
                            temp = a[i];
                            tempIndex = b[i];

                            a[i] = a[j];
                            b[i] = b[j];
                            i++;

                            a[j] = temp;
                            b[j] = tempIndex;
                            j--;
                        }
                    }
                
                    if (left < j)
                    {
                        q_sortDouble(left, j);
                    }
                    if (i < right)
                    {
                        q_sortDouble(i, right);
                    }
                }
            }

            private class QuickSortDecimal
            {
                //array of integers to hold values
                private decimal[] a;
                private int[] b;

                // number of elements in array
                private int x;

                //set up data as arrays
                public void runQuickSortDecimal(ref List<decimal> data, ref List<int> index)
                {
                    a = data.ToArray();
                    b = index.ToArray();

                    sortArray();

                    for (int i = 0; i < data.Count; i++)
                    {
                        data[i] = a[i];
                        index[i] = b[i];
                    }
                }

                // Quick Sort Algorithm
                public void sortArray()
                {
                    x = a.GetLength(0);//number of elements in array
                    q_sortDecimal(0, x - 1);
                }

                //recursive part
                //http://www.eggheadcafe.com/community/aspnet/2/23006/how-to-modify-quicksort-to-handle-string.aspx

                public void q_sortDecimal(int left, int right)
                {
                    int i = left;
                    int j = right;
                    decimal leftString = a[j];
                    decimal rightString = a[j];
                    decimal pivotValue = (left + right) / 2;
                    decimal middle = a[(int)pivotValue];
                    decimal temp = 0;
                    int tempIndex = 0;

                    while (i <= j)
                    {
                        while (a[i].CompareTo(middle) < 0)
                        {
                            i++;
                            leftString = a[j];
                        }

                        while (a[j].CompareTo(middle) > 0)
                        {
                            j--;
                            rightString = a[j];
                        }

                        if (i <= j)
                        {
                            temp = a[i];
                            tempIndex = b[i];

                            a[i] = a[j];
                            b[i] = b[j];
                            i++;

                            a[j] = temp;
                            b[j] = tempIndex;
                            j--;
                        }
                    }

                    if (left < j)
                    {
                        q_sortDecimal(left, j);
                    }
                    if (i < right)
                    {
                        q_sortDecimal(i, right);
                    }
                }
            }
        #endregion
    }
}
    
    
    

