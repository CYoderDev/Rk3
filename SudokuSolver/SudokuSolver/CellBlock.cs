using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    internal class CellBlock : IEnumerable<Cell>
    {
        private IEnumerable<Cell> Cells;
        private ISet<int> remainingNumbers;

        /// <summary>
        /// Cell block represents a row, column, or square of cells
        /// </summary>
        /// <param name="Cells">Cells that belong to the cell block</param>
        internal CellBlock(IEnumerable<Cell> Cells)
        {
            this.Cells = Cells;
            this.remainingNumbers = new HashSet<int>();
        }

        /// <summary>
        /// Sets the possibilities of each cell based on the values of other cells within the cell block.
        /// </summary>
        /// <param name="solving">If the sudoku board is being solved.</param>
        internal void setPossibilities(bool solving)
        {
            //Get cells that contain values, and those values
            var cellsWithVals = this.Cells.Where(x => x.HasValue());
            var setValues = cellsWithVals.Select(x => x.Value).Distinct();

            for (int i = 1; i <= Cells.Count(); i++)
            {
                if (setValues.Contains(i))
                    Cells.Where(x => x.Possibilities.Count > 1).ToList().ForEach(x =>
                    {
                        if (x.RemovePossibility(i, solving))
                        {                         
                            cellsWithVals = this.Cells.Where(y => y.HasValue());
                            setValues = cellsWithVals.Select(y => y.Value).Distinct();
                        }
                    });
            }
        }

        /// <summary>
        /// Get a possibility value that only belongs to a single cell in the cell block.
        /// </summary>
        /// <returns>A distinct value belonging to only one cell</returns>
        internal int getUniquePossibility()
        {
            var possibilities = Cells.Where(x => !x.HasValue()).SelectMany(x => x.Possibilities)
                .GroupBy(x => x)
                .Where(group => group.Count() == 1)
                .Select(group => group.Key);

            if (possibilities.Count() == 1)
                return possibilities.FirstOrDefault();
            else
                return 0;
        }

        /// <summary>
        /// Get a cell within a cell block.
        /// </summary>
        /// <param name="x">horizontal index</param>
        /// <param name="y">vertical index</param>
        /// <returns></returns>
        internal Cell GetCell(int x, int y)
        {
            return Cells.Where(c => c.xIndex == x && c.yIndex == y).FirstOrDefault();
        }

        public IEnumerator<Cell> GetEnumerator()
        {
            return Cells.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
