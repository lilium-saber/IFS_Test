using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Silk.NET.OpenGL;
using KdTree;
using KdTree.Math;

namespace AvaloniaApp.Ults.Object;

// file class Voxelizer(float x)
// {
//     private readonly Dictionary<Vector3, bool> _voxeLDictionary = new();
//     private readonly HashSet<Vector3> _occupiedVoxels = [];
//     private readonly float _voxelSize = x;
//
//     internal void Voxelize(List<Vector3> points)
//     {
//         foreach (var voxelPos in points.Select(point => new Vector3(
//                      (float)Math.Floor(point.X / _voxelSize) * _voxelSize,
//                      (float)Math.Floor(point.Y / _voxelSize) * _voxelSize,
//                      (float)Math.Floor(point.Z / _voxelSize) * _voxelSize
//                  )))
//         {
//             _occupiedVoxels.Add(voxelPos);
//         }
//     }
//     
//     internal List<float> GenerateVoxelGeometry()
//     {
//         var vertices = new List<float>();
//         var halfSize = _voxelSize / 2;
//
//         foreach (var voxel in _occupiedVoxels)
//         {
//             for (var x = -1; x <= 1; x += 2)
//             {
//                 for (var y = -1; y <= 1; y += 2)
//                 {
//                     for (var z = -1; z <= 1; z += 2)
//                     {
//                         vertices.Add(voxel.X + halfSize * x);
//                         vertices.Add(voxel.Y + halfSize * y);
//                         vertices.Add(voxel.Z + halfSize * z);
//                     }
//                 }
//             }
//         }
//
//         return vertices;
//     }
//
//     internal List<Vector3> GetVoxelCenters() =>
//         [.._voxeLDictionary.Keys.Select(voxel => voxel + new Vector3(x / 2))];
//     
//     internal List<float> GetVoxelVertices()
//     {
//         List<float> vertices = [];
//         var halfSize = x / 2;
//         foreach (var voxel in _voxeLDictionary.Keys)
//         {
//             // 为每个体素生成立方体的8个顶点
//             for (var x = -1; x <= 1; x += 2)
//             {
//                 for (var y = -1; y <= 1; y += 2)
//                 {
//                     for (var z = -1; z <= 1; z += 2)
//                     {
//                         vertices.Add(voxel.X + halfSize * x);
//                         vertices.Add(voxel.Y + halfSize * y);
//                         vertices.Add(voxel.Z + halfSize * z);
//                     }
//                 }
//             }
//         }
//         return vertices;
//     }
// }

internal class OpenGlScatterObject
{
    private uint _vao;
    private uint _vbo;
    private uint _program;
    private uint _fragmentShader;
    private readonly Random _random = new();
    private List<PointColorType> _colorList;
    
    internal Matrix4x4 Mold { get; set; } = Matrix4x4.Identity;
    internal float Scale { get; set; } = 1.0f;
    internal Vector3 Color { get; set; } = new(139.0f / 255.0f, 69.0f / 255.0f, 19.0f / 255.0f);

    private float[] _sitPoints = [];
    private const string VertexCode = 
        """
        #version 330 core
        layout (location = 0) in vec3 aPos;
        uniform mat4 model;
        uniform mat4 view;
        uniform mat4 projection;
        void main()
        {
            gl_Position = projection * view * model * vec4(aPos, 1.0);
            gl_PointSize = 1.0;
        }
        """;
    private const string FragmentCode =
        """
        #version 330 core
        out vec4 FragColor;
        uniform vec3 objectColor;
        void main()
        {
            FragColor = vec4(objectColor, 1.0);
        }
        """;
    
    // 隐式曲面
    // private List<Vector3> _pointVector3;
    // private float FieldStrength(Vector3 p) => 
    //     _pointVector3.Select(po => Vector3.DistanceSquared(po, p))
    //         .Where(distanceSquared => distanceSquared > 0)
    //         .Sum(distanceSquared => 0.5f * 0.5f / distanceSquared);
    // private List<float> GenerateSurface(int gridSize, float gridSpacing, float threshold)
    // {
    //     List<float> vertices = [];
    //     for (var x = 0; x < gridSize; x++)
    //     {
    //         for (var y = 0; y < gridSize; y++)
    //         {
    //             for (var z = 0; z < gridSize; z++)
    //             {
    //                 var point = new Vector3(x * gridSpacing, y * gridSpacing, z * gridSpacing);
    //                 if (FieldStrength(point) >= threshold)
    //                 {
    //                     vertices.AddRange([point.X, point.Y, point.Z]);
    //                 }
    //             }
    //         }
    //     }
    //     return vertices;
    // }
    
