using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hci_tetris.Processing
{
    public class Playfield
    {
        private readonly int[,] field;
        public int Rows { get; }
        public int Columns { get; }

        public int this[int row, int column]
        {
            get => field[row, column];
            set => field[row, column] = value;
        }

        public Playfield(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            field = new int[rows, columns];
        }

        public bool IsInside(int row, int column)
        {
            return row >= 0 && row < Rows && column >= 0 && column < Columns;
        }

        public bool IsEmpty(int row, int column)
        {
            return IsInside(row, column) && field[row, column] == 0;
        }

        public bool IsRowFull(int row)
        {
            for (int column = 0; column < Columns; column++)
            {
                if (field[row, column] == 0)
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsRowEmpty(int row)
        {
            for (int column = 0; column < Columns; column++)
            {
                if (field[row, column] != 0)
                {
                    return false;
                }
            }
            return true;
        }

        public void ClearRow(int row)
        {
            for (int column = 0; column < Columns; column++)
            {
                field[row, column] = 0;
            }
        }

        private void MoveRowDown(int row, int numRows)
        {
            for (int column = 0; column < Columns; column++)
            {
                field[row + numRows, column] = field[row, column];
                field[row, column] = 0;
            }
        }

        public int ClearFullRows()
        {
            int cleared = 0;

            for (int row = Rows - 1; row >= 0; row--)
            {
                if (IsRowFull(row))
                {
                    ClearRow(row);
                    cleared++;
                }
                else if (cleared > 0)
                {
                    MoveRowDown(row, cleared);
                }
            }

            return cleared;
        }
    }
}
