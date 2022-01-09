namespace pdp_lab5.domain.operation;

public class ThreadedKaratsubaMultiplication : IMultiplication
{
    private int MAX_DEPTH = 2;
    private SequentialKaratsubaMultiplication seq = new SequentialKaratsubaMultiplication();

    public Polynomial Run(Polynomial p1, Polynomial p2)
    {
        return Karatsuba(p1, p2, 1);
    }

    private Polynomial Karatsuba(Polynomial p1, Polynomial p2, int currentDepth)
    {
        if (currentDepth > MAX_DEPTH)
        {
            return seq.Run(p1, p2);
        }

        if (p1.Degree < 2 || p2.Degree < 2)
        {
            return seq.Run(p1, p2);
        }

        int len = Math.Max(p1.Degree, p2.Degree) / 2;
        var lowP1 = new Polynomial(p1.Coefficients.GetRange(0, len));
        var highP1 = new Polynomial(p1.Coefficients.GetRange(len, p1.Length - len));
        var lowP2 = new Polynomial(p2.Coefficients.GetRange(0, len));
        var highP2 = new Polynomial(p2.Coefficients.GetRange(len, p2.Length - len));
        Task<Polynomial> f1 = Task.Run(() => Karatsuba(lowP1, lowP2, currentDepth + 1));
        Task<Polynomial> f2 = Task.Run(() => Karatsuba(lowP1 + highP1, lowP2 + highP2, currentDepth + 1));
        Task<Polynomial> f3 = Task.Run(() => Karatsuba(highP1, highP2, currentDepth + 1));
        Task.WaitAll(f1, f2, f3);
        var z1 = f1.Result;
        var z2 = f2.Result;
        var z3 = f3.Result;

        var r1 = Polynomial.AddZeros(z3, 2 * len);
        var r2 = Polynomial.AddZeros((z2 - z3) - z1, len);
        var result = (r1 + r2) + z1;
        return result;
    }
}
