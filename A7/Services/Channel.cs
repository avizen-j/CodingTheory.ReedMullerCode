using System;
using System.Collections.Generic;

namespace A7.Services
{
    public static class Channel
    {
        private static readonly Random Random = new Random();

        /// <summary>
        /// Simuliuoja siuntimą kanalu iškraipant vektoriaus koordinates. 
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="distortionProbability"></param>
        /// <returns>Iškraipytas vektorius</returns>
        public static int[] Send(int[] vector, double distortionProbability)
        {
            var distortedVector = new int[vector.Length];

            for (var i = 0; i < vector.Length; i++)
            {
                var rnd = Random.NextDouble();
                var positionBit = vector[i];
                var reversedBit = positionBit == 0 ? 1 : 0;
                distortedVector[i] = rnd < distortionProbability ? reversedBit : vector[i];
            }

            return distortedVector;
        }

        /// <summary>
        /// Simuliuoja siuntimą kanalu iškraipant vektoriaus koordinates išskyrus paskutinius bitus, kurie nešą "tarnybinę" informaciją.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="distortionProbability"></param>
        /// <param name="bitsToSkip"></param>
        /// <returns>Iškraipytas vektorius</returns>
        public static int[] Send(int[] vector, double distortionProbability, int bitsToSkip)
        {
            var distortedVector = new int[vector.Length];

            for (var i = 0; i < vector.Length; i++)
            {
                var actualBit = vector[i];
                if (i >= vector.Length - bitsToSkip)
                {
                    distortedVector[i] = actualBit;
                }
                else
                {
                    var rnd = Random.NextDouble();
                    var reversedBit = actualBit == 0 ? 1 : 0;
                    distortedVector[i] = rnd < distortionProbability ? reversedBit : actualBit;
                }
            }

            return distortedVector;
        }

        /// <summary>
        /// Skaičiuoja pozicijas, kur įvyko klaidos.
        /// </summary>
        /// <param name="originalVector"></param>
        /// <param name="distortedVector"></param>
        /// <returns>Vektoriaus pozicijos</returns>
        public static List<int> GetErrornousPositions(int[] originalVector, int[] distortedVector)
        {
            var errornousPositions = new List<int>();
            for (var i = 0; i < originalVector.Length; i++)
            {
                if (originalVector[i] != distortedVector[i])
                {
                    errornousPositions.Add(i);
                }
            }

            return errornousPositions;
        }

        /// <summary>
        /// Pakeičia vektoriuje pasirinktas pozicijas keičiant jas į atvirkštines.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="positionsToReverse"></param>
        /// <returns></returns>
        public static int[] FixDistortedVector(int[] vector, List<int> positionsToReverse)
        {
            for (var i = 0; i < positionsToReverse.Count; i++)
            {
                var actualBit = vector[positionsToReverse[i]];
                vector[positionsToReverse[i]] = actualBit == 0 ? 1 : 0;
            }

            return vector;
        }
    }
}
