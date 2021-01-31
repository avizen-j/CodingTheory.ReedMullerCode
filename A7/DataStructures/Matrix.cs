using System;
using System.Linq;

namespace A7.DataStructures
{
    public class Matrix
    {
        private int _nextRowToFill = 0;
        public int[,] Values { get; private set; }

        public Matrix()
        {

        }

        public Matrix(int[,] values)
        {
            Values = values;
        }

        /// <summary>
        /// Gauna matricos eilutę pagal eilutės indeksą.
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <returns>Matricos eilutė</returns>
        public int[] GetRow(int rowIndex)
        {
            return Enumerable.Range(0, Values.GetLength(1))
                             .Select(r => Values[rowIndex, r])
                             .ToArray();
        }

        /// <summary>
        /// Skaičiuoja matricos eilučių skaičių
        /// </summary>
        /// <returns>Matricos eilučių skaičius</returns>
        public int GetRowsCount()
        {
            return Values.GetLength(0);
        }

        /// <summary>
        /// Gauna pirmos neužpildytos matricos eilutės indeksą (nuo pradžios).
        /// </summary>
        /// <returns>Eilutės indeksas</returns>
        public int GetNextRowToFill()
        {
            return _nextRowToFill;
        }

        /// <summary>
        /// Užpildo matricos eilutę sveikųjų skačių masyvu.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="rowIndex"></param>
        public void SetRow(int[] values, int rowIndex)
        {
            var colsCount = GetColsCount();
            if (colsCount != values.Length)
            {
                throw new InvalidOperationException($"Cannot insert longer {values.Length} int array than matrix row {colsCount}");
            }

            var rowsCount = GetRowsCount();
            if (rowIndex >= rowsCount)
            {
                throw new InvalidOperationException($"Cannot insert longer {values.Length} int array than matrix row {colsCount}");
            }

            for (var col = 0; col < colsCount; col++)
            {
                Values[rowIndex, col] = values[col];
            }

            _nextRowToFill++;
        }

        /// <summary>
        /// Gauna matricos stulpelį pagal stulpelio indeksą.
        /// </summary>
        /// <param name="colNumber"></param>
        /// <returns>Matricos stulpelis</returns>
        public int[] GetCol(int colNumber)
        {
            return Enumerable.Range(0, Values.GetLength(0))
                             .Select(r => Values[r, colNumber])
                             .ToArray();
        }

        /// <summary>
        /// Skaičiuoja matricos stulpelių skaičių
        /// </summary>
        /// <returns>Matricos stulpelių skaičius</returns>
        public int GetColsCount()
        {
            return Values.GetLength(1);
        }

        /// <summary>
        /// Spausdina matricą į konsolę.
        /// </summary>
        public void PrintMatrix()
        {
            var rows = GetRowsCount();
            for (var i = 0; i < rows; i++)
            {
                var row = GetRow(i);
                Console.WriteLine(string.Join(" ", row));
            }
        }
    }
}
