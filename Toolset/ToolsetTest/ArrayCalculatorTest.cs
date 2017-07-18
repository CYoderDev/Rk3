using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rk3.Toolset;

namespace ToolsetTest
{
    [TestClass]
    public class ArrayCalculatorTest
    {
        [TestMethod]
        public void Median_WithValidArrayType_ReturnsMedianValue()
        {
            Random randomNum = new Random();
            for (int i = 0; i < 5; i++)
            {
                int[] array = generateRandomIntArray(1, 1000, randomNum.Next(1, 200)).Distinct().ToArray();
                //int selectIndex = randomSize.Next(1, arrays[i].Length);
                int selectIndex = array.Length / 2;
                var med = ArrayCalculator.MedianOfMedians(array, selectIndex);
                Assert.IsTrue(med >= array.Min() && med <= array.Max(), 
                    string.Format("(Med){0} >= {1} && =< {2}", med, array.Min(), array.Max()));
                Assert.AreEqual(array.OrderBy(x => x).ElementAt(selectIndex), med, 
                    string.Format("\nIndex: {0}\n{1}",selectIndex, string.Join(System.Environment.NewLine, 
                    array.OrderBy(x => x))));
            }
        }

        [TestMethod]
        public void Median_WithPreDeterminedArray_ReturnsSmallestNthValue()
        {
            int[] array = new int[] { 10, 1, 5, 20, 15, 3, 6, 21, 32, 16, 0, 10, 16 };
            int selectIndex = array.Length / 2;
            var med = ArrayCalculator.MedianOfMedians(array, selectIndex);
            Assert.IsTrue(med >= array.Min() && med <= array.Max(),
                    string.Format("(Med){0} >= {1} && =< {2}", med, array.Min(), array.Max()));
            Assert.AreEqual(array.Distinct().OrderBy(x => x).ElementAt(selectIndex), med,
                string.Format("\nIndex: {0}\n{1}", selectIndex, string.Join(System.Environment.NewLine,
                array.OrderBy(x => x))));
        }

        [TestMethod]
        public void QuickSort_WithPreDeterminedArray_SortsNumerically()
        {
            int[] array = new int[] { 10, 1, 5, 20, 15, 3, 6, 21, 32, 16, 0, 10, 16 };
            ArrayCalculator.Quicksort(array, 0, array.Length - 1);
            CollectionAssert.AreEqual(array.OrderBy(x => x).ToArray(), array, 
                string.Format("\nExpected: {0}\nActual: {1}", 
                    string.Join(",", array.OrderBy(x => x).ToArray()),
                    string.Join(",", array)));
        }

        private int[] generateRandomIntArray(int MinVal, int MaxVal, int Size)
        {
            int[] ret = new int[Size];

            Random randNum = new Random();

            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = randNum.Next(MinVal, MaxVal);
            }

            return ret;
        }
    }
}
