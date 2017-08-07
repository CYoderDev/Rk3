using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    public class Cell
    {
        public event ValueChangedHandler ValueChanged;
        public delegate void ValueChangedHandler(Cell c, EventArgs e);

        /// <summary>
        /// The current value of the cell. 0 means no value assigned.
        /// </summary>
        public int Value 
        { 
            get { return _value; } 
            private set
            {
                if (value < 1)
                    this._value = 0;
                else if (this.Possibilities.Contains(value))
                    this._value = value;
                else
                    throw new ArgumentOutOfRangeException("Given cell value is outside of the range of possible values.");
            } 
        }
        private int _value;

        /// <summary>
        /// A set of possible values for the cell.
        /// </summary>
        internal ISet<int> Possibilities;

        /// <summary>
        /// Horizontal index of the cell on the board.
        /// </summary>
        public int xIndex { get; private set; }

        /// <summary>
        /// Vertical index of the cell on the board.
        /// </summary>
        public int yIndex { get; private set; }

        /// <summary>
        /// Represents a sudoku cell.
        /// </summary>
        /// <param name="x">horizontal index of the cell.</param>
        /// <param name="y">vertical index of the cell.</param>
        internal Cell( int x, int y)
        {
            this._value = 0;
            this.xIndex = x;
            this.yIndex = y;
            this.Possibilities = new HashSet<int>();
            initializePossibilities();
        }

        /// <summary>
        /// Resets all possible values for the cell
        /// </summary>
        internal void resetPossibilities()
        {
            this.Possibilities.Clear();
            initializePossibilities();
        }

        /// <summary>
        /// Remove a possible value from the cell.
        /// </summary>
        /// <param name="val">Value to remove.</param>
        /// <returns>True if a new value was assigned to the cell.</returns>
        internal bool RemovePossibility(int val)
        {
            return RemovePossibility(val, false);
        }

        /// <summary>
        /// Remove a possible value from the cell.
        /// </summary>
        /// <param name="val">Value to remove.</param>
        /// <param name="solving">Whether or not the puzzle is currently being solved.</param>
        /// <returns>True if a new value was assigned to the cell.</returns>
        internal bool RemovePossibility(int val, bool solving)
        {
            if (this.Possibilities.Contains(val) && !HasValue())
                this.Possibilities.Remove(val);


            if (this.Possibilities.Count == 1 && solving)
            {
                this.Value = this.Possibilities.First();
                if (ValueChanged != null)
                    ValueChanged(this, EventArgs.Empty);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Attempts to set the cell value.
        /// </summary>
        /// <param name="value">Value to set.</param>
        /// <returns>True if the value was successfully assigned to the cell.</returns>
        internal bool TrySetValue(int value)
        {
            if (Possibilities.Contains(value) && !HasValue())
            {
                this.Value = value;
                resetPossibilities();
                if (ValueChanged != null)
                    ValueChanged(this, EventArgs.Empty);
                return true;
            }
            else if (HasValue() && value != this.Value)
                throw new Exception(string.Format("Attempted to set cell {0},{1} value to {2} when it already has set value of {3}",
                    xIndex, yIndex, value, Value));
            return false;
        }

        /// <summary>
        /// If the cell currently has an assigned value.
        /// </summary>
        /// <returns>True if a value is currently assigned to the cell.</returns>
        public bool HasValue()
        {
            return this.Value > 0;
        }

        /// <summary>
        /// Initializes all existing possibilities for the cell, or sets the possibilities to
        /// the current value if one exists.
        /// </summary>
        private void initializePossibilities()
        {
            for (int i = 1; i <= 9; i++)
            {
                if (!HasValue() || this.Value == i)
                    this.Possibilities.Add(i);
            }
        }

    }
}
