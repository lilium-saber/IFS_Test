using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using AvaloniaApp.Ults;
using MathNet.Numerics.LinearAlgebra.Single;
using Silk.NET.GLFW;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using StbImageSharp;

namespace AvaloniaApp.OpenGLUse;

public class SILKOpenGLOnly
{
    private GL _gl;
    private IWindow _window;
    private readonly Camera _camera;
    private uint _vao; // 顶点数组对象
    private uint _vbo; // 顶点缓冲对象 
    private uint _ebo; // 索引缓冲对象
    private uint _texture; // 纹理对象
    private List<uint> _vaos; // 顶点数组对象list
    private List<uint> _vbos; // 顶点缓冲对象list
    private List<uint> _ebos; // 索引缓冲对象list
    private List<uint> _textures;
    private List<uint> _program; // 着色器程序
    private List<uint> _fragmentShader; // 片段着色器
    private readonly List<float> _colors = 
        [
            1.0f, 0.5f, 0, 1.0f, 
            0.0f, 0.75f, 0.0f, 1.0f
        ];
    private readonly List<MathNet.Numerics.LinearAlgebra.Vector<float>> _points;

    private bool useTime = false;
    private float deltaTime = 0.0f;
    private float lastTime = 0.0f;
    
    private float lastX = 0.0f;
    private float lastY = 0.0f;
    private float sensitivity = 0.05f; // 灵敏度
    private float yaw = 0.0f;
    private float pitch = 0.0f;
    private bool firstMouse = true;
    
    private void KeyDown(IKeyboard keyboard, Key key, int code)
    {
        Console.WriteLine($"input key: {key}");
        _camera.cameraSpeed = 2.5f * deltaTime;
        
        switch (key)
        {
            case Key.T:
                useTime = !useTime;
                break;
            case Key.W:
                _camera.cameraPos += _camera.cameraSpeed * _camera.cameraFront;
                break;
            case Key.S:
                _camera.cameraPos -= _camera.cameraSpeed * _camera.cameraFront;
                break;
            case Key.A:
                _camera.cameraPos -= Vector3.Normalize(Vector3.Cross(_camera.cameraFront, _camera.cameraUp)) * _camera.cameraSpeed;
                break;
            case Key.D:
                _camera.cameraPos += Vector3.Normalize(Vector3.Cross(_camera.cameraFront, _camera.cameraUp)) * _camera.cameraSpeed;
                break;
            case Key.Escape:
                Console.WriteLine("close window.\nopenGL window close.");
                _window.Close();
                break;
            default:
                Console.WriteLine($"Key {key} not handled.");
                break;
        }
    }

    private void OnMouseMove(IMouse mouse, Vector2 position)
    {
        if (firstMouse)
        {
            lastX = position.X;
            lastY = position.Y;
            firstMouse = false;
        }
        var offsetX = position.X - lastX;
        var offsetY = lastY - position.Y;
        lastX = position.X;
        lastY = position.Y;

        offsetX *= sensitivity;
        offsetY *= sensitivity;
        
        yaw += offsetX;
        pitch += offsetY;

        if (yaw >= 90.0f)
            yaw = 89.0f;
        if(pitch <= -90.0f)
            pitch = -89.0f;

        var front = new Vector3
        {
            X = MathF.Cos(Matrix4Calculator.GetRadians(yaw)) * MathF.Cos(Matrix4Calculator.GetRadians(pitch)),
            Y = MathF.Sin(Matrix4Calculator.GetRadians(pitch)),
            Z = MathF.Sin(Matrix4Calculator.GetRadians(yaw)) * MathF.Cos(Matrix4Calculator.GetRadians(pitch))
        };
        _camera.cameraFront = Vector3.Normalize(front);
    }

    private void OnMouseScroll(IMouse mouse, ScrollWheel scroll)
    {
        if (_camera.fov >= 1.0f && _camera.fov <= 45.0f)
            _camera.fov -= scroll.Y;
        if (_camera.fov <= 1.0f)
            _camera.fov = 1.0f;
        if (_camera.fov >= 45.0f)
            _camera.fov = 45.0f;
    }
        
