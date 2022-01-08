using System.Collections;

namespace pdp_lab5.domain.operation;

public class SimpleThreadMultiplication : IMultiplication
{
    void multiply(int index, Polynomial p1, Polynomial p2, List<int> coefficients, object @lock)
    {
        for (int j = 0; j < p2.Length; j++)
        {
            int newIndex = index + j;
            int value = p1.Coefficients[index] * p2.Coefficients[j];
            lock (@lock)
            {
                coefficients[newIndex] += value;
            }
        }
    }

    public Polynomial Run(Polynomial p1, Polynomial p2)
    {
        int sizeOfResultCoefficientList = p1.Degree + p2.Degree + 1;
        var coefficients = new List<int>(Enumerable.Range(0, sizeOfResultCoefficientList).Select(_ => 0));
        var tasks = new List<Task>();
        var coeffLock = new object();
        for (int i = 0; i < p1.Length; i++)
        {
            var index = i;
            tasks.Add(Task.Run(() => multiply(index, p1, p2, coefficients, coeffLock)));
        }

        Task.WaitAll(tasks.ToArray());
        return new Polynomial(coefficients);
    }
}
