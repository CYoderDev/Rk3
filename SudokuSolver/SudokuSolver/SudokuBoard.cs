using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    class SudokuBoard: ICloneable
    {
        private int _defaultMax;
        private int _largestDivisor;
        private Cell[,] _board;
        private bool hasProgressed = false;
        private bool _isComplete = false;
        private bool _solving = false;

        internal bool HasProgressed { get => hasProgressed; private set => hasProgressed = value; }
        internal bool IsComplete { get => _isComplete; private set => _isComplete = value; }

        /// <summary>
        /// Represents a sudoku board containing cells.
        /// </summary>
        /// <param name="Size">The amount of squares horizontally or vertically. Size x Size.</param>
        internal SudokuBoard(int Size)
        {
            this._board = new Cell[Size, Size];
            this._defaultMax = Size;
            this._largestDivisor = getLargestDivisor(_defaultMax);
            ValidateBoardCells();
        }

        /// <summary>
        /// Sets the value of the cell in the provided location.
        /// </summary>
        /// <param name="x">Horizontal index position of the cell on the board from left to right.</param>
        /// <param name="y">Veritical index position of the cell on the board from bottom to top.</param>
        /// <param name="value">Value to assign to the cell.</param>
        internal void SetValue(int x, int y, int value)
        {
            CellBlock[] cellBlocks = getCellBlocks(x, y);

            getCellBlocks(x, y).First().GetCell(x, y).TrySetValue(value);
        }

        /// <summary>
        /// Gets the value of the cell in the provided location.
        /// </summary>
        /// <param name="x">Horizontal index position of the cell on the board from left to right.</param>
        /// <param name="y">Veritical index position of the cell on the board from bottom to top.</param>
        /// <returns>The value assigned to the cell. 0 means no value assigned.</returns>
        internal int GetValue(int x, int y)
        {
            if (x > _defaultMax || y > _defaultMax)
                throw new ArgumentOutOfRangeException("Provided indexes are outside the range of the board.");

            return _board[x, y].Value;
        }

        /// <summary>
        /// Gets all cells on the board.
        /// </summary>
        /// <returns>Enumerable of cell objects.</returns>
        internal IEnumerable<Cell> GetCells()
        {
            foreach (var cell in _board)
                yield return cell;
        }

        /// <summary>
        /// Attempts to solve the sudoku puzzle.
        /// </summary>
        internal void Solve()
        {
            this._solving = true;
            for (int i = 0; i < _defaultMax; i++)
                for (int j = 0; j < _defaultMax; j++)
                {
                    Cell cell = _board[i, j];

                    if (cell.HasValue())
                        continue;

                    if (cell.Possibilities.Count == 1)
                    {
                        cell.TrySetValue(cell.Possibilities.First());
                        continue;
                    }

                    ICollection<CellBlock> adjRows = getAdjacentRows(j);
                    ICollection<CellBlock> adjCols = getAdjacentColumns(i);
                    ICollection<CellBlock> adjBlocks = getAdjacentSquares(i, j);

                    var rowVals = getDuplicateCellValues(getCellBlockCellValues(adjRows), adjRows.Count).Where(x => cell.Possibilities.Contains(x));
                    var colVals = getDuplicateCellValues(getCellBlockCellValues(adjCols), adjCols.Count).Where(x => cell.Possibilities.Contains(x));
                    var blkVals = getDuplicateCellValues(getCellBlockCellValues(adjBlocks), adjBlocks.Count).Where(x => cell.Possibilities.Contains(x));

                    var setVal = Enumerable.Intersect(rowVals, colVals).Intersect(blkVals).Distinct();

                    if (setVal.Count() != 1)
                    {
                        int uniquePossibility = 0;
                        uniquePossibility = GetSquare(i, j).getUniquePossibility();

                        if (uniquePossibility > 0 && cell.Possibilities.Contains(uniquePossibility) && !setVal.Contains(uniquePossibility))
                            setVal = setVal.Concat(new List<int>() { uniquePossibility });
                    }

                    if(setVal.Count() == 1)
                        SetValue(i, j, setVal.First());
                }

            if (!isComplete() && !simulate())
            {
                throw new Exception("Unable to solve.");
            }
        }

        /// <summary>
        /// Attempts to create a simulation when there is no clear value for the remaining
        /// unsolved cells. It will determine which values to set based on whether or not setting
        /// those values solved the puzzle. If successful, it will mirror the successful outcome onto the existing board.
        /// </summary>
        /// <returns>Whether or not the simulation was successful.</returns>
        private bool simulate()
        {
            List<int> exclude = new List<int>();
            while (true)
            {
                SudokuBoard newBoard = this.Clone() as SudokuBoard;
                newBoard._solving = true;
                List<Tuple<int, int, int>> setValues = new List<Tuple<int, int, int>>();
                var cellsWithoutVals = newBoard.getCellsWithoutValues();
                var uniquePossibilities = cellsWithoutVals.SelectMany(x => x.Possibilities).Except(exclude).ToList();
                foreach (var possibility in uniquePossibilities)
                {
                    foreach (var cell in cellsWithoutVals)
                    {
                        if (cell.HasValue() || !cell.Possibilities.Contains(possibility))
                            continue;

                        newBoard.SetValue(cell.xIndex, cell.yIndex, possibility);
                        setValues.Add(new Tuple<int, int, int>(cell.xIndex, cell.yIndex, possibility));
                    }
                    if (!newBoard.isComplete())
                        exclude.Add(possibility);
                }
                if (newBoard.isComplete())
                {
                    setValues.ForEach(x =>
                    {
                        this.SetValue(x.Item1, x.Item2, x.Item3);
                    });
                    return this.isComplete();
                }
                else if (setValues.Count < 1)
                    break;
            }

            return false;
        }

        /// <summary>
        /// Event handler to handle when a cell block's value has changed.
        /// </summary>
        /// <param name="obj">Sudoku Cell.</param>
        /// <param name="e">Event Arguments.</param>
        private void cellBlockValueHasChanged(object obj, EventArgs e)
        {
            var cell = obj as Cell;

            if (cell == null)
                return;

            var cellBlocks = getCellBlocks(cell.xIndex, cell.yIndex);

            for (int i = 0; i < cellBlocks.Length; i++)
            {
                cellBlocks[i].setPossibilities(this._solving);
            }
        }

        /// <summary>
        /// Gets the row, column, and square to which the provide cell belongs.
        /// </summary>
        /// <param name="x">Horizontal index position of the cell on the board from left to right.</param>
        /// <param name="y">Veritical index position of the cell on the board from bottom to top.</param>
        /// <returns>Each cell block as an array.</returns>
        private CellBlock[] getCellBlocks(int x, int y)
        {
            return new CellBlock[3] { GetRow(y), GetColumn(x), GetSquare(x, y) };
        }

        /// <summary>
        /// Gets all cells on the board that currently do not have an assigned value.
        /// </summary>
        /// <returns>All cells without a value as an enumerable object.</returns>
        private IEnumerable<Cell> getCellsWithoutValues()
        {
            foreach (var cell in _board)
            {
                if (!cell.HasValue())
                    yield return cell;
            }
        }

        /// <summary>
        /// Gets all cell values that appear more than the provided count in a collection.
        /// </summary>
        /// <param name="vals">Values to check.</param>
        /// <param name="count">Maximum amount of times a value is allowed to appear.</param>
        /// <returns></returns>
        private IEnumerable<int> getDuplicateCellValues(IEnumerable<int> vals, int count)
        {
            return vals.GroupBy(x => x)
                .Where(group => group.Count() > count - 1)
                .Select(group => group.Key);
        }

        /// <summary>
        /// Gets all of the values within a cell block.
        /// </summary>
        /// <param name="cellBlocks">Collection of cell blocks to get values from.</param>
        /// <returns>Enumerable of cell values within the cell block.</returns>
        private IEnumerable<int> getCellBlockCellValues(ICollection<CellBlock> cellBlocks)
        {
            foreach (var cellBlock in cellBlocks)
            {
                foreach (var cell in cellBlock)
                {
                    yield return cell.Value;
                }
            }
        }

        /// <summary>
        /// Gets 2 rows that are adjacent, or nearest to the provided vertical index.
        /// </summary>
        /// <param name="yIndex">Veritical index position of the cell on the board from bottom to top.</param>
        /// <returns>Depending on the position of the row within each square, either the 2 rows above or below, or the row above
        /// and below.</returns>
        private ICollection<CellBlock> getAdjacentRows(int yIndex)
        {
            ICollection<CellBlock> colRows = new Collection<CellBlock>();
            //if horizontal space is furthest to the bottom in block
            if (yIndex % _largestDivisor == 0)
            {
                colRows.Add(GetRow(yIndex + 1));
                colRows.Add(GetRow(yIndex + 2));
            }
            //if horizontal space is furthest to the top in block
            else if (yIndex == _defaultMax - 1 || yIndex + 1 < _defaultMax - 1 && (yIndex + 1) % _largestDivisor == 0)
            {
                colRows.Add(GetRow(yIndex - 1));
                colRows.Add(GetRow(yIndex - 2));
            }
            //if horizontal space is in the middle of the block
            else
            {
                colRows.Add(GetRow(yIndex + 1));
                colRows.Add(GetRow(yIndex - 1));
            }

            return colRows;
        }

        /// <summary>
        /// Gets 2 columns that are adjacent, or nearest to the provided horizontal index.
        /// </summary>
        /// <param name="xIndex">Horizontal index position of the cell on the board from left to right.</param>
        /// <returns>epending on the position of the row within each square, either the 2 rows above or below, or the row above
        /// and below.</returns>
        private ICollection<CellBlock> getAdjacentColumns(int xIndex)
        {
            ICollection<CellBlock> colCols = new Collection<CellBlock>();
            //if vertical space is furthest to the bottom in block
            if (xIndex % _largestDivisor == 0)
            {
                colCols.Add(GetColumn(xIndex + 1));
                colCols.Add(GetColumn(xIndex + 2));
            }
            //if vertical space is furthest to the top in block
            else if (xIndex == _defaultMax - 1 || xIndex + 1 < _defaultMax - 1 && (xIndex + 1) % _largestDivisor == 0)
            {
                colCols.Add(GetColumn(xIndex - 1));
                colCols.Add(GetColumn(xIndex - 2));
            }
            //if vertical space is in the middle of the block
            else
            {
                colCols.Add(GetColumn(xIndex + 1));
                colCols.Add(GetColumn(xIndex - 1));
            }

            return colCols;
        }

        /// <summary>
        /// Gets the squares to the left, right, above, and below of the square to which the provided
        /// cell belongs if it exists.
        /// </summary>
        /// <param name="xIndex">Horizontal index position of the cell on the board from left to right.</param>
        /// <param name="yIndex">Veritical index position of the cell on the board from bottom to top.</param>
        /// <returns>The squares to the left, right, above, and below of the square.</returns>
        private ICollection<CellBlock> getAdjacentSquares(int xIndex, int yIndex)
        {
            ICollection<CellBlock> colSquares = new Collection<CellBlock>();

            int xStart = xIndex, yStart = yIndex;

            xIndex = xStart - _largestDivisor;
            
            //Loop to go left, top, right, then bottom
            do
            {
                //Check if the square exists, if so, get it and add to the collection
                if (xIndex >= 0 && xIndex < _defaultMax && yIndex >= 0 && yIndex < _defaultMax)
                {
                    colSquares.Add(GetSquare(xIndex, yIndex));
                }

                //Change the x and y index accordingly to cells belonging in the squares we want to return
                var xtmp = xIndex;
                xIndex = xIndex == xStart ? yIndex > yStart ? xIndex + _largestDivisor : xIndex - _largestDivisor : xStart;
                yIndex = yIndex == yStart ? xtmp < xStart ? yIndex + _largestDivisor : yIndex - _largestDivisor : yStart;

            } while (xIndex != xStart - _largestDivisor || yIndex != yStart);

            return colSquares;

        }

        /// <summary>
        /// Get the row of cells at the specified position.
        /// </summary>
        /// <param name="index">Vertical index position of the cell on the board from bottom to top.</param>
        /// <returns>A row of cells.</returns>
        private CellBlock GetRow(int index)
        {
            IList<Cell> cells = new List<Cell>();
            for (int i = 0; i < _defaultMax; i++)
                cells.Add(_board[i,index]);

            return new CellBlock(cells);
        }

        /// <summary>
        /// Get the column of cells at the specified position.
        /// </summary>
        /// <param name="index">Horizontal index position of the cell on the board from left to right.</param>
        /// <returns>A column of cells.</returns>
        private CellBlock GetColumn(int index)
        {
            IList<Cell> cells = new List<Cell>();
            for (int i = 0; i < _defaultMax; i++)
                cells.Add(_board[index, i]);

            return new CellBlock(cells);
        }

        /// <summary>
        /// Get the block or square of cells at the specified position.
        /// </summary>
        /// <param name="xIndex">Horizontal index position of the cell on the board from left to right.</param>
        /// <param name="yIndex">Vertical index position of the cell on the board from bottom to top.</param>
        /// <returns></returns>
        private CellBlock GetSquare(int xIndex, int yIndex)
        {
            IList<Cell> cells = new List<Cell>();

            int remainder;

            //Loop to move the x and y index to the bottom-left most cell within
            //the square.
            do
            {
                xIndex = xIndex == 0 || xIndex % _largestDivisor == 0 && xIndex != 1 ? xIndex : xIndex - 1;
                yIndex = yIndex == 0 || yIndex % _largestDivisor == 0 && yIndex != 1 ? yIndex : yIndex - 1;
                int iLargest = xIndex % _largestDivisor >= yIndex % _largestDivisor ? xIndex : yIndex % _largestDivisor >= xIndex % _largestDivisor ? yIndex : 0;
                remainder = iLargest % _largestDivisor;

                remainder = remainder == 0 ? _largestDivisor : remainder;
            } while (remainder % _largestDivisor != 0 || xIndex == 1 || yIndex == 1);


            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                    cells.Add(_board[xIndex + i, yIndex + j]);
            }

            return new CellBlock(cells);
        }

        /// <summary>
        /// Validates and initializes all cells within the board.
        /// </summary>
        internal void ValidateBoardCells()
        {
            int possibilities = _defaultMax;
            for (int i = 0; i < this._defaultMax; i++)
            {
                for (int j = 0; j < this._defaultMax; j++)
                {
                    if (this._board[i, j] == null)
                        this._board[i, j] = new Cell(i, j);
                    Cell sudoCell = this._board[i, j];
                    int cellPossibilities = sudoCell.Possibilities.Count;
                    if (cellPossibilities < possibilities)
                        possibilities = cellPossibilities;
                    sudoCell.ValueChanged += new Cell.ValueChangedHandler(cellBlockValueHasChanged);
                }
            }
            if (possibilities > 1 && possibilities < this._defaultMax)
                throw new Exception("Invalid game board. There are no cells with a single possible value.");
            else if (possibilities != this._defaultMax)
                this.HasProgressed = true;
        }

        /// <summary>
        /// Gets the largest possible integer that
        /// can be evenly divided into the provided integer.
        /// The return value must be 3 or larger.
        /// </summary>
        /// <param name="n">Integer for which to find the divisor.</param>
        /// <returns>Greatest divisor larger than 3.</returns>
        private int getLargestDivisor(int n)
        {
            int iSmallestDivisor = 1;

            for (int i = 3; i < n / 2; i++)
            {
                if (n % i == 0)
                    iSmallestDivisor = i;
            }

            return n / iSmallestDivisor;
        }

        public object Clone()
        {
            SudokuBoard newBoard = new SudokuBoard(_defaultMax);

            foreach(var cell in _board)
            {
                if (cell.HasValue())
                    newBoard.SetValue(cell.xIndex, cell.yIndex, cell.Value);
            }

            return newBoard;
        }

        /// <summary>
        /// Determines whether the puzzle on the board is complete.
        /// </summary>
        /// <returns>True if puzzle is complete.</returns>
        private bool isComplete()
        {
            foreach (var cell in _board)
                if (!cell.HasValue())
                    return false;

            this.IsComplete = true;
            return true;
        }
    }
}
