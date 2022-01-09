using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pdp_lab7.domain
{
    [Serializable()]
    public class Polynomial
    {
        private List<int> coefficients;
        private int degree;

        public int Length => this.coefficients.Count;
        public List<int> Coefficients => this.coefficients;
        public int Degree => this.degree;

        public Polynomial(IEnumerable<int> coefficients)
        {
            this.coefficients = new List<int>(coefficients);
            this.degree = this.coefficients.Count - 1;
        }

        public Polynomial(int degree)
        {
            this.degree = degree;
            coefficients = new List<int>(degree + 1);
            GenerateCoefficients();
        }

        private void GenerateCoefficients()
        {
            Random random = new Random();
            int MAX_VALUE = 10;
            for (int i = 0; i < degree; i++)
            {
                coefficients.Add(random.Next(MAX_VALUE));
            }

            coefficients.Add(random.Next(MAX_VALUE) + 1);
        }

        public static Polynomial AddZeros(Polynomial p, int offset)
        {
            var coefficients = Enumerable.Range(0, offset).Select(i => 0);
            var newCoef = coefficients.Concat(p.Coefficients);
            return new Polynomial(newCoef);
        }

        public static Polynomial BuildEmptyPolynomial(int degree)
        {
            List<int> zeros = Enumerable.Range(0, degree).Select(i => 0).ToList();
            return new Polynomial(zeros);
        }

        public static Polynomial operator +(Polynomial p1, Polynomial p2)
        {
            int minDegree = Math.Min(p1.Degree, p2.Degree);
            int maxDegree = Math.Max(p1.Degree, p2.Degree);
            List<int> coefficients = new List<int>(maxDegree + 1);
            for (int i = 0; i <= minDegree; i++)
            {
                coefficients.Add(p1.Coefficients[i] + p2.Coefficients[i]);
            }

            AddRemainingCoefficients(p1, p2, minDegree, maxDegree, coefficients);
            return new Polynomial(coefficients);
        }

        public static Polynomial operator -(Polynomial p1, Polynomial p2)
        {
            int minDegree = Math.Min(p1.Degree, p2.Degree);
            int maxDegree = Math.Max(p1.Degree, p2.Degree);
            List<int> coefficients = new List<int>(maxDegree + 1);
            int i;
            for (i = 0; i <= minDegree; i++)
            {
                coefficients.Add(p1.Coefficients[i] - p2.Coefficients[i]);
            }

            SubtractRemainingCoefficients(p1, p2, minDegree, maxDegree, coefficients);
            i = coefficients.Count - 1;
            while (coefficients[i] == 0 && i > 0)
            {
                coefficients.RemoveAt(i);
                i--;
            }

            return new Polynomial(coefficients);
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            int power = 0;
            for (int i = 0; i <= this.degree; i++)
            {
                // if (coefficients[i] == 0)
                // {
                //     power++;
                //     continue;
                // }

                str.Append(" ").Append(coefficients[i]).Append("x^").Append(power).Append(" +");
                power++;
            }

            str.Remove(str.Length - 1, 1);
            return str.ToString();
        }

        private static void AddRemainingCoefficients(Polynomial p1, Polynomial p2, int minDegree, int maxDegree,
            List<int> coefficients)
        {
            if (minDegree != maxDegree)
            {
                if (maxDegree == p1.Degree)
                {
                    for (int i = minDegree + 1; i <= maxDegree; i++)
                    {
                        coefficients.Add(p1.Coefficients[i]);
                    }
                }
                else
                {
                    for (int i = minDegree + 1; i <= maxDegree; i++)
                    {
                        coefficients.Add(p2.Coefficients[i]);
                    }
                }
            }
        }

        private static void SubtractRemainingCoefficients(Polynomial p1, Polynomial p2, int minDegree, int maxDegree,
            List<int> coefficients)
        {
            if (minDegree != maxDegree)
            {
                if (maxDegree == p1.Degree)
                {
                    for (int i = minDegree + 1; i <= maxDegree; i++)
                    {
                        coefficients.Add(p1.Coefficients[i]);
                    }
                }
                else
                {
                    for (int i = minDegree + 1; i <= maxDegree; i++)
                    {
                        coefficients.Add(-p2.Coefficients[i]);
                    }
                }
            }
        }
    }
}
