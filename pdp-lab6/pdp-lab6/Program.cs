using System.Diagnostics;
using pdp_lab6.domain;

namespace pdp_lab6;

static class Program
{
    private const int NrGraphs = 50;

    static void Main()
    {
        for (int i = 1; i <= NrGraphs; i++)
        {
            DirectedGraph graph = DirectedGraph.GenerateRandomHamiltonian(i * 10); // nr of vertices
            Test(graph);
        }
    }

    private static void Test(DirectedGraph graph)
    {
        var watch = new Stopwatch();
        watch.Start();
        Find(graph);
        watch.Stop();
        Console.WriteLine(graph.Size + " vertices: " + watch.ElapsedMilliseconds + " ms");
    }

    private static void Find(DirectedGraph graph)
    {
        var cancellation = new CancellationTokenSource();
        var tasks = new List<Task>();
        var result = new List<int>(graph.Size + 1);
        var @lock = new object();
        for (int i = 0; i < graph.Size; i++)
        {
            var finder = new CycleFinder(graph, i, result, cancellation, @lock);
            tasks.Add(Task.Run(finder.Run, cancellation.Token));
        }

        try
        {
            Task.WaitAll(tasks.ToArray());
        }
        catch (OperationCanceledException)
        {
        }
        catch (AggregateException e) when (e.InnerException is OperationCanceledException castedException)
        {
        }

        Console.WriteLine("[{0}]", string.Join(", ", result));
    }

    public static IEnumerable<T> Randomize<T>(this IEnumerable<T> source)
    {
        Random rnd = new Random();
        return source.OrderBy((_) => rnd.Next());
    }
}
