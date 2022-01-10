using System.Drawing;
using Microsoft.VisualBasic.CompilerServices;

namespace final_project;

[Serializable]
public class HoughLine
{
    private double _theta;
    private double _r;

    public HoughLine(double theta, double r)
    {
        this._theta = theta;
        this._r = r;
    }

    public void DrawLine(Bitmap bitmap, Color color)
    {
        int height = bitmap.Height;
        int width = bitmap.Width;

        int houghHeight = (int)(Math.Sqrt(2) * Math.Max(height, width)) / 2;
        float centerX = (float)width / 2;
        float centerY = (float)height / 2;
        double tSin = Math.Sin(_theta);
        double tCos = Math.Cos(_theta);

        if (_theta < Math.PI * 0.25 || _theta > Math.PI * 0.75)
        {
            for (int y = 0; y < height; y++)
            {
                int x = (int)((((_r - houghHeight) - ((y - centerY) * tSin)) / tCos) + centerX);
                if (x < width && x >= 0)
                {
                    bitmap.SetPixel(x, y, color);
                }
            }
        }
        else
        {
            for (int x = 0; x < width; x++)
            {
                int y = (int)((((_r - houghHeight) - ((x - centerX) * tCos)) / tSin) + centerX);
                if (y < height && y >= 0)
                {
                    bitmap.SetPixel(x, y, color);
                }
            }
        }
    }

    public override string ToString()
    {
        return string.Format("HoughLine{theta={0},r={1}}", _theta, _r);
    }
}
