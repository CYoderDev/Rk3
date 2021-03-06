﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rk3.Toolset
{
    public class ArrayCalculator
    {
        public static int MedianOfMedians(int[] a, int index)
        {
            int iSubGrpLen = 5;
            var iSplitCount = Math.Ceiling((double)a.Length / iSubGrpLen);
            int[][] arrSubsets = new int[(int)iSplitCount][];
            int iMedian;
            int iPosition = 0;

            for (int i = 0; i < iSplitCount; i++)
            {
                //if there are less than 5 remaining values in subset, adjust length
                if (i == iSplitCount - 1 && a.Length % 5 != 0)
                    iSubGrpLen = (int)(a.Length % 5);

                arrSubsets[i] = new int[iSubGrpLen];

                for (int j = 0; j < iSubGrpLen; j++)
                {
                    arrSubsets[i][j] = a[j + iPosition];
                }
                iPosition += iSubGrpLen;
            }

            //Get medians of all subgroups into an array
            int[] arrSubGrpMedians = new int[(int)iSplitCount];

            for (var i = 0; i < arrSubsets.Length; i++)
            {
                ArrayCalculator.Quicksort(arrSubsets[i], 0, arrSubsets[i].Length - 1);
                arrSubGrpMedians[i] = arrSubsets[i][arrSubsets[i].Length / 2];
            }

            //If more than 5 values in array, then recurse
            if (arrSubGrpMedians.Length <= 5)
                iMedian = arrSubGrpMedians.OrderBy(x => x).ElementAt(arrSubGrpMedians.Length / 2);
            else
                iMedian = MedianOfMedians(arrSubGrpMedians, arrSubGrpMedians.Length / 2);

            //Separate lesser values to the left of the median, and greater to the right
            int[] low = a.Where(x => x < iMedian).ToArray();
            int[] high = a.Where(x => x > iMedian).ToArray();

            var k = low.Length;

            //Recursively narrow in on value based on provided index
            if (index < k)
                return MedianOfMedians(low, index);
            else if (index > k)
                return MedianOfMedians(high, index - k - 1);
            else
                return iMedian;
        }

        public static void Quicksort(int[] a, int left, int right)
        {
            if (left < right)
            {
                int iPivot = ArrayCalculator.partition(a, left, right);

                if (iPivot > 1)
                    Quicksort(a, left, iPivot - 1);

                if (iPivot + 1 < right)
                    Quicksort(a, iPivot + 1, right);
            }
        }

        private static int partition(int[] a, int left, int right)
        {
            int iPivot = a[left];

            while(true)
            {
                while (a[left] < iPivot)
                    left++;

                while (a[right] > iPivot)
                    right--;

                if (a[right] == iPivot && a[left] == iPivot)
                {
                    left++;
                }

                if (left < right)
                {
                    int iTmp = a[right];
                    a[right] = a[left];
                    a[left] = iTmp;
                }
                else
                {
                    return right;
                }
            }
        }
    }
}
