using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using DenseMatrix = MathNet.Numerics.LinearAlgebra.Single.DenseMatrix;
namespace AvaloniaApp.Mold;

public class MoldCalculator
{
    internal static DenseMatrix GetMatrixZoom(float xScale = 1, float yScale = 1, float zScale = 1)
    {
        return DenseMatrix.OfArray(new[,]
        {
            { xScale, 0, 0 },
            { 0, yScale, 0 },
            { 0, 0, zScale }
        });
    }

    internal static DenseMatrix GetRotationMatrixX(float angle = MathF.PI)
    {
        return DenseMatrix.OfArray(new[,]
        {
            { 1, 0, 0 },
            { 0, MathF.Cos(angle), -MathF.Sin(angle) },
            { 0, MathF.Sin(angle), MathF.Cos(angle) }
        });
    }
    
    internal static DenseMatrix GetRotationMatrixY(float angle = MathF.PI)
    {
        return DenseMatrix.OfArray(new[,]
        {
            { MathF.Cos(angle), 0, MathF.Sin(angle) },
            { 0, 1, 0 },
            { -MathF.Sin(angle), 0, MathF.Cos(angle) }
        });
    }
    
    internal static DenseMatrix GetRotationMatrixZ(float angle = MathF.PI)
    {
        return DenseMatrix.OfArray(new[,]
        {
            { MathF.Cos(angle), -MathF.Sin(angle), 0 },
            { MathF.Sin(angle), MathF.Cos(angle), 0 },
            { 0, 0, 1 }
        });
    }
        
    // Vector is MathNet.Numerics.LinearAlgebra.Vector<float>
    internal static Vector<float> GetMatrixCalculateRes(ref Vector<float> start,
        float xAngle = MathF.PI, float yAngle = MathF.PI, float zAngle = MathF.PI, float xScale = 1, float yScale = 1, float zScale = 1, float TX = 0, float TY = 0, float TZ = 0)
    {
        var transPlus = MathNet.Numerics.LinearAlgebra.Single.Vector.Build.DenseOfArray([TX, TY, TZ]);
        var temp =  (GetMatrixZoom(xScale, yScale, zScale) * GetRotationMatrixX(xAngle) * GetRotationMatrixY(yAngle) * GetRotationMatrixZ(zAngle)) * start + transPlus;
        start = temp;
        return temp;
    }
    
    // Vector is MathNet.Numerics.LinearAlgebra.Vector<float>
    // ReSharper disable once RedundantNameQualifier
    public static List<MathNet.Numerics.LinearAlgebra.Vector<float>> GetMatrixListRes(List<(float xAngle, float yAngle, float zAngle, float xScale, float yScale, float zScale, float transPlusX, float transPlusY, float transPlusZ, float p)> transformations,
        Vector<float> start, int counts = (int)1e3)
    {
        List<Vector<float>> points = [];
        var temp = new Random();
        var startTemp = start;
        
        for (var i = 0; i < counts; i++)
        {
            var rand = temp.NextSingle() * 100;
            foreach (var (xAngle, yAngle, zAngle, xScale, yScale, zScale, TX, TY, TZ, p) in transformations)
            {
                if (rand < p)
                {
                    // points.Add(GetMatrixCalculateRes( ref start, xAngle, yAngle, zAngle, xScale, yScale, zScale, TX, TY, TZ));
                    var transzPlue = Vector.Build.DenseOfArray([TX, TY, TZ]);
                    startTemp = (GetMatrixZoom(xScale, yScale, zScale) * GetRotationMatrixX(xAngle) * GetRotationMatrixY(yAngle) * GetRotationMatrixZ(zAngle)) * startTemp + transzPlue;
                    points.Add(startTemp);
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

    public static List<MathNet.Numerics.LinearAlgebra.Vector<float>> GetSimpleRes(List<(float a, float b, float c, float d, float e, float f, float g, float h, float k, float u, float v, float r, float p)> simpleTrans,
        Vector<float> start, int counts = (int)1e3)
    {
        List<Vector<float>> points = [];
        var temp = new Random();
        var startTemp = start;

        for (var i = 0; i < counts; i++)
        {
            var rand = temp.NextSingle() * 100;
            foreach (var (a, b, c, d, e, f, g, h, k, u, v, r, p) in simpleTrans)
            {
                if (rand < p)
                {
                    var transPlus = MathNet.Numerics.LinearAlgebra.Single.Vector.Build.DenseOfArray([u, v, r]);
                    var trans = DenseMatrix.OfArray(new[,]
                    {
                        { a, b, c },
                        { d, e, f },
                        { g, h, k }
                    });
                    startTemp = trans * startTemp + transPlus;
                    points.Add(startTemp);
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