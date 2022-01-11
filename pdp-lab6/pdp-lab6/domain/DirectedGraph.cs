namespace pdp_lab6.domain;

public class DirectedGraph
{
    private List<List<int>> container;
    private List<int> nodes;

    public DirectedGraph(int nodeCount)
    {
        this.container = new List<List<int>>(nodeCount);
        this.nodes = new List<int>();

        for (int i = 0; i < nodeCount; i++)
        {
            this.container.Add(new List<int>());
            this.nodes.Add(i);
        }
    }

    public void AddEdge(int nodeA, int nodeB)
    {
        this.container[nodeA].Add(nodeB);
    }

    public List<int> NeighboursOf(int node)
    {
        return this.container[node];
    }

    public List<int> Nodes => nodes;

    public int Size => this.container.Count;

    public static DirectedGraph GenerateRandomHamiltonian(int size)
    {
        DirectedGraph graph = new DirectedGraph(size);

        List<int> nodes = graph.Nodes;

        nodes.Randomize();

        for (int i = 1; i < nodes.Count; i++)
        {
            graph.AddEdge(nodes[i - 1], nodes[i]);
        }

        graph.AddEdge(nodes[nodes.Count - 1], nodes[0]);

        Random random = new Random();

        for (int i = 0; i < size / 2; i++)
        {
            int nodeA = random.Next(size - 1);
            int nodeB = random.Next(size - 1);

            graph.AddEdge(nodeA, nodeB);
        }

        return graph;
    }
}
