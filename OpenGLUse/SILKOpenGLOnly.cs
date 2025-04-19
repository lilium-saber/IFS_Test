﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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
using Bitmap = Avalonia.Media.Imaging.Bitmap;

namespace AvaloniaApp.OpenGLUse;

public class SILKOpenGLOnly
{
    private GL? _gl;
    private IWindow? _window;
    private readonly Camera _camera;
    private readonly LightObject _sunLight;
    private readonly IfsResObject _ifsResObject;
    private uint _vao; // 顶点数组对象
    private uint _vbo; // 顶点缓冲对象 
    private uint _ebo; // 索引缓冲对象
    private uint _texture; // 纹理对象
    private List<byte[]> _pictures;
    private byte[] _pic;
    private List<uint> _vaos; // 顶点数组对象list
    private List<uint> _vbos; // 顶点缓冲对象list
    private List<uint> _ebos; // 索引缓冲对象list
    private List<uint> _textures;
    private List<uint> _program; // 着色器程序
    private List<uint> _fragmentShader; // 片段着色器
    private readonly List<MathNet.Numerics.LinearAlgebra.Vector<float>> _points;

    private static readonly int _weight = 800;
    private static readonly int _height = 800;

    private bool _useTime = false;
    private float _deltaTime = 0.0f;
    private float _lastTime = 0.0f;

    private float _lastX = _weight / 2;
    private float _lastY = _height / 2;
    private float _yaw = 0.0f;
    private float _pitch = 0.0f;
    // private bool _firstMouse = true;
    private bool _useMouse = true;
    private Vector3 _savePos;
    private Vector3 _saveFront;
    private float _saveYaw;
    private float _savePitch;
    
    private IInputContext? _input;

    
    private void KeyDown(IKeyboard keyboard, Key key, int code)
    {
        Console.WriteLine($"input key: {key}");
        _camera.CameraSpeed = 20.0f * _deltaTime;
        
        switch (key)
        {
            case Key.M:
                _useMouse = !_useMouse;
                if(_useMouse)
                {
                    _camera.CameraPos = _savePos;
                    _camera.CameraFront = _saveFront;
                    _yaw = _saveYaw;
                    _pitch = _savePitch;
                }
                else
                {
                    _savePos = _camera.CameraPos;
                    _saveFront = _camera.CameraFront;
                    _saveYaw = _yaw;
                    _savePitch = _pitch;
                }
                break;
            case Key.L:
                _camera.CameraSpeed += 10.0f;
                Console.WriteLine($"camera speed: {_camera.CameraSpeed}");
                break;
            case Key.K:
                _camera.CameraSpeed -= 10.0f;
                Console.WriteLine($"camera speed: {_camera.CameraSpeed}");
                break;
            case Key.T:
                _useTime = !_useTime;
                break;
            case Key.W:
                _camera.CameraPos += _camera.CameraSpeed * 1.5f * _camera.CameraFront;
                break;
            case Key.S:
                _camera.CameraPos -= _camera.CameraSpeed * 1.5f * _camera.CameraFront;
                break;
            case Key.A:
                _camera.CameraPos -= Vector3.Normalize(Vector3.Cross(Vector3.Normalize(new(_camera.CameraFront.X, 0.0f, _camera.CameraFront.Z)), _camera.CameraUp)) * _camera.CameraSpeed / 2;
                break;
            case Key.D:
                _camera.CameraPos += Vector3.Normalize(Vector3.Cross(Vector3.Normalize(new(_camera.CameraFront.X, 0.0f, _camera.CameraFront.Z)), _camera.CameraUp)) * _camera.CameraSpeed / 2;
                break;
            case Key.Space:
                _camera.CameraPos += _camera.CameraSpeed * _camera.CameraUp;
                break;
            case Key.ControlLeft:
                _camera.CameraPos -= _camera.CameraSpeed * _camera.CameraUp;
                break;
            case Key.Escape:
                Console.WriteLine("close window.\nopenGL window close.");
                _window!.Close();
                break;
            default:
                Console.WriteLine($"Key {key} not handled.");
                break;
        }
        Console.WriteLine($"pos is {_camera.CameraPos}");
    }

