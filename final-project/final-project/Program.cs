using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using MPI;

namespace final_project;

static class Program
{
    static void Main(string[] args)
    {
        const string file = "assets/vase.png";
        if (args[0] == "mpi")
        {
            HoughMpi(file);
        }
        else if (args[0] == "threads")
        {
            HoughThreads(file);
        }
        else
        {
            Console.WriteLine("Not compatible");
        }
    }

    private static void HoughThreads(String file)
    {
        Console.WriteLine("Running Threads");
        var watch = new Stopwatch();
        var image = Image.FromFile(file);
        var transform = new HoughTransform(image);
        List<Task> tasks = new List<Task>();
        for (int x = 0; x < transform.Width; x++)
        {
            var xx = x;
            tasks.Add(Task.Run(() =>
            {
                for (int y = 0; y < transform.Height; y++)
                {
                    transform.AddPoint(xx, y);
                }
            }));
        }
        Task.WaitAll(tasks.ToArray());
        List<HoughLine> lines = transform.GetLines();

        var bitmap = new Bitmap(image);
        DrawLines(bitmap, lines);
        watch.Stop();
        Console.WriteLine("Thread Performance: {0}ms", watch.Elapsed.Milliseconds);
    }

    private static void DrawLines(Bitmap bitmap, List<HoughLine> lines)
    {
        foreach (var line in lines)
        {
            line.DrawLine(bitmap, Color.Red);
        }

        bitmap.Save("assets/output.png", ImageFormat.Png);
    }


    private static void HoughMpi(string file)
    {
        MPI.Environment.Run(comm =>
        {
            var image = Image.FromFile(file);
            var houghTransform = new HoughTransform(image);
            if (comm.Rank == 0)
            {
                MPIMaster(houghTransform, comm);
            }
            else
            {
                MPIWorker(houghTransform, comm);
            }
        });
    }

    private static void MPIMaster(HoughTransform transform, Intracommunicator comm)
    {
        Console.WriteLine("Starting {0} processes", comm.Size);
        Console.WriteLine("Starting master process {0}", comm.Rank);
        var watch = new Stopwatch();
        int nrProcs = comm.Size;
        int begin = 0;
        int end = transform.Width / (nrProcs - 1);
        for (int i = 1; i < nrProcs; i++)
        {
            List<int> beginList = new List<int>();
            for (int x = begin; x < end; x++)
            {
                beginList.Add(x + (end) * (i - 1));
            }

            comm.Send(beginList, i, 0);
        }

        var lines = new List<HoughLine>();
        for (int i = 1; i < nrProcs; i++)
        {
            var list = comm.Receive<List<HoughLine>>(i, 0);
            lines.AddRange(list);
        }

        DrawLines(transform.Bitmap, lines);
        watch.Stop();
        Console.WriteLine("MPI Performance: {0}ms", watch.Elapsed.Milliseconds);
    }

    private static void MPIWorker(HoughTransform transform, Intracommunicator comm)
    {
        Console.WriteLine("Starting worker process {0}", comm.Rank);
        var columns = comm.Receive<List<int>>(0, 0);
        foreach (var x in columns)
        {
            for (int y = 0; y < transform.Height; y++)
            {
                transform.AddPoint(x, y);
            }
        }

        var lines = transform.GetLines();
        comm.Send(lines, 0, 0);
    }
}
