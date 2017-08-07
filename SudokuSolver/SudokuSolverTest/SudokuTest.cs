using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SudokuSolver;

namespace SudokuSolverTest
{
    [TestClass]
    public class SudokuTest
    {
        private bool _sudoComplete = false;

        [TestMethod]
        public void SetCellValue()
        {
            Sudoku sudo = new Sudoku(9);
            sudo.Completed += new Sudoku.IsComplete(verifySudokuSolved);
            sudo.SetCellValue(0, 0, 6);
            sudo.SetCellValue(2, 0, 3);
            sudo.SetCellValue(3, 0, 2);
            sudo.SetCellValue(4, 0, 4); //issue here, removes possible 4 from 6,2 ??
            sudo.SetCellValue(8, 0, 1);
            sudo.SetCellValue(0, 1, 4); 
            sudo.SetCellValue(1, 1, 1);
            sudo.SetCellValue(2, 1, 5);
            sudo.SetCellValue(5, 1, 7);
            sudo.SetCellValue(7, 1, 2);
            sudo.SetCellValue(8, 1, 3);
            sudo.SetCellValue(1, 2, 2);
            sudo.SetCellValue(6, 2, 4);
            sudo.SetCellValue(0, 3, 1);
            sudo.SetCellValue(5, 3, 4);
            sudo.SetCellValue(6, 3, 5);
            sudo.SetCellValue(8, 3, 2);
            sudo.SetCellValue(0, 4, 3);
            sudo.SetCellValue(2, 4, 6);
            sudo.SetCellValue(4, 4, 1);
            sudo.SetCellValue(6, 4, 9);
            sudo.SetCellValue(8, 4, 7);
            sudo.SetCellValue(0, 5, 5);
            sudo.SetCellValue(2, 5, 2);
            sudo.SetCellValue(3, 5, 8);
            sudo.SetCellValue(8, 5, 4);
            sudo.SetCellValue(2, 6, 1);
            sudo.SetCellValue(7, 6, 4);
            sudo.SetCellValue(0, 7, 2);
            sudo.SetCellValue(1, 7, 5);
            sudo.SetCellValue(3, 7, 3);
            sudo.SetCellValue(6, 7, 7);
            sudo.SetCellValue(7, 7, 1);
            sudo.SetCellValue(8, 7, 8);
            sudo.SetCellValue(0, 8, 7);
            sudo.SetCellValue(4, 8, 2);
            sudo.SetCellValue(5, 8, 1);
            sudo.SetCellValue(6, 8, 3);
            sudo.SetCellValue(8, 8, 9);

            sudo.Solve();

            Assert.IsTrue(this._sudoComplete);
        }

        private void verifySudokuSolved(object obj, EventArgs e)
        {
            Sudoku sudo = obj as Sudoku;

            if (sudo == null)
                return;

            foreach (var cell in sudo.GetCells())
            {
                Debug.WriteLine("Cell:{0},{1}\tValue:{2}", cell.xIndex, cell.yIndex, cell.Value);
            }

            this._sudoComplete = true;
        }
    }
}