    private void OnMouseMove(IMouse mouse, Vector2 position)
    {
        if(!_useMouse)
            return;
        var offsetX = position.X - _lastX;
        var offsetY = _lastY - position.Y;
        _lastX = position.X;
        _lastY = position.Y;

        offsetX *= _camera.Sensitivity;
        offsetY *= _camera.Sensitivity;
        
        _yaw += offsetX;
        _pitch += offsetY;

        if (_yaw >= 90.0f)
            _yaw = 89.0f;
        if(_pitch <= -90.0f)
            _pitch = -89.0f;

        var front = new Vector3
        {
            X = MathF.Cos(Matrix4Calculator.GetRadians(_yaw)) * MathF.Cos(Matrix4Calculator.GetRadians(_pitch)),
            Y = MathF.Sin(Matrix4Calculator.GetRadians(_pitch)),
            Z = MathF.Sin(Matrix4Calculator.GetRadians(_yaw)) * MathF.Cos(Matrix4Calculator.GetRadians(_pitch))
        };
        Console.WriteLine($"yaw {_yaw}, pitch {_pitch}");
        _camera.CameraFront = Vector3.Normalize(front);
    }

    private void OnMouseScroll(IMouse mouse, ScrollWheel scroll)
    {
        if (_camera.Fov is >= 1.0f and <= 45.0f)
            _camera.Fov -= scroll.Y;
        if (_camera.Fov <= 1.0f)
            _camera.Fov = 1.0f;
        if (_camera.Fov >= 45.0f)
            _camera.Fov = 45.0f;
    }
        
    private unsafe void OnRender(double temp)
    {
        _gl!.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        
        var nowTime = (float)_window!.Time;
        _deltaTime = nowTime - _lastTime;
        _lastTime = nowTime;
        var view = _camera.ViewMatrix;
        var axis = new Vector3(0.5f, 1.0f, 0);
        var model = Matrix4x4.Identity;
        model = Matrix4x4.CreateFromAxisAngle(axis, 50.0f) * model;
        // var view = Matrix4x4.CreateTranslation(0.0f, 0.0f, -3.0f);
        var proj = Matrix4Calculator.CreatePerspective(Matrix4Calculator.GetRadians(_camera.Fov), (float)_window.Size.X / _window.Size.Y, 0.1f, 100.0f);
        
        _sunLight.RenderLight(ref _gl, view, proj);
        _ifsResObject.DrawResObject(ref _gl, view, proj, _sunLight.LightPosition, _sunLight.LightColor, _camera.CameraPos);
        const float radius = 5.0f;
        _sunLight.LightPosition = new(radius * MathF.Cos(nowTime), 2.0f, radius * MathF.Sin(nowTime));
    }

    private unsafe void OnLoad()
    {
        if (_input is null)
        {
            _input = _window!.CreateInput();
        }
        else
        {
            Console.WriteLine("Input is already set.");
        }
        foreach (var t in _input.Keyboards)
            t.KeyDown += KeyDown;

        foreach (var mouse in _input.Mice)
        {
            mouse.Cursor.CursorMode = CursorMode.Disabled;
            mouse.MouseMove += OnMouseMove;
            mouse.Scroll += OnMouseScroll;
        }
        
        // 创建OpenGL上下文绑定到窗口。 如果离屏渲染如avalonia控件使用GetApi()方法
        _gl = _window.CreateOpenGL();
        // _gl.ClearColor(Color.LightSkyBlue);
        _gl.Enable(EnableCap.DepthTest);
        
        // 其他物体的代码
        _sunLight.LoadLight(ref _gl); // 光照物体
        _ifsResObject.LoadResObject(ref _gl, ref _pic);
        
        _gl.Enable(EnableCap.Blend);
        _gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
    }

