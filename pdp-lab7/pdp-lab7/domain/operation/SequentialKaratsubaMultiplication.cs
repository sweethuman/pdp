
using System;

namespace pdp_lab7.domain.operation
{
    public class SequentialKaratsubaMultiplication : IMultiplication
    {
        public Polynomial Run(Polynomial p1, Polynomial p2)
        {
            if (p1.Degree < 2 || p2.Degree < 2)
            {
                return new SequentialMultiplication().Run(p1, p2);
            }

            int len = Math.Max(p1.Degree, p2.Degree) / 2;
            var lowP1 = new Polynomial(p1.Coefficients.GetRange(0, len));
            var highP1 = new Polynomial(p1.Coefficients.GetRange(len, p1.Length - len));
            var lowP2 = new Polynomial(p2.Coefficients.GetRange(0, len));
            var highP2 = new Polynomial(p2.Coefficients.GetRange(len, p2.Length - len));

            var z1 = this.Run(lowP1, lowP2);
            var z2 = this.Run(lowP1 + highP1, lowP2 + highP2);
            var z3 = this.Run(highP1, highP2);

            var r1 = Polynomial.AddZeros(z3, 2 * len);
            var r2 = Polynomial.AddZeros((z2 - z3) - z1, len);
            var result = (r1 + r2) + z1;
            return result;
        }
    }
}
