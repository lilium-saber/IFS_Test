using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFS_line.Point
{
    public class PointCalculator : IPointCalculator
    {
        private static readonly Random _random = new();

        private static (decimal x0, decimal y0) GetPointSet(int len, decimal a, decimal b, decimal c, decimal d, decimal e, decimal f,
            ref decimal x, ref decimal y)
        {
            x = a * x + b * y + e;
            y = c * x + d * y + f;
            var (x0, y0) = (x * len, y * len);
            return (x0, y0);
        }

        public static List<(decimal x, decimal y)> GetPointList(List<(decimal a, decimal b, decimal c, decimal d, decimal e, decimal f, decimal p)> transformations,
            int counts = (int)1e5, decimal x = 0, decimal y = 0, int bitmapLenght = 500)
        {
            List<(decimal x, decimal y)> points = [];
            for(int i = 0; i < counts; i++)
            {
                var temp = _random.Next(100) + 1; // [1, 100]
                foreach (var (a, b, c, d, e, f, p) in transformations)
                {
                    if(temp <= (int)p)
                    {
                        points.Add(GetPointSet(bitmapLenght, a, b, c, d, e, f, ref x, ref y));
                        break;
                    }
                    else
                    {
                        temp -= (int)p;
                    }
                }
            }
            return points;
        }

        public static List<(decimal x, decimal y, PointColorType type)> GetPointList(
            List<(decimal a, decimal b, decimal c, decimal d, decimal e, decimal f, decimal p, PointColorType type)>
                transformations,
            int counts = (int)1e5, decimal x = 0, decimal y = 0, int bitmapLenght = 500)
        {
            List<(decimal x, decimal y, PointColorType t)> points = [];
            for(int i = 0; i < counts; i++)
            {
                var temp = _random.Next(100) + 1; // [1, 100]
                foreach (var (a, b, c, d, e, f, p, type) in transformations)
                {
                    if(temp <= (int)p)
                    {
                        var res = GetPointSet(bitmapLenght, a, b, c, d, e, f, ref x, ref y);
                        points.Add((res.x0, res.y0, type));
                        break;
                    }
                    else
                    {
                        temp -= (int)p;
                    }
                }
            }
            return points;
        }
        
    }
}
