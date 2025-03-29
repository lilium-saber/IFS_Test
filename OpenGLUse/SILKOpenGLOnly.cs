using System;
using System.Collections.Generic;
using System.Drawing;
using AvaloniaApp.Ults;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace AvaloniaApp.OpenGLUse;

public class SILKOpenGLOnly
{
    private GL _gl;
    private IWindow _window;
    private uint _vao; // 顶点数组对象
    private uint _vbo; // 顶点缓冲对象 
    private uint _ebo; // 索引缓冲对象
    private List<uint> _program; // 着色器程序
    private List<uint> _fragmentShader; // 片段着色器
    private readonly List<float> _colors = 
        [
            1.0f, 0.5f, 0.2f, 1.0f, 
            0.0f, 0.75f, 0.0f, 1.0f
        ];
    private readonly List<Vector<float>> _points;
    
    private void KeyDown(IKeyboard keyboard, Key key, int code)
    {
        Console.WriteLine($"input key: {key}");
        if (key == Key.Escape)
        {
            _window.Close();
        }
    }
        
    private unsafe void OnRender(double temp)
    {
        _gl.Clear(ClearBufferMask.ColorBufferBit);
        _gl.BindVertexArray(_vao);
        
        // 绑定EBO
        _gl.UseProgram(_program[0]);
        _gl.DrawElements(PrimitiveType.Triangles, 3, DrawElementsType.UnsignedInt, (void*) 0);
        
        _gl.UseProgram(_program[1]);
        _gl.DrawElements(PrimitiveType.Triangles, 3, DrawElementsType.UnsignedInt, (void*)(3 * sizeof(uint)));
    }

    private unsafe void OnLoad()
    {
        var input = _window.CreateInput();
        foreach (var t in input.Keyboards)
            t.KeyDown += KeyDown;
        
        // 创建OpenGL上下文绑定到窗口。 如果离屏渲染如avalonia控件使用GetApi()方法
        _gl = _window.CreateOpenGL();
        
        _gl.ClearColor(Color.LightSkyBlue);
        
        // 开始渲染
        // 顶点缓冲区
        _vao = _gl.GenVertexArray();
        _gl.BindVertexArray(_vao);
        float[] vertices =
        [
            0.5f,  0.5f, 0.0f,
            0.5f, -0.5f, 0.0f,
            0, 0, 0,
            -0.5f, -0.5f, 0.0f,
            -0.5f,  0.5f, 0.0f
        ];
        _vbo = _gl.GenBuffer();
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo); //ArrayBuffer: 顶点缓冲区目标, ElementArrayBuffer: 数组缓冲区目标, UniformBuffer: 统一缓冲区目标
        fixed (float* buf = vertices)
        {
            _gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(vertices.Length * sizeof(float)), buf, BufferUsageARB.StaticDraw); //StaticDraw- 设置一次数据, DynamicDraw- 多次更新数据
        }
        // 元素缓冲区EBO or 索引缓冲区
        uint[] indices =
        [
            0u, 1u, 2u,
            4u, 2u, 3u
        ]; // vertex索引, 当前程序中是三角形
        _ebo = _gl.GenBuffer();
        _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _ebo);
        fixed (uint* buf = indices)
        {
            _gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(indices.Length * sizeof(uint)), buf, BufferUsageARB.StaticDraw);
        }
        
        // 创建片段着色器
        uint vertexShader = 0;
        OpenGLFunc.CreateVertexShader(ref _gl, ref vertexShader);
        var fragmentCode = OpenGLFunc.ChangeColor(_colors[0], _colors[1], _colors[2], _colors[3]);
        OpenGLFunc.CreateFragmentShader(ref _gl, ref _fragmentShader, fragmentCode);
        var fragmentCode2 = OpenGLFunc.ChangeColor(_colors[4], _colors[5], _colors[6], _colors[7]);
        OpenGLFunc.CreateFragmentShader(ref _gl, ref _fragmentShader, fragmentCode2);
        
        // 创建着色器程序
        OpenGLFunc.CreateShaderProgram(ref _gl, ref _program, ref vertexShader, ref _fragmentShader, 0);
        OpenGLFunc.CreateShaderProgram(ref _gl, ref _program, ref vertexShader, ref _fragmentShader, 1);
        
        // 删除着色器
        OpenGLFunc.DeleteShader(ref _gl, ref _program, ref _fragmentShader, ref vertexShader);
        
        // 绑定顶点属性
        const uint positionLoc = 0;
        _gl.EnableVertexAttribArray(positionLoc);
        _gl.VertexAttribPointer(positionLoc, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), (void*) 0);
        
        // 解绑缓冲区
        _gl.BindVertexArray(0);
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
        _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);
    }
    
    private void OnUpdate(double delta)
    {
        
    }
    
    // 状态机
    private void StartOpenGl()
    {
        var options = WindowOptions.Default;
        options.Size = new Vector2D<int>(800, 800);
        options.Title = "SILKOpenGLExample";
        _window = Silk.NET.Windowing.Window.Create(options);

        _window.Load += OnLoad;
        _window.Update += OnUpdate;
        _window.Render += OnRender;
        _window.Run();
    }

    public SILKOpenGLOnly()
    {
        _points = [];
        _program = [];
        _fragmentShader = [];
        Console.WriteLine("No Points!");
    }

    public SILKOpenGLOnly(List<Vector<float>> points)
    {
        _points = points;
        _program = [];
        _fragmentShader = [];
    }

    public void PubStartOpenGl()
    {
        StartOpenGl();
    }
    
}