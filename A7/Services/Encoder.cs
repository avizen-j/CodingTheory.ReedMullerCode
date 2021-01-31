using A7.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace A7.Services
{
    public static class Encoder
    {
        /// <summary>
        /// Užkoduoja vektorių dauginant pradinį vektorių iš generuojančios matricos.
        /// </summary>
        /// <param name="initialVector"></param>
        /// <param name="generatorMatrix"></param>
        /// <returns></returns>
        public static int[] EncodeVector(int[] initialVector, Matrix generatorMatrix)
        {
            var rowsCount = generatorMatrix.GetRowsCount();
            var colsCount = generatorMatrix.GetColsCount();
            var multipliedRowsByScalar = new List<int[]>();

            for (var i = 0; i < initialVector.Length; i++)
            {
                var bit = initialVector[i];
                var matrixRow = generatorMatrix.GetRow(i);
                var result = matrixRow.Select(r => r * bit).ToArray();
                multipliedRowsByScalar.Add(result);
            }

            return Enumerable.Range(0, colsCount)
                             .Select(col => multipliedRowsByScalar.Sum(row => row[col]) % 2)
                             .ToArray();
        }
    }
}
