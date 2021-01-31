using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace A7.Services
{
    public static class ConversionService
    {
        private static readonly Random Random = new Random();

        /// <summary>
        /// Konvertuoja binarinę simbolių eilutę į vektorių.
        /// 48 baitas pagal utf-8 koduotę yra lygus 0, 49 baitas yra lygus 1.
        /// </summary>
        /// <param name="binaryString"></param>
        /// <returns></returns>
        public static int[] ConvertBinaryStringToVector(string binaryString)
        {
            var vector = new List<int>();
            var encoding = new UTF8Encoding();
            var byteArray = encoding.GetBytes(binaryString);

            foreach (var b in byteArray)
            {
                if (b.Equals(48) || b.Equals(49))
                {
                    vector.Add(b - 48);
                }
                else
                {
                    throw new FormatException($"Unexpected element was found: '{b}'");
                }
            }

            return vector.ToArray();
        }

        /// <summary>
        /// Konvertuoja baitų masyvą suskaidydamas jį į atitinkamo ilgio vektorius.
        /// </summary>
        /// <param name="byteArray"></param>
        /// <param name="vectorExpectedLength"></param>
        /// <returns>Vektoriai ir pridėti bitai</returns>
        public static (List<int[]>, int) ConvertByteArrayToVectors(byte[] byteArray, int vectorExpectedLength)
        {
            var vectors = new List<int[]>();

            // Baitų masyvas yra transformuojamas į dvejetainę seką.
            var binaryString = string.Join("", byteArray.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')));

            // Dvejetainė seka yra dalinama į atitinkamo ilgio (expectedLength) vektorius
            var addedBits = 0;
            var splittedBinaryStrings = SplitIntoChunks(binaryString, vectorExpectedLength).ToList();

            // Tikrinama, ar paskutinis vektorius yra teisingo ilgio. Jeigu ne, užpildom nuliais iki reikiamo.
            var lastChunk = splittedBinaryStrings.Last();
            if (lastChunk.Length != vectorExpectedLength)
            {
                splittedBinaryStrings[^1] = lastChunk.PadRight(vectorExpectedLength, '0');
                addedBits = vectorExpectedLength - lastChunk.Length;
            }

            // Konvertuojamas dvejetainę simbolių eilutę į vektorius reikiamo formato.
            splittedBinaryStrings.ForEach(x => vectors.Add(ConvertBinaryStringToVector(x)));

            return (vectors, addedBits);
        }

        /// <summary>
        /// Konvertuoja vektorius į baitų masyvą atimant pridėtus bitus.
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="addedBits"></param>
        /// <returns>Baitų masyvas</returns>
        public static byte[] ConvertVectorsToByteArray(List<int[]> vectors, int addedBits)
        {
            // Vektoriai yra transformuojami į dvejetainę seka be pridėtų bitų.
            var binaryText = string.Join("", vectors.Select(x => string.Join("", x)));
            var initialBinaryText = binaryText.Substring(0, binaryText.Length - addedBits);

            // Konvertuojam dvejetainę seka į baitų masyvą.
            var byteArray = SplitIntoChunks(initialBinaryText, 8).Select(s => Convert.ToByte(s, 2)).ToArray();

            return byteArray;
        }

        /// <summary>
        /// Dalina simbolių eilutę į nurodyto grupes. 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="chunkLength"></param>
        /// <returns>Sąrašas padalintų simbolių eilučių</returns>
        public static IEnumerable<string> SplitIntoChunks(string text, int chunkLength)
        {
            for (int i = 0; i < text.Length; i += chunkLength)
            {
                yield return text.Substring(i, Math.Min(chunkLength, text.Length - i));
            }
        }
    }
}