    private unsafe void LoadTexture(byte[] pictures)
    {
        if (pictures.Length == 0)
        {
            Console.WriteLine("Texture file not found");
            return;
        }
        Console.WriteLine($"Loading texture...");
        using var imageStream = new MemoryStream(pictures);
        var image = ImageResult.FromStream(imageStream, ColorComponents.RedGreenBlueAlpha);
        _textures.Add(_gl!.GenTexture());
        _gl.BindTexture(TextureTarget.Texture2D, _textures[^1]);
        _gl.TexParameter(TextureTarget.Texture2D, GLEnum.TextureWrapS, (int)TextureWrapMode.Repeat);
        _gl.TexParameter(TextureTarget.Texture2D, GLEnum.TextureWrapT, (int)TextureWrapMode.Repeat);
        _gl.TexParameter(TextureTarget.Texture2D, GLEnum.TextureMinFilter, (int)TextureMinFilter.Linear);
        _gl.TexParameter(TextureTarget.Texture2D, GLEnum.TextureMagFilter, (int)TextureMagFilter.Linear);
        
        fixed (byte* data = image.Data)
        {
            _gl.TexImage2D(TextureTarget.Texture2D, 0, (int)InternalFormat.Rgba,
                IFS_line.Ults.Ults.BitmapLen, IFS_line.Ults.Ults.BitmapLen, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data);
        }
        
        _gl.GenerateMipmap(TextureTarget.Texture2D);
    }

    private unsafe void LoadTexture(string pngPath, int pictureType = 0)
    {
        if (!File.Exists(pngPath))
        {
            Console.WriteLine($"Texture file not found: {pngPath}");
            return;
        }
        Console.WriteLine($"{pngPath} Loading texture...");
        _textures.Add(_gl!.GenTexture());
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

    private void OnClose()
    {
        // 释放 OpenGL 资源
        if (_gl != null)
        {
            _input?.Dispose();
            _gl.DeleteVertexArray(_vao);
            _gl.DeleteBuffer(_vbo);
            foreach(var vao in _vaos)
            {
                _gl.DeleteVertexArray(vao);
            }
            foreach(var vbo in _vbos)
            {
                _gl.DeleteBuffer(vbo);
            }
            foreach(var ebo in _ebos)
            {
                _gl.DeleteBuffer(ebo);
            }
            foreach(var program in _program)
            {
                _gl.DeleteProgram(program);
            }
            foreach(var fragmentShader in _fragmentShader)
            {
                _gl.DeleteShader(fragmentShader);
            }
            foreach(var texture in _textures)
            {
                _gl.DeleteTexture(texture);
            }
            _gl.Dispose();
            Console.WriteLine("OpenGL resources disposed.");
        }
        else
        {
            Console.WriteLine("OpenGL context is already null.");
        }

        // 清理窗口引用
        _window = null;
        Console.WriteLine("Window reference cleared.");
    }
    
    // private unsafe void OnUpdate(double delta)
    // {
    //     
    // }
    
    // 状态机
    private void StartOpenGl()
    {
        var options = WindowOptions.Default;
        options.Size = new Vector2D<int>(_weight, _height);
        options.Title = "SILKOpenGLExample";
        options.IsVisible = true; // 是否显示窗口
        _window = Silk.NET.Windowing.Window.Create(options);

        _window.Load += OnLoad;
        // _window.Update += OnUpdate;
        _window.Render += OnRender;
        _window.Closing += OnClose;
        _window.Run();
    }

    public SILKOpenGLOnly()
    {
        _camera = new();
        _sunLight = new();
        _ifsResObject = new();
        _vaos = [];
        _vbos = [];
        _ebos = [];
        _textures = [];
        _points = [];
        _program = [];
        _fragmentShader = [];
        _pictures = [];
        _pic = [];
        Console.WriteLine("No Points!");
    }

    public SILKOpenGLOnly(List<MathNet.Numerics.LinearAlgebra.Vector<float>> points)
    {
        _points = points;
        _camera = new();
        _sunLight = new();
        _ifsResObject = new();
        _vaos = [];
        _vbos = [];
        _ebos = [];
        _program = [];
        _textures = [];
        _pictures = [];
        _fragmentShader = [];
        _pic = [];
    }
    
    public SILKOpenGLOnly(List<byte[]> pictures)
    {
        _pictures = pictures;
        _camera = new();
        _sunLight = new();
        _ifsResObject = new();
        _points = [];
        _vaos = [];
        _vbos = [];
        _ebos = [];
        _program = [];
        _textures = [];
        _fragmentShader = [];
        _pic = pictures[0];
    }

    public void PubStartOpenGl()
    {
        StartOpenGl();
    }
    
}