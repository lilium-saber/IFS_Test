using System;
using System.Drawing;
using System.Numerics;
using AvaloniaApp.Ults;
using AvaloniaApp.Ults.Object;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace AvaloniaApp.OpenGLUse;

public class OtherOpenGl
{
    private GL? _gl;
    private IWindow? _window;
    private IInputContext? _input = null;
    private readonly OtherObject _otherObject = new();
    private readonly LightObject _lightObject = new();
    private readonly Camera _cameraObject = new();
    private readonly SkyBox _skyBoxObject = new();
    private float _deltaTime = 0.0f;
    private float _lastTime = 0.0f;
    private float _lastX = 400.0f;
    private float _lastY = 400.0f;
    private float _yaw = -90.0f;
    private float _pitch = 0.0f;
    private bool _rowObject = false;
    private bool _useMouse = true;
    
    private void KeyDown(IKeyboard keyboard, Key key, int code)
    {
        Console.WriteLine($"input key: {key}");
        _cameraObject.CameraSpeed = 20.0f * _deltaTime;
        
        switch (key)
        {
            case Key.R:
                _rowObject = !_rowObject;
                break;
            case Key.L:
                _cameraObject.CameraSpeed += 10.0f;
                Console.WriteLine($"camera speed: {_cameraObject.CameraSpeed}");
                break;
            case Key.K:
                _cameraObject.CameraSpeed -= 10.0f;
                Console.WriteLine($"camera speed: {_cameraObject.CameraSpeed}");
                break;
            case Key.W:
                _cameraObject.CameraPos += _cameraObject.CameraSpeed * 1.5f * _cameraObject.CameraFront;
                break;
            case Key.S:
                _cameraObject.CameraPos -= _cameraObject.CameraSpeed * 1.5f * _cameraObject.CameraFront;
                break;
            case Key.A:
                _cameraObject.CameraPos -= Vector3.Normalize(Vector3.Cross(Vector3.Normalize(_cameraObject.CameraFront with { Y = 0.0f }), _cameraObject.CameraUp)) * _cameraObject.CameraSpeed / 2;
                break;
            case Key.D:
                _cameraObject.CameraPos += Vector3.Normalize(Vector3.Cross(Vector3.Normalize(_cameraObject.CameraFront with { Y = 0.0f }), _cameraObject.CameraUp)) * _cameraObject.CameraSpeed / 2;
                break;
            case Key.Space:
                _cameraObject.CameraPos += _cameraObject.CameraSpeed * _cameraObject.CameraUp;
                break;
            case Key.ControlLeft:
                _cameraObject.CameraPos -= _cameraObject.CameraSpeed * _cameraObject.CameraUp;
                break;
            case Key.Escape:
                Console.WriteLine("close window.\nopenGL window close.");
                _window!.Close();
                break;
            default:
                Console.WriteLine($"Key {key} not handled.");
                break;
        }
        Console.WriteLine($"pos is {_cameraObject.CameraPos}");
    }
    
    private void OnMouseScroll(IMouse mouse, ScrollWheel scroll)
    {
        if (_cameraObject.Fov is >= 1.0f and <= 45.0f)
            _cameraObject.Fov -= scroll.Y;
        if (_cameraObject.Fov <= 1.0f)
            _cameraObject.Fov = 1.0f;
        if (_cameraObject.Fov >= 45.0f)
            _cameraObject.Fov = 45.0f;
    }
    
    private void OnMouseMove(IMouse mouse, Vector2 position)
    {
        if(!_useMouse)
            return;
        var offsetX = position.X - _lastX;
        var offsetY = _lastY - position.Y;
        _lastX = position.X;
        _lastY = position.Y;

        offsetX *= _cameraObject.Sensitivity;
        offsetY *= _cameraObject.Sensitivity;
        
        _yaw += offsetX;
        _pitch += offsetY;

        // if (_yaw >= 90.0f)
        //     _yaw = 89.0f;
        if(_pitch <= -90.0f)
            _pitch = -90.0f;
        if(_pitch >= 90.0f)
            _pitch = 90.0f;

        var front = new Vector3
        {
            X = MathF.Cos(Matrix4Calculator.GetRadians(_yaw)) * MathF.Cos(Matrix4Calculator.GetRadians(_pitch)),
            Y = MathF.Sin(Matrix4Calculator.GetRadians(_pitch)),
            Z = MathF.Sin(Matrix4Calculator.GetRadians(_yaw)) * MathF.Cos(Matrix4Calculator.GetRadians(_pitch))
        };
        Console.WriteLine($"yaw {_yaw}, pitch {_pitch}");
        _cameraObject.CameraFront = Vector3.Normalize(front);
    }
    
    private void OnLoad()
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
        
        _gl = _window.CreateOpenGL();
        _gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
        _gl.Enable(EnableCap.DepthTest);
        
        _otherObject.LoadObject(ref _gl);
        _lightObject.LoadLight(ref _gl);
        _skyBoxObject.LoadSkyBox(ref _gl);
        
        _gl.Enable(EnableCap.Blend);
        _gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
    }

    private void OnRender(double delta)
    {
        _gl!.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        _gl.ClearColor(Color.DimGray);
        var nowTime = (float)_window!.Time;
        _deltaTime = nowTime - _lastTime;
        _lastTime = nowTime;

        if (_rowObject)
        {
            var temp = Matrix4x4.CreateRotationY(_deltaTime * 0.25f);
            _otherObject.Mold = temp * _otherObject.Mold;
        }
        
        const float radius = 5.0f;
        _lightObject.LightPosition = new(radius * MathF.Cos(nowTime), 2.0f, radius * MathF.Sin(nowTime));
        var view = _cameraObject.ViewMatrix;
        var proj = Matrix4Calculator.CreatePerspective(Matrix4Calculator.GetRadians(_cameraObject.Fov), (float)_window.Size.X / _window.Size.Y, 0.1f, 300.0f);
        
        _lightObject.RenderLight(ref _gl, view, proj);
        _otherObject.RenderObject(ref _gl, view, proj);
        _skyBoxObject.RenderSkyBox(ref _gl, view, proj);
    }
    
    private void OnClose()
    {
        _input?.Dispose();
        _gl?.Dispose();
        _gl = null;
        _window = null;
    }
    
    public void StartOpenGlBackground()
    {
        var options = WindowOptions.Default;
        options.Size = new Vector2D<int>(600, 600);
        options.IsVisible = true;
        options.Title = "OpenGL Window";
        
        _window = Silk.NET.Windowing.Window.Create(options);
        _window.Load += OnLoad;
        _window.Render += OnRender;
        _window.Closing += OnClose;
        _window.Run();
    }
}