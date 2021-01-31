namespace A7.Extensions
{
    public static class IntegerArrayExtensions
    {
        /// <summary>
        /// Vektorių daugyba
        /// </summary>
        /// <param name="firstVector"></param>
        /// <param name="secondVector"></param>
        /// <returns>Sudaugintas vektorius</returns>
        public static int[] MultiplyVector(this int[] firstVector, int[] secondVector)
        {
            var multiplied = new int[firstVector.Length];
            for (var i = 0; i < firstVector.Length; i++)
            {
                multiplied[i] = firstVector[i] & secondVector[i];
            }

            return multiplied;
        }

        /// <summary>
        /// Vektorių atimtis
        /// </summary>
        /// <param name="firstVector"></param>
        /// <param name="secondVector"></param>
        /// <returns>Vektorius po atimties</returns>
        public static int[] SubtractVector(this int[] firstVector, int[] secondVector)
        {
            var subtractedVector = new int[firstVector.Length];
            for (var i = 0; i < firstVector.Length; i++)
            {
                subtractedVector[i] = firstVector[i] == secondVector[i] && (firstVector[i] == 0 || firstVector[i] == 1) ? 0 : 1;
            }

            return subtractedVector;
        }
    }
}
