using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Silk.NET.OpenGL;
using StbImageSharp;

namespace AvaloniaApp.Ults.Object;

internal class OtherObject
{
    internal uint Vao { get; set; }
    internal uint Vbo { get; set; }
    internal uint Program { get; set; }
    internal uint FragmentShader { get; set; }
    internal uint Texture { get; set; }
    internal Matrix4x4 Mold { get; set; } = Matrix4x4.Identity;
    internal float MoldScala { get; set; } = 1.0f;
    internal Vector3 ObjectColor { get; set; } = new(139.0f / 255.0f, 69.0f / 255.0f, 19.0f / 255.0f);
    
    private readonly Random _random = new();

    private float[] _sitPoints =
        [];
    
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

    private List<float> GenerateCylinder(Vector3 start, Vector3 end, float startRadius, float endRadius, int segments)
    {
        List<float> vertices = [];
        var direction = Vector3.Normalize(end - start);
        var up = Vector3.UnitY;
        // if (Vector3.Dot(direction, up) > 0.99f) up = Vector3.UnitX; // 防止方向与Y轴平行
        var right = Vector3.Normalize(Vector3.Cross(up, direction));
        up = Vector3.Cross(direction, right);

        for (var i = 0; i <= segments; i++)
        {
            var angle = MathF.PI * 2 * i / segments;
            var x = MathF.Cos(angle);
            var y = MathF.Sin(angle);

            var offsetStart = right * x * startRadius + up * y * startRadius;
            var offsetEnd = right * x * endRadius + up * y * endRadius;

            var vertexStart = start + offsetStart;
            var vertexEnd = end + offsetEnd;
            
            vertices.AddRange([vertexStart.X, vertexStart.Y, vertexStart.Z, vertexEnd.X, vertexEnd.Y, vertexEnd.Z]);
        }
        return vertices;
    }
    
    private void GeneratePlantGeometry()
    {
        List<float> plantVertices = [];
        var points = _sitPoints;
        const float baseRadius = 0.1f;

        var maxY = _sitPoints
            .Select((value, index) => new { value, index })
            .GroupBy(_ => _.index / 3)
            .Max(_ => _.ElementAt(1).value);
        
        for (var i = 0; i < points.Length / 3 - 1; i++)
        {
            Vector3 start = new(points[i * 3], points[i * 3 + 1], points[i * 3 + 2]);
            Vector3 end = new(points[(i + 1) * 3], points[(i + 1) * 3 + 1], points[(i + 1) * 3 + 2]);
            // var startRadius = baseRadius * (1.0f - i * 0.1f);
            var startRadius = baseRadius * (1.0f - start.Y / maxY);
            // var endRadius = baseRadius * (1.0f - (i + 1) * 0.1f);

            plantVertices.AddRange(GenerateCylinder(start, end, startRadius, 0, 20));
        }

        _sitPoints = [..plantVertices]; 
    }

