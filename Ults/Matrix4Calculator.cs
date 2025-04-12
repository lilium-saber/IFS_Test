using System;
using System.Numerics;

namespace AvaloniaApp.Ults;

internal static class Matrix4Calculator
{
    internal static Matrix4x4 GetStandardMatrixX4() =>
        new Matrix4x4(
            1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1);
    
    internal static Matrix4x4 CreateMatrixX4(float x, float y, float z, float k) =>
        new Matrix4x4(
            x, 0, 0, 0,
            0, y, 0, 0,
            0, 0, z, 0,
            0, 0, 0, k);
    

    internal static Matrix4x4 RotationX(float degrees, Matrix4x4 matrix) =>
        Matrix4x4.CreateRotationX(MathF.PI * degrees / 180.0f) * matrix;
    
    internal static Matrix4x4 RotationY(float degrees, Matrix4x4 matrix) => 
        Matrix4x4.CreateRotationY(MathF.PI * degrees / 180.0f) * matrix;
    
    internal static Matrix4x4 RotationZ(float degrees, Matrix4x4 matrix) => 
        Matrix4x4.CreateRotationZ(MathF.PI * degrees / 180.0f) * matrix;
    

    internal static Matrix4x4 Scale(Matrix4x4 matrix, float x, float y, float z) =>
        Matrix4x4.CreateScale(x, y, z) * matrix;
    
    // fovDegrees: 视场角, aspectRatio: 纵横比, nearPlane: 近距离, farPlane: 远距离
    internal static Matrix4x4 CreatePerspective(float fovDegrees, float aspectRatio, float nearPlane, float farPlane) =>
        Matrix4x4.CreatePerspectiveFieldOfView((MathF.PI * fovDegrees / 180.0f), aspectRatio, nearPlane, farPlane);
    
    // 角度转弧度
    internal static float GetRadians(float degrees) => degrees * MathF.PI / 180.0f;
    
    // 弧度转角度
    internal static float GetDegrees(float radians) => (float)(radians * 180.0 / MathF.PI);
}