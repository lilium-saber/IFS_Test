using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Controls;
using Avalonia.Threading;
using Silk.NET.OpenGL;

namespace AvaloniaApp.OpenGLUse;

public class SILKOpenGLControl : OpenGlControlBase
{
    private readonly float VHeight = 400f;
    private readonly float VWidth = 400f;
    private readonly float VMaxDepth = 2000f;
    private readonly float VMinDepth = 0.1f;
    private uint _vertexArray;
    private uint _vertexBuffer;
    private uint _shaderProgram;
    private uint _axisVBO;
    private GL _gl;
    private List<MathNet.Numerics.LinearAlgebra.Vector<float>> _points;
    private readonly float[] _axisVertices =
    [
        // X轴
        0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, // 红色
        1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, // 红色
        // Y轴
        0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, // 绿色
        0.0f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f, // 绿色
        // Z轴
        0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, // 蓝色
        0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 1.0f  // 蓝色
    ];
    
    private void CheckShaderCompileStatus(uint shader)
    {
        _gl.GetShader(shader, ShaderParameterName.CompileStatus, out var status);
        if (status == 0)
        {
            var infoLog = _gl.GetShaderInfoLog(shader);
            Console.WriteLine($"Shader compile error: {infoLog}");
        }
    }

    private void CheckProgramLinkStatus(uint program)
    {
        _gl.GetProgram(program, ProgramPropertyARB.LinkStatus, out var status);
        if (status == 0)
        {
            var infoLog = _gl.GetProgramInfoLog(program);
            Console.WriteLine($"Program link error: {infoLog}");
        }
    }
    
    public SILKOpenGLControl()
    {
        _points = [];
    }
    
    public SILKOpenGLControl(List<MathNet.Numerics.LinearAlgebra.Vector<float>> points)
    {
        float maxX = points.Max(p => Math.Abs(p[0]));
        float maxY = points.Max(p => Math.Abs(p[1]));
        float scaleFactorX = VWidth / 2 / maxX;
        float scaleFactorY = VHeight / 2 / maxY;
        float scaleFactor = Math.Min(scaleFactorX, scaleFactorY);
        _points = [..points.Select(_ => _ * scaleFactor)];
        Console.WriteLine($"SILKOpenGLControl points count: {_points.Count}");
    }
    
    protected override unsafe void OnOpenGlInit(GlInterface gl)
    {
        _gl = GL.GetApi(gl.GetProcAddress);
        _gl.ClearColor(0.2f, 0.3f, 0.4f, 1.0f);
        
        _vertexArray = _gl.GenVertexArray();
        _gl.BindVertexArray(_vertexArray);
        _vertexBuffer = _gl.GenBuffer();
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vertexBuffer);
        
        float[] vertices = [.._points.SelectMany(_ => _.ToArray())];
        _gl.BufferData<float>(BufferTargetARB.ArrayBuffer, vertices, BufferUsageARB.StaticDraw);
        
        const string vertexShaderSource = @"
        #version 100
        attribute vec3 aPos;
        uniform mat4 projection;
        uniform mat4 view;
        void main()
        {
            gl_Position = projection * view * vec4(aPos, 1.0);
        }";
        const string fragmentShaderSource = @"
        #version 100
        precision mediump float;
        void main()
        {
            gl_FragColor = vec4(1.0, 0.5, 0.2, 1.0);
        }";
        
        var vertexShader = _gl.CreateShader(ShaderType.VertexShader);
        _gl.ShaderSource(vertexShader, vertexShaderSource);
        _gl.CompileShader(vertexShader);
        CheckShaderCompileStatus(vertexShader);

        var fragmentShader = _gl.CreateShader(ShaderType.FragmentShader);
        _gl.ShaderSource(fragmentShader, fragmentShaderSource);
        _gl.CompileShader(fragmentShader);
        CheckShaderCompileStatus(fragmentShader);
        
        _shaderProgram = _gl.CreateProgram();
        _gl.AttachShader(_shaderProgram, vertexShader);
        _gl.AttachShader(_shaderProgram, fragmentShader);
        _gl.LinkProgram(_shaderProgram);
        CheckProgramLinkStatus(_shaderProgram);
        
        _gl.DeleteShader(vertexShader);
        _gl.DeleteShader(fragmentShader);
        
        _gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        _gl.EnableVertexAttribArray(0);
        
        var projection = Matrix4x4.CreateOrthographic(VWidth, VHeight, VMinDepth, VMaxDepth);
        var view = Matrix4x4.CreateLookAt(new Vector3(0, 0, (int)VWidth), Vector3.Zero, Vector3.UnitY);

        var projectionLocation = _gl.GetUniformLocation(_shaderProgram, "projection");
        var viewLocation = _gl.GetUniformLocation(_shaderProgram, "view");

        _gl.UseProgram(_shaderProgram);
        _gl.UniformMatrix4(projectionLocation, 1, false, &projection.M11);
        _gl.UniformMatrix4(viewLocation, 1, false, &view.M11);
        
        _axisVBO = _gl.GenBuffer();
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _axisVBO);
        _gl.BufferData<float>(BufferTargetARB.ArrayBuffer, _axisVertices, BufferUsageARB.StaticDraw);
    }

    protected override void OnOpenGlRender(GlInterface gl, int fb)
    {
        _gl.Clear((uint)ClearBufferMask.ColorBufferBit | (uint)ClearBufferMask.DepthBufferBit);
        // _gl.ClearColor(0.2f, 0.3f, 0.4f, 1.0f);
        
        _gl.PointSize(10.0f);

        // 在这里添加你的 OpenGL 渲染代码
        _gl.UseProgram(_shaderProgram);
        _gl.BindVertexArray(_vertexArray);
        _gl.DrawArrays(PrimitiveType.Points, 0, (uint)_points.Count);
        
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _axisVBO);
        _gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
        _gl.EnableVertexAttribArray(0);
        _gl.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
        _gl.EnableVertexAttribArray(1);
        _gl.DrawArrays(PrimitiveType.Lines, 0, 6);
        
        // 请求下一帧渲染
        Dispatcher.UIThread.Post(RequestNextFrameRendering, DispatcherPriority.Render);
    }
    
    protected override void OnOpenGlDeinit(GlInterface gl)
    {
        // 清理 OpenGL 资源
        _gl.DeleteVertexArray(_vertexArray);
        _gl.DeleteBuffer(_vertexBuffer);
        _gl.DeleteBuffer(_axisVBO);
        _gl.DeleteProgram(_shaderProgram);
        _gl.Dispose();
    }
}