using System.Drawing;

namespace final_project;

public class HoughTransform
{
    private const double ThetaStep = Math.PI / Config.MaxTheta;
    public readonly int Width;
    public readonly int Height;
    private readonly Image _image;
    private readonly float _centerX, _centerY;
    private readonly int _houghHeight;
    private readonly int _doubledHeight;

    private int _numberOfPoints;
    private int[,] _houghArray;

    private readonly double[] _sinCache;
    private readonly double[] _cosCache;
    public readonly Bitmap Bitmap;

    private readonly object _arrayLock = new object();
    private readonly object _nrPointsLock = new object();
    private object _bitmapLock = new object();

    public HoughTransform(Image image)
    {
        this.Width = image.Width;
        this.Height = image.Height;
        this._image = image;
        Bitmap = new Bitmap(image);
        this._houghHeight = (int)(Math.Sqrt(2) * Math.Max(Height, Width)) / 2;
        _doubledHeight = 2 * _houghHeight;
        _houghArray = new int[Config.MaxTheta, _doubledHeight];
        _centerX = (float)Width / 2;
        _centerY = (float)Height / 2;
        _numberOfPoints = 0;

        _sinCache = new double[Config.MaxTheta];
        _cosCache = new double[Config.MaxTheta];

        for (int t = 0; t < Config.MaxTheta; t++)
        {
            double realTheta = t * ThetaStep;
            _sinCache[t] = Math.Sin(realTheta);
            _cosCache[t] = Math.Cos(realTheta);
        }
    }


    public void AddPoints()
    {
        List<Task> tasks = new List<Task>();
        for (int x = 0; x < _image.Width; x++)
        {
            var xx = x;
            tasks.Add(Task.Run(() =>
            {
                for (int y = 0; y < this.Height; y++)
                {
                    AddPoint(xx, y);
                }
            }));
        }

        Task.WaitAll(tasks.ToArray());
    }

    public void AddPoint(int x, int y)
    {
        Color pixel;
        lock (_bitmapLock)
        {
            pixel = Bitmap.GetPixel(x, y);
        }

        if (pixel.R == 0 && pixel.G == 0 && pixel.B == 0)
        {
            return;
        }

        for (int t = 0; t < Config.MaxTheta; t++)
        {
            int r = (int)(((x - _centerX) * _cosCache[t]) + ((y - _centerY) * _sinCache[t]));
            r += _houghHeight;
            if (r < 0 || r >= _doubledHeight) continue;

            lock (_arrayLock)
            {
                _houghArray[t, r]++;
            }
        }

        lock (_nrPointsLock)
        {
            _numberOfPoints++;
        }
    }

    public List<HoughLine> GetLines()
    {
        List<HoughLine> lines = new List<HoughLine>();
        if (_numberOfPoints == 0) return lines;

        for (int t = 0; t < Config.MaxTheta; t++)
        {
            for (int r = Config.NeighbourhoodSize; r < _doubledHeight - Config.NeighbourhoodSize; r++)
            {
                if (_houghArray[t, r] > Config.Threshold)
                {
                    int peak = _houghArray[t, r];

                    for (int dx = -Config.NeighbourhoodSize; dx <= Config.NeighbourhoodSize; dx++)
                    {
                        for (int dy = -Config.NeighbourhoodSize; dy <= Config.NeighbourhoodSize; dy++)
                        {
                            int dt = t + dx;
                            int dr = r + dy;
                            if (dt < 0) dt += Config.MaxTheta;
                            else if (dt >= Config.MaxTheta) dt -= Config.MaxTheta;
                            if (_houghArray[dt, dr] > peak)
                            {
                                goto end_of_loop;
                            }
                        }
                    }

                    double theta = t * ThetaStep;
                    lines.Add(new HoughLine(theta, r));
                }

                end_of_loop:
                continue;
            }
        }

        return lines;
    }
}