    private void GetPoints()
    {
        List<X3dIfs> x3dIfs = 
            [new(0.0m, 0.0m, 0.0m, 0.0m, 0.0m, 0.2m, 0.0m, 0.0m, 0.0m, 0.0m, 0.2m, 0.0m, 5),
            new(0.1m, 0.0m, 0.0m, 0.0m, 0.0m, 0.7m, 0.0m, -0.2m, 0.0m, 0.0m, 0.7m, 0.0m, 45),
            new(0.1m, 0.0m, 0.0m, 0.0m, 0.0m, 0.7m, 0.0m, 0.2m, 0.0m, 0.0m, 0.7m, 0.0m, 45),
            new(0.7m, 0.0m, 0.0m, 0.0m, 0.0m, 0.7m, 0.0m, 0.0m, 0.0m, 0.0m, 0.7m, 0.2m, 5)];
        // x3dIfs = TransCodeX3D.Tree1;
        // x3dIfs = TransCodeX3D.Tree2;
        // x3dIfs = TransCodeX3D.Tree3;
        List<float> points = [];
        const decimal zMinOffset = -0.05m;
        const decimal zMaxOffset = 0.05m;
        
        var (x, y, z) = (0.0m, 0.0m, 0.0m);
        for (var i = 0; i < 1e4; i++)
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
                    break;
                }
                else
                {
                    temp -= (int)p.P;
                }
            }
        }

        _sitPoints = [0, 0, 0, ..points];
    }
    
    private unsafe void LoadTexture(ref GL gl, string pngPath, int pictureType = 0)
    {
        if (!File.Exists(pngPath))
        {
            Console.WriteLine($"Texture file not found: {pngPath}");
            return;
        }
        Console.WriteLine($"{pngPath} Loading texture...");
        Texture = gl.GenTexture();
        gl.BindTexture(TextureTarget.Texture2D, Texture);
        gl.TexParameter(TextureTarget.Texture2D, GLEnum.TextureWrapS, (int)TextureWrapMode.Repeat);
        gl.TexParameter(TextureTarget.Texture2D, GLEnum.TextureWrapT, (int)TextureWrapMode.Repeat);
        gl.TexParameter(TextureTarget.Texture2D, GLEnum.TextureMinFilter, (int)TextureMinFilter.Linear);
        gl.TexParameter(TextureTarget.Texture2D, GLEnum.TextureMagFilter, (int)TextureMagFilter.Linear);
        
        using var imageStream = File.OpenRead(pngPath);
        var image = pictureType switch
        {
            1 => ImageResult.FromStream(imageStream),
            _ => ImageResult.FromStream(imageStream, ColorComponents.RedGreenBlueAlpha)
        };
        // 反转图像数据
        var width = image.Width;
        var height = image.Height;
        var channels = (int)image.Comp;
        var reversedData = new byte[image.Data.Length];
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width * channels; x++)
            {
                reversedData[(height - 1 - y) * width * channels + x] = image.Data[y * width * channels + x];
            }
        }

        fixed (byte* data = reversedData)
        {
            gl.TexImage2D(TextureTarget.Texture2D, 0, (int)InternalFormat.Rgba, 
                (uint)image.Width, (uint)image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data);
        }
        
        gl.GenerateMipmap(TextureTarget.Texture2D);
    }

    internal unsafe void LoadObject(ref GL gl, string? texturePath = null)
    {
        GetPoints();
        // _sitPoints =
        // [
        //     .._sitPoints
        //         .Select((value, index) => new { value, index })
        //         .GroupBy(_ => _.index / 3)
        //         .OrderBy(_ => _.ElementAt(1).value)
        //         .SelectMany(_ => _.Select(_ => _.value))
        // ];
        // _sitPoints = [.._sitPoints.Skip(150)];
        // GeneratePlantGeometry();
        Console.WriteLine($"points count: {_sitPoints.Length / 3}");
        Vao = gl.GenVertexArray();
        Vbo = gl.GenBuffer();
        gl.BindVertexArray(Vao);
        gl.BindBuffer(BufferTargetARB.ArrayBuffer, Vbo);
        fixed (float* p = _sitPoints)
        {
            gl.BufferData(BufferTargetARB.ArrayBuffer, (uint)(_sitPoints.Length * sizeof(float)), p, BufferUsageARB.StaticDraw);
        }
        var vertexShader = gl.CreateShader(ShaderType.VertexShader);
        gl.ShaderSource(vertexShader, VertexCode);
        gl.CompileShader(vertexShader);
        FragmentShader = gl.CreateShader(ShaderType.FragmentShader);
        gl.ShaderSource(FragmentShader, FragmentCode);
        gl.CompileShader(FragmentShader);
        gl.GetShader(vertexShader, GLEnum.CompileStatus, out var status);
        if (status != (int)GLEnum.True)
        {
            var infoLog = gl.GetShaderInfoLog(vertexShader);
            Console.WriteLine($"other Vertex shader compile error: {infoLog}");
        }
        Program = gl.CreateProgram();
        gl.AttachShader(Program, vertexShader);
        gl.AttachShader(Program, FragmentShader);
        gl.LinkProgram(Program);
        gl.GetProgram(Program, GLEnum.LinkStatus, out status);
        if (status != (int)GLEnum.True)
        {
            var infoLog = gl.GetProgramInfoLog(Program);
            Console.WriteLine($"other Program link error: {infoLog}");
        }
        gl.DetachShader(Program, vertexShader);
        gl.DetachShader(Program, FragmentShader);
        gl.DeleteShader(vertexShader);
        gl.DeleteShader(FragmentShader);
        
        const uint positionLoc = 0;
        gl.EnableVertexAttribArray(positionLoc);
        gl.VertexAttribPointer(positionLoc, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), (void*)0);
        
        gl.BindVertexArray(0);
        gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
        gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);
    }
    
    internal unsafe void RenderObject(ref GL gl, Matrix4x4 view, Matrix4x4 projection, Vector3? lightPos = null, Vector3? lightColor = null, Vector3? viewPos = null)
    {
        gl.UseProgram(Program);
        gl.BindVertexArray(Vao);

        MoldScala = 0.5f;
        var model = Matrix4x4.CreateScale(MoldScala) * Mold;
        var modelLoc = gl.GetUniformLocation(Program, "model");
        var viewLoc = gl.GetUniformLocation(Program, "view");
        var projectionLoc = gl.GetUniformLocation(Program, "projection");
        gl.UniformMatrix4(modelLoc, 1, false, (float*)&model);
        gl.UniformMatrix4(viewLoc, 1, false, (float*)&view);
        gl.UniformMatrix4(projectionLoc, 1, false, (float*)&projection);

        var objColor = ObjectColor;
        var objectColorLoc = gl.GetUniformLocation(Program, "objectColor");
        gl.Uniform3(objectColorLoc, 1, (float*)&objColor);
        
        gl.PointSize(1.5f);
        gl.DrawArrays(PrimitiveType.Points, 0, (uint)(_sitPoints.Length / 3));
        gl.BindVertexArray(0);
    }
}