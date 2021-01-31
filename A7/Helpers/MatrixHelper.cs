using A7.DataStructures;
using A7.Extensions;
using A7.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace A7.Helpers
{
    public static class MatrixHelper
    {
        /// <summary>
        /// Sukuria generuojančią pagal M ir R parametrus.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="r"></param>
        /// <returns>Generuojanti matrica</returns>
        public static Matrix CreateGeneratorMatrix(int m, int r)
        {
            // Skaičiuojame matricos eilučių bei stulpelių skaičių.
            var colsCount = (int)Math.Pow(2, m);
            var rowsCount = m.CalculateCombinations(r).Sum();
            var matrix = new Matrix(new int[rowsCount, colsCount]);


            // Generuojame trumpuosius vektorius, kurių bus 2^m
            var shortVectors = GenerateOrderedBinaryVectors(m);

            // Bus nulinis vektorius + m vektoriai.
            var iterator = 0;
            foreach (var set in m.CalculateCombinations(r))
            {
                // V0 visada turi būti lygus vienetiniam vektoriui.
                if (iterator == 0)
                {
                    matrix.SetRow(Enumerable.Repeat(1, colsCount).ToArray(), iterator);
                }
                else
                {
                    // Matrica yra užpildoma pagrindiniais vektoriais. Jų skaičius bus lygus M.
                    if (iterator == 1)
                    {
                        for (var i = 0; i < set; i++)
                        {
                            var nextRowToFill = matrix.GetNextRowToFill();
                            var mainVector = ReturnMainVector(shortVectors, i);
                            matrix.SetRow(mainVector, nextRowToFill);
                        }
                    }
                    else
                    {
                        // Jeigu iterator == 1, bus grąžintos "viengubos daugybos" - tiesiog pagrindiniai vektoriai. Pvz.: 1, 2, 3
                        // Jeigu iterator == 2, bus grąžintos visos įmanomos "dvigubos daugybos". Pvz.: 1:2, 1:3, 2:3
                        // Ir tt.
                        var multiplicationSubsets = GetVectorMultiplicationSubset(shortVectors, iterator);
                        foreach (var multiplicationSubset in multiplicationSubsets)
                        {
                            // Užpildau vienetais panašiai kaip prieš paprastą daugybą kintamajam priskiriame vienetą.
                            var initialVector = Enumerable.Repeat(1, colsCount).ToArray();
                            foreach (var value in multiplicationSubset)
                            {
                                initialVector = initialVector.MultiplyVector(matrix.GetRow(value));
                            }

                            var nextRowToFill = matrix.GetNextRowToFill();
                            matrix.SetRow(initialVector, nextRowToFill);
                        }
                    }
                }

                iterator++;
            }

            return matrix;
        }

        /// <summary>
        /// Algoritmas leidžiantis surasti binarinius vektorius surikiuotas nuo mažiausio iki didžiausio.
        /// </summary>
        /// <param name="length"></param>
        /// <returns>Sąrašas binarinių vektorių</returns>
        public static List<int[]> GenerateOrderedBinaryVectors(int length)
        {
            var ordered = new List<int[]>();
            var combinations = Math.Pow(2, length);

            if (combinations == 1)
            {
                return new List<int[]>();
            }

            for (var i = 0; i < combinations; i++)
            {
                var binaryString = Convert.ToString(i, 2).PadLeft(length, '0');
                var binaryStringArr = ConversionService.ConvertBinaryStringToVector(binaryString);
                ordered.Add(binaryStringArr);
                // comb = 4 
                // 000
                // 001
                // 010
                // 011
                // 100
            }

            return ordered;
        }

        /// <summary>
        /// Grąžina pagrindinį vektorių, kurio tam tikros koordinatės sutampa su trumpųjų vektorių 0 koordinate. Pvz.: v1, v2 ir tt.
        /// </summary>
        /// <param name="shortVectors"></param>
        /// <param name="positionToCompare"></param>
        /// <returns>Pagrindinis vektorius</returns>
        public static int[] ReturnMainVector(List<int[]> shortVectors, int positionToCompare)
        {
            var mainVector = new int[shortVectors.Count];

            var iterator = 0;
            foreach (var vector in shortVectors)
            {
                if (vector.ElementAt(positionToCompare) == 0)
                {
                    mainVector[iterator] = 1;
                }
                else
                {
                    mainVector[iterator] = 0;
                }

                iterator++;
            }

            return mainVector;
        }

        /// <summary>
        /// Naudojantis sugeneruotais mažais vektoriukais, gauname tų vektorių poaibį priklausomai nuo multiplicationGroupSize parametro.
        /// Iš visų kombinacijų pagal parametrą galima gauti tam tikras grupes: vektorių viengubas, dvigubas, trigubas ir tt. daugybas.
        /// </summary>
        /// <param name="combinations"></param>
        /// <param name="multiplicationGroupSize"></param>
        /// <returns>Sąrašas n-gubos daugybos vektorių "porų"</returns>
        public static List<List<int>> GetVectorMultiplicationSubset(List<int[]> combinations, int multiplicationGroupSize)
        {
            // 00 
            // 01 -> 1 poz -> 2
            // 10 -> 0 poz -> 1
            // 11 -> 1 2
            var allCombinations = new List<List<int>>();
            foreach (var combination in combinations)
            {
                var combinationValues = new List<int>();
                for (var i = 0; i < combination.Length; i++)
                {
                    if (combination.ElementAt(i) == 1)
                    {
                        combinationValues.Add(i + 1);
                    }
                }

                if (combinationValues.Count == multiplicationGroupSize)
                {
                    allCombinations.Add(combinationValues);
                }
            }

            allCombinations.Reverse();

            return allCombinations;
        }
    }
}
