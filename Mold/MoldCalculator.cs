using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using SixLabors.ImageSharp;
using DenseMatrix = MathNet.Numerics.LinearAlgebra.Double.DenseMatrix;
using Vector = MathNet.Numerics.LinearAlgebra.Single.Vector;

namespace IFS_line.Mold;

internal class MoldCalculator3D
{
    internal static DenseMatrix GetMatrixZoom(double xScale = 1, double yScale = 1, double zScale = 1)
    {
        return DenseMatrix.OfArray(new double[,]
        {
            { xScale, 0, 0 },
            { 0, yScale, 0 },
            { 0, 0, zScale }
        });
    }

    internal static DenseMatrix GetRotationMatrixX(double angle = Math.PI)
    {
        return DenseMatrix.OfArray(new double[,]
        {
            { 1, 0, 0 },
            { 0, Math.Cos(angle), -Math.Sin(angle) },
            { 0, Math.Sin(angle), Math.Cos(angle) }
        });
    }
    
    internal static DenseMatrix GetRotationMatrixY(double angle = Math.PI)
    {
        return DenseMatrix.OfArray(new double[,]
        {
            { Math.Cos(angle), 0, Math.Sin(angle) },
            { 0, 1, 0 },
            { -Math.Sin(angle), 0, Math.Cos(angle) }
        });
    }
    
    internal static DenseMatrix GetRotationMatrixZ(double angle = Math.PI)
    {
        return DenseMatrix.OfArray(new double[,]
        {
            { Math.Cos(angle), -Math.Sin(angle), 0 },
            { Math.Sin(angle), Math.Cos(angle), 0 },
            { 0, 0, 1 }
        });
    }
        
    internal static Vector<double> GetMatrixCalculateRes(Vector<double> transPlus, ref Vector<double> start,
        double xAngle = Math.PI, double yAngle = Math.PI, double zAngle = Math.PI, double xScale = 1, double yScale = 1, double zScale = 1)
    {
        var temp =  (GetMatrixZoom(xScale, yScale, zScale) * GetRotationMatrixX(xAngle) * GetRotationMatrixY(yAngle) * GetRotationMatrixZ(zAngle)) * start + transPlus;
        start = temp;
        return temp;
    }
    
    internal static List<Vector<double>> GetMatrixListRes(List<(double xAngle, double yAngle, double zAngle, double xScale, double yScale, double zScale, double p)> transformations,
        Vector<double> start, List<Vector<double>> transPlus, int counts = (int)1e3)
    {
        List<Vector<double>> points = [];
        var temp = new Random();
        
        for (int i = 0; i < counts; i++)
        {
            var rand = temp.NextDouble() * 100;
            foreach (var (xAngle, yAngle, zAngle, xScale, yScale, zScale, p) in transformations)
            {
                if (rand < p)
                {
                    points.Add(GetMatrixCalculateRes(transPlus[i], ref start, xAngle, yAngle, zAngle, xScale, yScale, zScale));
                    break;
                }
                else
                {
                    rand -= p;
                }
            }
        }
        
        return points;
    }
}