    private unsafe void OnRender(double temp)
    {
        _gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        _gl.BindVertexArray(_vao);
        
        // 绑定EBO
        _gl.UseProgram(_program[0]);
        // var timeValue = (float) _window.Time;
        // var greenValue = (MathF.Sin(timeValue) / 2.0f) + 0.5f;
        // var blueValue = (MathF.Sin(timeValue) / 2.0f) + 0.2f;
        // var vertexColorLocation = _gl.GetUniformLocation(_program[0], "ourColor");
        // _gl.Uniform4(vertexColorLocation, 0, greenValue, blueValue, 1.0f);
        
        var nowTime = (float)_window.Time;
        deltaTime = nowTime - lastTime;
        lastTime = nowTime;
       
        var view = _camera.viewMatrix;
        var axis = new Vector3(0.5f, 1.0f, 0);
        var model = Matrix4x4.Identity;
        model = Matrix4x4.CreateFromAxisAngle(axis, 50.0f) * model;
        // var view = Matrix4x4.CreateTranslation(0.0f, 0.0f, -3.0f);
        var proj = Matrix4Calculator.CreatePerspective(Matrix4Calculator.GetRadians(_camera.fov), (float)_window.Size.X / _window.Size.Y, 0.1f, 100.0f);
        var modelLoc = _gl.GetUniformLocation(_program[0], "model");
        _gl.UniformMatrix4(modelLoc, 1, false, (float*)&model);
        var viewLoc = _gl.GetUniformLocation(_program[0], "view");
        _gl.UniformMatrix4(viewLoc, 1, false, (float*)&view);
        var projLoc = _gl.GetUniformLocation(_program[0], "projection");
        _gl.UniformMatrix4(projLoc, 1, false, (float*)&proj);

        // var trans = Matrix4x4.CreateTranslation(0.5f, -0.5f, 0);
        // trans = Matrix4Calculator.Scale(trans, 0.5f, 0.5f, 0.5f);
        // trans = Matrix4Calculator.RotationZ((float)(_window.Time * 2.0), trans);
        // var transLoc = _gl.GetUniformLocation(_program[0], "transform");
        // _gl.UniformMatrix4(transLoc, 1, false, (float*)&trans);
        
        _gl.ActiveTexture(TextureUnit.Texture0);
        _gl.BindTexture(TextureTarget.Texture2D, _textures[0]);
        _gl.Uniform1(_gl.GetUniformLocation(_program[0], "ourTexture0"), 0);
        // _gl.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, (void*) 0);
        
        // var trans2 = Matrix4x4.CreateTranslation(-0.5f, 0.5f, 0);
        // trans2 = Matrix4Calculator.RotationZ((float)-(_window.Time * 2.0), trans2);
        // var transLoc2 = _gl.GetUniformLocation(_program[0], "transform");
        // _gl.UniformMatrix4(transLoc2, 1, false, (float*)&trans2);
        _gl.ActiveTexture(TextureUnit.Texture1);
        _gl.BindTexture(TextureTarget.Texture2D, _textures[1]);
        _gl.Uniform1(_gl.GetUniformLocation(_program[0], "ourTexture1"), 1);
        
        // count是需要绘制的顶点个数, 6是索引个数
        // _gl.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, (void*) 0);
        _gl.DrawArrays(PrimitiveType.Triangles, 0, 36); // 不需要ebo, 需要顶点数组顺序确定
    }

