using System.Collections.Generic;

namespace A7.Extensions
{
    public static class IntegerExtensions
    {
        /// <summary>
        /// Metodas skirtas skaičiaus faktorialui apskaičiuoti.
        /// </summary>
        /// <param name="n"></param>
        /// <returns>Skaičiaus faktorialas</returns>
        public static int CalculateFactorial(this int n)
        {
            int factorial = 1;
            while (n >= 1)
            {
                factorial *= n;
                n -= 1;
            }

            return factorial;
        }

        /// <summary>
        /// Generuojančios matricos vektorių kombinacijos.
        /// Skaičiuojama pagal formulę: 1 + C(1,m) + C(2,m) + ... + C(r,m)
        /// </summary>
        /// <param name="m"></param>
        /// <param name="r"></param>
        /// <returns>Kombinacijos</returns>
        public static IEnumerable<int> CalculateCombinations(this int m, int r)
        {
            for (var i = 0; i <= r; i++)
            {
                yield return m.CalculateFactorial() / (i.CalculateFactorial() * (m - i).CalculateFactorial());
            }
        }

    }
}
