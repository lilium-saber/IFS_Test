using System;
using System.Numerics;

namespace AvaloniaApp.Ults;

internal class Matrix4Calculator
{
    internal static Matrix4x4 GetStandardMatrixX4()
    {
        return new Matrix4x4(
            1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1);
    }
    
    internal static Matrix4x4 CreateMatrixX4(float x, float y, float z, float k)
    {
        return new Matrix4x4(
            x, 0, 0, 0,
            0, y, 0, 0,
            0, 0, z, 0,
            0, 0, 0, k);
    }

    internal static Matrix4x4 RotationX(float degrees, Matrix4x4 matrix)
    {
        return Matrix4x4.CreateRotationX(MathF.PI * degrees / 180.0f) * matrix;
    }
    
    internal static Matrix4x4 RotationY(float degrees, Matrix4x4 matrix)
    {
        return Matrix4x4.CreateRotationY(MathF.PI * degrees / 180.0f) * matrix;
    }
    
    internal static Matrix4x4 RotationZ(float degrees, Matrix4x4 matrix)
    {
        return Matrix4x4.CreateRotationZ(MathF.PI * degrees / 180.0f) * matrix;
    }

    internal static Matrix4x4 Scale(Matrix4x4 matrix, float x, float y, float z)
    {
        return Matrix4x4.CreateScale(x, y, z) * matrix;
    }
}