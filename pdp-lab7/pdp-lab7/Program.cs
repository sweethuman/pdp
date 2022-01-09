using System;
using System.Diagnostics;
using MPI;
using pdp_lab7.domain;
using pdp_lab7.domain.operation;

namespace pdp_lab7
{
    internal class Program
    {
        private const string Multiplication = "Simple";
        private const int PolynomialOrder = 5;

        private static void multiplicationMaster(Polynomial p, Polynomial q, int nrProcs, String type,
            Intracommunicator comm)
        {
            var watch = new Stopwatch();
            int start, finish = 0;
            int len = p.Length / (nrProcs - 1);

            for (int i = 1; i < nrProcs; i++)
            {
                start = finish;
                finish += len;
                if (i == nrProcs - 1)
                {
                    finish = p.Length;
                }

                comm.Send(p, i, 0);
                comm.Send(q, i, 0);

                comm.Send(start, i, 0);
                comm.Send(finish, i, 0);
            }

            var polys = new List<Polynomial>();
            for (int i = 1; i < nrProcs; i++)
            {
                polys.Add(comm.Receive<Polynomial>(i, 0));
            }

            Polynomial result = BuildResult(polys);
            watch.Stop();
            Console.WriteLine("Result: {0}\nExecution time: {1}ms", result, watch.Elapsed.Milliseconds);
        }

        private static void multiplyKaratsubaWorker(int me, Intracommunicator comm)
        {
            Console.WriteLine("Worker {0} started", me);
            Polynomial p = comm.Receive<Polynomial>(0, 0);
            Polynomial q = comm.Receive<Polynomial>(0, 0);
            int begin = comm.Receive<int>(0, 0);
            int end = comm.Receive<int>(0, 0);

            for(int i=0;i<begin; i++)
            {
                p.Coefficients[i] = 0;
            }

            for (int j = end; j < p.Coefficients.Count; j++)
            {
                p.Coefficients[j] = 0;
            }

            var result = new SequentialKaratsubaMultiplication().Run(p, q);
            comm.Send(result, 0, 0);
        }

        private static void multiplySimpleWorker(int me, Intracommunicator comm)
        {
            Console.WriteLine("Worker {0} started", me);
            Polynomial p = comm.Receive<Polynomial>(0, 0);
            Polynomial q = comm.Receive<Polynomial>(0, 0);
            int begin = comm.Receive<int>(0, 0);
            int end = comm.Receive<int>(0, 0);

            for(int i=0;i<begin; i++)
            {
                p.Coefficients[i] = 0;
            }

            for (int j = end; j < p.Coefficients.Count; j++)
            {
                p.Coefficients[j] = 0;
            }

            var result = new SequentialMultiplication().Run(p, q, begin, end);
            comm.Send(result, 0, 0);
        }

        static void Main(string[] args)
        {
            MPI.Environment.Run(ref args, communicator =>
            {
                int me = communicator.Rank;
                int nrProcs = communicator.Size;
                if (me == 0)
                {
                    Console.WriteLine("Master process generating polynomials:");
                    Polynomial p = new Polynomial(PolynomialOrder);
                    Polynomial q = new Polynomial(PolynomialOrder);

                    Console.WriteLine(p);
                    Console.WriteLine(q);
                    multiplicationMaster(p, q, nrProcs, Multiplication, communicator);
                }
                else
                {
                    if (Multiplication == "Simple")
                    {
                        multiplySimpleWorker(communicator.Rank, communicator);
                    }
                    else
                    {
                        multiplyKaratsubaWorker(communicator.Rank, communicator);
                    }
                }
            });
        }

        public static Polynomial BuildResult(IList<Polynomial> results)
        {
            int degree = results[0].Degree;
            Polynomial result = Polynomial.BuildEmptyPolynomial(degree + 1);
            for (int i = 0; i < result.Coefficients.Count; i++)
            {
                foreach (var o in results)
                {
                    result.Coefficients[i] += o.Coefficients[i];
                }
            }

            return result;
        }
    }
}