    // IFS
    private void GetPoints(List<X3dIfs> x3dIfs)
    {
        // List<Vector3> pointVector3S = [];
        List<float> points = [];
        var (x, y, z) = (0.0m, 0.0m, 0.0m);
        const decimal zMinOffset = -0.05m;
        const decimal zMaxOffset = 0.05m;
        for (var i = 0; i < 5e4; i++)
        {
            var temp = _random.Next(100);
            foreach (var p in x3dIfs)
            {
                if (temp < p.P)
                {
                    x = p.A * x + p.B * y + p.C * z + p.Ur;
                    y = p.D * x + p.E * y + p.F * z + p.Vr;
                    z = p.G * x + p.H * y + p.K * z + p.Rr;
                    
                    z += (decimal)(_random.NextDouble() * (double)(zMaxOffset - zMinOffset));
                    
                    points.AddRange([(float)x, (float)y, (float)z]);
                    // pointVector3S.Add(new((float)x, (float)y, (float)z));
                    break;
                }
                temp -= (int)p.P;
            }
        }
        Console.WriteLine("All point has done");
        
        // 体素化
        // Voxelizer voxelizer = new(0.1f);
        // voxelizer.Voxelize(pointVector3S);
        // _sitPoints = [..voxelizer.GenerateVoxelGeometry()];
        
        // 隐式曲面
        // _pointVector3 = pointVector3S;
        // _sitPoints = [..GenerateSurface(5, 0.1f, 1.0f)];
        
        // 邻接三角形
        // KdTree<float, int> kdTree = new(3, new FloatMath());
        // for (var i = 0; i < pointVector3S.Count; i++)
        // {
        //     kdTree.Add([pointVector3S[i].X, pointVector3S[i].Y, pointVector3S[i].Z], i);
        // }
        //
        // foreach (var node in kdTree)
        // {
        //     var neighbors = kdTree.GetNearestNeighbours(node.Point, 3 + 1);
        //     if (neighbors.Length < 3) continue;
        //     for (var i = 1; i < neighbors.Length - 1; i++)
        //     {
        //         points.AddRange([node.Point[0], node.Point[1], node.Point[2], 
        //             neighbors[i].Point[0], neighbors[i].Point[1], neighbors[i].Point[2], 
        //             neighbors[i + 1].Point[0], neighbors[i + 1].Point[1], neighbors[i + 1].Point[2]]);
        //     }
        // }
        
        _sitPoints = [..points];
    }

    internal unsafe void LoadObject(ref GL gl)
    {
        _vao = gl.GenVertexArray();
        _vbo = gl.GenBuffer();
        gl.BindVertexArray(_vao);
        gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);
        fixed (float* p = _sitPoints)
        {
            gl.BufferData(BufferTargetARB.ArrayBuffer, (uint)(_sitPoints.Length * sizeof(float)), p, BufferUsageARB.StaticDraw);
        }
        var vertexShader = gl.CreateShader(ShaderType.VertexShader);
        gl.ShaderSource(vertexShader, VertexCode);
        gl.CompileShader(vertexShader);
        _fragmentShader = gl.CreateShader(ShaderType.FragmentShader);
        gl.ShaderSource(_fragmentShader, FragmentCode);
        gl.CompileShader(_fragmentShader);
        gl.GetShader(vertexShader, GLEnum.CompileStatus, out var status);
        if (status != (int)GLEnum.True)
        {
            var infoLog = gl.GetShaderInfoLog(vertexShader);
            Console.WriteLine($"other Vertex shader compile error: {infoLog}");
        }
        _program = gl.CreateProgram();
        gl.AttachShader(_program, vertexShader);
        gl.AttachShader(_program, _fragmentShader);
        gl.LinkProgram(_program);
        gl.GetProgram(_program, GLEnum.LinkStatus, out status);
        if (status != (int)GLEnum.True)
        {
            var infoLog = gl.GetProgramInfoLog(_program);
            Console.WriteLine($"other Program link error: {infoLog}");
        }
        gl.DetachShader(_program, vertexShader);
        gl.DetachShader(_program, _fragmentShader);
        gl.DeleteShader(vertexShader);
        gl.DeleteShader(_fragmentShader);
        
        const uint positionLoc = 0;
        gl.EnableVertexAttribArray(positionLoc);
        gl.VertexAttribPointer(positionLoc, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), (void*)0);
        
        gl.BindVertexArray(0);
        gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
        gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);
    }
    
    internal unsafe void RenderObject(ref GL gl, Matrix4x4 view, Matrix4x4 projection)
    {
        gl.UseProgram(_program);
        gl.BindVertexArray(_vao);

        Scale = 0.8f;
        var model = Matrix4x4.CreateScale(Scale) * Mold;
        var modelLoc = gl.GetUniformLocation(_program, "model");
        var viewLoc = gl.GetUniformLocation(_program, "view");
        var projectionLoc = gl.GetUniformLocation(_program, "projection");
        gl.UniformMatrix4(modelLoc, 1, false, (float*)&model);
        gl.UniformMatrix4(viewLoc, 1, false, (float*)&view);
        gl.UniformMatrix4(projectionLoc, 1, false, (float*)&projection);

        var objColor = Color;
        var objectColorLoc = gl.GetUniformLocation(_program, "objectColor");
        gl.Uniform3(objectColorLoc, 1, (float*)&objColor);
        
        gl.PointSize(1.5f);
        gl.DrawArrays(PrimitiveType.Points, 0, (uint)(_sitPoints.Length / 3));
        gl.BindVertexArray(0);
    }

    internal OpenGlScatterObject(List<X3dIfs> x3dIfs, List<PointColorType> colors)
    {
        _colorList = colors;
        GetPoints(x3dIfs);
    }
    
}