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
            int[][] arrays = new int[5][];
            Random randomSize = new Random();
            for (int i = 0; i < arrays.Length; i++)
            {
                arrays[i] = generateRandomIntArray(1, 1000, randomSize.Next(1, 200));
                //int selectIndex = randomSize.Next(1, arrays[i].Length);
                int selectIndex = arrays[i].Length / 2;
                ArrayCalculator<int> arrCalc = new ArrayCalculator<int>(arrays[i]);
                var med = arrCalc.GetMedian();
                Assert.IsTrue(med > arrays[i].Min() && med < arrays[i].Max());
                Assert.AreEqual(arrays[i].OrderBy(x => x).ElementAt(selectIndex), med, 
                    string.Format("\nIndex: {0}\n{1}",selectIndex, string.Join(System.Environment.NewLine, arrays[i].OrderBy(x => x))));
            }
        }

        [TestMethod]
        public void Median_WithInvalidArrayType_ThrowsException()
        {
            try
            {
                string[] arrString = new string[]{"t1","t2"};
                ArrayCalculator<string> arrCalc = new ArrayCalculator<string>(arrString);
                arrCalc.GetMedian();
            }
            catch (NotSupportedException ex)
            {
                StringAssert.Contains(ex.Message, "Cannot get median of array that is of type");
                return;
            }
            catch (Exception)
            {
                Assert.Fail("Incorrect exception type was thrown.");
            }
            Assert.Fail("No exception was thrown");
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
