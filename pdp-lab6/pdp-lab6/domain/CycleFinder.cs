namespace pdp_lab6.domain;

public class CycleFinder
{
    private DirectedGraph graph;
    private int startingNode;
    private List<int> path;
    private object @lock;
    private List<int> result;
    private CancellationTokenSource cancel;

    public CycleFinder(DirectedGraph graph, int node, List<int> result, CancellationTokenSource cancel, object @lock)
    {
        this.graph = graph;
        this.startingNode = node;
        this.path = new List<int>();
        this.@lock = @lock;
        this.cancel = cancel;
        this.result = result;
    }

    public void Run()
    {
        Visit(startingNode);
    }

    private void Visit(int node)
    {
        cancel.Token.ThrowIfCancellationRequested();
        path.Add(node);
        if (path.Count == graph.Size)
        {
            if (graph.NeighboursOf(node).Contains(startingNode))
            {
                cancel.Cancel();
                lock (@lock)
                {
                    result.Clear();
                    result.AddRange(path);
                }
            }

            path.RemoveAt(path.Count - 1);
            return;
        }

        graph.NeighboursOf(node).ForEach(neighbour =>
        {
            if (!path.Contains(neighbour))
            {
                Visit(neighbour);
            }
        });
        path.RemoveAt(path.Count - 1);
    }
}
