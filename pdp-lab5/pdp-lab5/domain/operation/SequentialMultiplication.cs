namespace pdp_lab5.domain.operation;

public class SequentialMultiplication : IMultiplication
{
    public Polynomial Run(Polynomial p1, Polynomial p2)
    {
        int sizeOfResultCoefficientList = p1.Degree + p2.Degree + 1;
        List<int> coefficients = new List<int>(Enumerable.Range(0, sizeOfResultCoefficientList).Select(_ => 0));

        foreach ((var valuep1, var indexp1) in p1.Coefficients.Select(((valuep1, indexp1) => (valuep1, indexp1))))
        {
            foreach ((var valuep2, var indexp2) in p2.Coefficients.Select(((valuep2, indexp2) => (valuep2, indexp2))))
            {
                int index = indexp1 + indexp2;
                int value = valuep1 * valuep2;
                coefficients[index] += value;
            }
        }

        return new Polynomial(coefficients);
    }
}