    private unsafe void OnLoad()
    {
        var input = _window.CreateInput();
        foreach (var t in input.Keyboards)
            t.KeyDown += KeyDown;

        foreach (var mouse in input.Mice)
        {
            mouse.Cursor.CursorMode = CursorMode.Disabled;
            mouse.MouseMove += OnMouseMove;
            mouse.Scroll += OnMouseScroll;
        }
        
        // 创建OpenGL上下文绑定到窗口。 如果离屏渲染如avalonia控件使用GetApi()方法
        _gl = _window.CreateOpenGL();
        
        _gl.ClearColor(Color.LightSkyBlue);
        _gl.Enable(EnableCap.DepthTest);
        
        // 开始渲染
        // 顶点缓冲区
        _vao = _gl.GenVertexArray();
        _gl.BindVertexArray(_vao);
        // 前3个是x,y,z坐标, 后3个是颜色, 最后2个是纹理坐标
        float[] vertices =
        [
            -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
            0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
            0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
            0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
            -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,

            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
            0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
            0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
            0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
            -0.5f,  0.5f,  0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,

            -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
            -0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
            -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

            0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
            0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
            0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
            0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
            0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
            0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
            0.5f, -0.5f, -0.5f,  1.0f, 1.0f,
            0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
            0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,

            -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
            0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
            0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
            0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
            -0.5f,  0.5f,  0.5f,  0.0f, 0.0f,
            -0.5f,  0.5f, -0.5f,  0.0f, 1.0f
        ];
        _vbo = _gl.GenBuffer();
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo); //ArrayBuffer: 顶点缓冲区目标, ElementArrayBuffer: 数组缓冲区目标, UniformBuffer: 统一缓冲区目标
        fixed (float* buf = vertices)
        {
            _gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(vertices.Length * sizeof(float)), buf, BufferUsageARB.StaticDraw); //StaticDraw- 设置一次数据, DynamicDraw- 多次更新数据
        }
        // 元素缓冲区EBO or 索引缓冲区
        // uint[] indices =
        // [
        //     0u, 1u, 2u,
        //     0u, 3u, 2u,
        // ]; // vertex索引, 当前程序中是三角形
        // _ebo = _gl.GenBuffer();
        // _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _ebo);
        // fixed (uint* buf = indices)
        // {
        //     _gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(indices.Length * sizeof(uint)), buf, BufferUsageARB.StaticDraw);
        // }
        
        // 创建片段着色器
        uint vertexShader = 0;
        OpenGLFunc.CreateVertexShader(ref _gl, ref vertexShader, VertexCodeType.InputTemp);
        var fragmentCode = OpenGLFunc.fragmentCodeTemp;
        // var fragmentCode = OpenGLFunc.ChangeColor0(0.5f, 0, 0, 1f);
        OpenGLFunc.CreateFragmentShader(ref _gl, ref _fragmentShader, fragmentCode);
        
        // 创建着色器程序
        OpenGLFunc.CreateShaderProgram(ref _gl, ref _program, ref vertexShader, ref _fragmentShader, 0);
        
        // 删除着色器
        OpenGLFunc.DeleteShader(ref _gl, ref _program, ref _fragmentShader, ref vertexShader);
        
        // 绑定顶点属性, size对应vertexShader中一个点的坐标数, stride是每个顶点的字节数长度
        const uint positionLoc = 0;
        _gl.EnableVertexAttribArray(positionLoc);
        _gl.VertexAttribPointer(positionLoc, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), (void*) 0);
        // const uint colorLoc = 1;
        // _gl.EnableVertexAttribArray(colorLoc);
        // _gl.VertexAttribPointer(colorLoc, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), (void*) (3 * sizeof(float)));
        const uint texCoordLoc = 1;
        _gl.EnableVertexAttribArray(texCoordLoc);
        _gl.VertexAttribPointer(texCoordLoc, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), (void*) (3 * sizeof(float)));
        
        var picPath = Path.Combine(AppContext.BaseDirectory, "Picture", "silk.png");
        LoadTexture(picPath);
        picPath = Path.Combine(AppContext.BaseDirectory, "Picture", "WoodBox.jpg");
        LoadTexture(picPath);
        
        // const uint colorLoc = 1;
        // _gl.EnableVertexAttribArray(colorLoc);
        // _gl.VertexAttribPointer(colorLoc, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), (void*) (3 * sizeof(float)));
        
        // 解绑缓冲区
        _gl.BindVertexArray(0);
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
        _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);
        
        // _gl.Enable(GLEnum.Blend);
        // _gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
    }

    private unsafe void LoadTexture(string pngPath, int pictureType = 0)
    {
        if (!File.Exists(pngPath))
        {
            Console.WriteLine($"Texture file not found: {pngPath}");
            return;
        }
        Console.WriteLine($"{pngPath} Loading texture...");
        _textures.Add(_gl.GenTexture());
        _gl.BindTexture(TextureTarget.Texture2D, _textures[^1]);
        _gl.TexParameter(TextureTarget.Texture2D, GLEnum.TextureWrapS, (int)TextureWrapMode.Repeat);
        _gl.TexParameter(TextureTarget.Texture2D, GLEnum.TextureWrapT, (int)TextureWrapMode.Repeat);
        _gl.TexParameter(TextureTarget.Texture2D, GLEnum.TextureMinFilter, (int)TextureMinFilter.Linear);
        _gl.TexParameter(TextureTarget.Texture2D, GLEnum.TextureMagFilter, (int)TextureMagFilter.Linear);
        
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
            _gl.TexImage2D(TextureTarget.Texture2D, 0, (int)InternalFormat.Rgba, 
                (uint)image.Width, (uint)image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data);
        }
        
        _gl.GenerateMipmap(TextureTarget.Texture2D);
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
        _camera = new();
        _vaos = [];
        _vbos = [];
        _ebos = [];
        _textures = [];
        _points = [];
        _program = [];
        _fragmentShader = [];
        Console.WriteLine("No Points!");
    }

    public SILKOpenGLOnly(List<MathNet.Numerics.LinearAlgebra.Vector<float>> points)
    {
        _points = points;
        _camera = new();
        _vaos = [];
        _vbos = [];
        _ebos = [];
        _program = [];
        _textures = [];
        _fragmentShader = [];
    }

    public void PubStartOpenGl()
    {
        StartOpenGl();
    }
    
}