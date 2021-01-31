using A7.DataStructures;
using A7.Extensions;
using A7.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace A7.Services
{
    public static class Decoder
    {
        private static readonly Random Random = new Random();

        /// <summary>
        /// Dekoduoja užkoduota stringą pagal Reedo Mullerio algoritmą.
        /// </summary>
        /// <param name="encodedVector"></param>
        /// <param name="generatorMatrix"></param>
        /// <param name="m"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static int[] DecodeVector(int[] encodedVector, Matrix generatorMatrix, int m, int r)
        {
            // Generuojame trumpuosius vektorius, kurių bus 2^m
            var shortVectors = MatrixHelper.GenerateOrderedBinaryVectors(m);
            var singleMS = MatrixHelper.GetVectorMultiplicationSubset(shortVectors, 1).Select(x => x.First()).ToList();
            var matrixRowIterator = generatorMatrix.GetRowsCount();
            var coefficients = new List<int>();

            // Pradedam nuo pabaigos.
            for (var i = r; i >= 0; i--)
            {
                // Paimame visas r-gubas sandaugas
                var multiplicationSubsets = MatrixHelper.GetVectorMultiplicationSubset(shortVectors, i);
                multiplicationSubsets.Reverse();
                var subsetsCoefficients = new List<int>(multiplicationSubsets.Count);

                foreach (var multiplicationSubset in multiplicationSubsets)
                {
                    // Kiekvienai r-gubų sandaugų porai pradedame algoritmą
                    // Surandame L - nepanaudoti indeksus
                    var unusedIndices = singleMS.Except(multiplicationSubset).ToList();

                    // Sugeneruojam 2^(M-R) T trumpuosius vektoriukus, kurie bus M-R ilgio
                    var tCount = (int)Math.Pow(2, m - i);
                    var tShortVectors = MatrixHelper.GenerateOrderedBinaryVectors(m - i);

                    // Raktas - w žodžio indeksas, Reikšmė - w žodžio reikšmė.
                    var dictionary = new Dictionary<int[], int[]>(tCount);

                    // Atvejis, kai m==r
                    if (!tShortVectors.Any())
                    {
                        dictionary.Add(new int[0], Enumerable.Repeat(1, shortVectors.Count).ToArray());
                    }
                    else
                    {
                        for (var j = 0; j < tCount; j++)
                        {
                            var words = new List<int[]>();
                            for (var index = 0; index < unusedIndices.Count; index++)
                            {
                                var lCoordinate = unusedIndices[index] - 1;
                                words.Add(shortVectors.Select(x => x[lCoordinate] == tShortVectors[j][index])
                                                      .Select(boolean => boolean ? 1 : 0)
                                                      .ToArray());
                            }

                            // Words sąrašas susideda iš žodžių: žodis kur l1 koordinatėje yra t1 pirmas elementas, žodis kur l2 koordinatėje yra t1 antras elementas ir tt.
                            // Metodas žemiau sujungia žodžius į vieną logical and pagalba.
                            var matchingWord = Enumerable.Repeat(1, shortVectors.Count).ToArray();
                            FindMatchingWord(words, ref matchingWord);
                            dictionary.Add(tShortVectors[j], matchingWord);
                        }

                    }

                    var coef = FindCoefficient(dictionary, encodedVector);
                    subsetsCoefficients.Add(coef);
                    coefficients.Add(coef);
                }

                matrixRowIterator -= subsetsCoefficients.Count();
                encodedVector = UpdateEncodedVector(subsetsCoefficients, generatorMatrix, matrixRowIterator, encodedVector);
            }

            coefficients.Reverse();
            return coefficients.ToArray();
        }

        /// <summary>
        /// Pritaiko AND funkcija visiems žodžiams (skirtas sąlygos apjungimui).
        /// </summary>
        /// <param name="words"></param>
        /// <param name="matchingWord"></param>
        private static void FindMatchingWord(List<int[]> words, ref int[] matchingWord)
        {
            foreach (var word in words)
            {
                matchingWord = word.MultiplyVector(matchingWord);
            }
        }

        /// <summary>
        /// Atnaujina kodo vektorių surandant padaugintą iš rastų koeficientų vektorių ir atimant jį iš buvusio. Kitaip: iš C padaro C', iš C' - C'' ir tt.
        /// </summary>
        /// <param name="subsetsCoefficients"></param>
        /// <param name="generatorMatrix"></param>
        /// <param name="matrixRowIterator"></param>
        /// <param name="encodedVector"></param>
        /// <returns>Atnaujintas kodo vektorius</returns>
        private static int[] UpdateEncodedVector(List<int> subsetsCoefficients, Matrix generatorMatrix, int matrixRowIterator, int[] encodedVector)
        {
            var multipliedRowsByScalar = new List<int[]>();
            var coefIndex = 0;
            var lastMatrixRow = matrixRowIterator + subsetsCoefficients.Count;
            var matrixColsCount = generatorMatrix.GetColsCount();
            subsetsCoefficients.Reverse();
            for (var rowIndex = matrixRowIterator; rowIndex < lastMatrixRow; rowIndex++)
            {
                // Multiply by scalar and add
                var row = generatorMatrix.GetRow(rowIndex);
                var multipliedRow = row.Select(r => r * subsetsCoefficients[coefIndex]).ToArray();
                multipliedRowsByScalar.Add(multipliedRow);
                coefIndex++;
            }

            var sum = Enumerable.Range(0, matrixColsCount)
                                .Select(col => multipliedRowsByScalar.Sum(row => row[col]) % 2)
                                .ToArray();

            return encodedVector.SubtractVector(sum);
        }

        /// <summary>
        /// Suranda koeficientą pasinaudojant surastais žodžiais 'w' ir balsavimo būdu nusprendžia galutinę koeficiento reikšmę.
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="encodedVector"></param>
        /// <returns>Matricos eilutės koeficientas</returns>
        private static int FindCoefficient(Dictionary<int[], int[]> dictionary, int[] encodedVector)
        {
            var voteResults = new List<int>();
            foreach (var word in dictionary)
            {
                var multiplied = word.Value.MultiplyVector(encodedVector);
                var sum = multiplied.Sum() % 2;
                voteResults.Add(sum);
            }

            var zeros = voteResults.Count(x => x == 0);
            var ones = voteResults.Count(x => x == 1);

            if (zeros == ones)
            {
                return Random.Next(0, 1);
            }
            else if (zeros > ones)
            {
                return 0;
            }

            return 1;
        }
    }
}
