using System;
using System.Diagnostics;
using pdp_lab7.domain;
using pdp_lab7.domain.operation;

namespace pdp_lab7
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // var p1 = new Polynomial(Enumerable.Range(0, 300));
            // var p2 = new Polynomial(Enumerable.Range(0, 300));
            // Stopwatch watch;
            // Console.WriteLine("p1: {0}", p1);
            // Console.WriteLine("p2: {0}", p2);
            // Console.WriteLine("p3: {0}", Polynomial.AddZeros(p1, 10));
            // Console.WriteLine("p1+p2: {0}", p1 + p2);
            // Console.WriteLine("p1-p2: {0}", p1 - p2);
            // Console.WriteLine("p2-p1: {0}", p2 - p1);
            // watch = new Stopwatch();
            // watch.Start();
            // IMultiplication mult = new SequentialMultiplication();
            // watch.Stop();
            // Console.WriteLine("p1*p2(seq): {1}\n{0}", mult.Run(p1, p2), watch.Elapsed);
            // watch = new Stopwatch();
            // watch.Start();
            // mult = new SimpleThreadMultiplication();
            // watch.Stop();
            // Console.WriteLine("p1*p2(simple thread): {1}\n{0}", mult.Run(p1, p2), watch.Elapsed);
            // watch = new Stopwatch();
            // watch.Start();
            // mult = new SequentialKaratsubaMultiplication();
            // watch.Stop();
            // Console.WriteLine("p1*p2(karatsuba): {1}\n{0}", mult.Run(p1, p2), watch.Elapsed);
            // watch = new Stopwatch();
            // watch.Start();
            // mult = new ThreadedKaratsubaMultiplication();
            // watch.Stop();
            // Console.WriteLine("p1*p2(threaded karatsuba): {1}\n{0}", mult.Run(p1, p2), watch.Elapsed);
            MPI.Environment.Run(ref args, communicator =>
            {
                Console.WriteLine("Hello, from process number "
                                  + communicator.Rank + " of "
                                  + communicator.Size);
            });
        }

        public static Polynomial BuildResult(Object[] results)
        {
            int degree = ((Polynomial)results[0]).Degree;
            Polynomial result = Polynomial.BuildEmptyPolynomial(degree + 1);
            for (int i = 0; i < result.Coefficients.Count; i++)
            {
                foreach (Object o in results)
                {
                    result.Coefficients[i] += ((Polynomial)o).Coefficients[i];
                }
            }

            return result;
        }
    }
}
