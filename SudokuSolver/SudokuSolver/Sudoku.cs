using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    public class Sudoku
    {
        private int _size;
        private SudokuBoard _board;

        public delegate void IsComplete(Sudoku sudoku, EventArgs e);
        public event IsComplete Completed;

        public Sudoku(int BoxSize)
        {
            this._size = BoxSize;
            this._board = new SudokuBoard(BoxSize);
        }

        public void SetCellValue(int x, int y, int value)
        {
            ValidateParameters(x, y);
            _board.SetValue(x, y, value);
        }

        public IEnumerable<Cell> GetCells()
        {
            return _board.GetCells();
        }

        public string GetCellValue(int xIndex, int yIndex)
        {
            int val = _board.GetValue(xIndex, yIndex);

            if (val == 0)
                return string.Empty;
            else
                return val.ToString();
        }

        public void Solve()
        {
            this._board.ValidateBoardCells();
            if (!this._board.HasProgressed)
                throw new Exception("Cannot solve until initial cell values have been defined.");

            _board.Solve();

            if (this.Completed != null && this._board.IsComplete)
                this.Completed(this, EventArgs.Empty);
        }

        public void ValidateParameters(int x, int y)
        {
            if (x + y > this._size * this._size)
                throw new ArgumentOutOfRangeException("Cell value outside of provided board range.");
        }
    }
}
