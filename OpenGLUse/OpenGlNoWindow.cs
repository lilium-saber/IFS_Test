using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using AvaloniaApp.Ults;
using AvaloniaApp.Ults.Object;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using StbImageSharp;

namespace AvaloniaApp.OpenGLUse;

public class OpenGlNoWindow
{
    private GL _gl;
    private IWindow _window;
    private Camera _camera = new();
    private LightObject _lightObject = new();
    private OtherObject _otherObject = new();
    private uint _fbo;
    private uint _texture;
    private uint _vao;
    private uint _vbo;
    private uint _program;
    private uint _fragmentShader;
    
    private float _deltaTime;
    private float _lastTime;
    // private uint _rbo;
    
    private TcpClient? _client;
    private NetworkStream? _stream;
    
    // ReSharper disable once MemberCanBePrivate.Global
    public int VWidth { get; set; } = 500;
    // ReSharper disable once MemberCanBePrivate.Global
    public int VHeight { get; set; } = 500;

    private unsafe void OnRender(double delta)
    {
        _gl.BindFramebuffer(FramebufferTarget.Framebuffer, _fbo);
        _gl.Viewport(0, 0, (uint)VWidth, (uint)VHeight);
        
        // Render
        _gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        var nowTime = (float)_window!.Time;
        _deltaTime = nowTime - _lastTime;
        _lastTime = nowTime;
        const float radius = 5.0f;
        _lightObject.LightPosition = new(radius * MathF.Cos(nowTime), 2.0f, radius * MathF.Sin(nowTime));

        var view = _camera.ViewMatrix;
        var proj = Matrix4Calculator.CreatePerspective(Matrix4Calculator.GetRadians(_camera.Fov), (float)_window.Size.X / _window.Size.Y, 0.1f, 300.0f);
        _lightObject.RenderLight(ref _gl, view, proj);
        _otherObject.RenderObject(ref _gl, view, proj, _lightObject.LightPosition, _lightObject.LightColor, _camera.CameraPos);
        
        // Socket
        var pixels = new byte[VWidth * VHeight * 4];
        fixed(byte* p = pixels)
        {
            _gl.ReadPixels(0, 0, (uint)VWidth, (uint)VHeight, PixelFormat.Rgba, PixelType.UnsignedByte, p);
        }
        SendSocket(pixels);
        _gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }
    
    private void OnLoad()
    {
        _gl = GL.GetApi(_window);
        _fbo = _gl.GenFramebuffer();
        _gl.BindFramebuffer(FramebufferTarget.Framebuffer, _fbo);
        
        // Load
        _gl.Enable(EnableCap.DepthTest);
        _lightObject.LoadLight(ref _gl);
        var texturePath = Path.Combine(AppContext.BaseDirectory, "Picture", "silk.png");
        _otherObject.LoadObject(ref _gl, texturePath);
        
        _gl.Enable(EnableCap.Blend);
        _gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
    }
    
    private void OnUpdate(double delta)
    {
        // Update logic here
    }
    
    private void OnClose()
    {
        _gl.DeleteFramebuffer(_fbo);
        // _gl.DeleteRenderbuffer(_rbo);
        _gl.Dispose();
        _window?.Dispose();
        SocketClose();
    }

    private void SendSocket(byte[] data)
    {
        try
        {
            if(_stream == null || !_client!.Connected)
            {
                SocketInit();
                Console.WriteLine("Socket restart connected");
                return;
            }
            var dataWight = BitConverter.GetBytes(VWidth);
            var dataHeight = BitConverter.GetBytes(VHeight);
            _stream!.Write(dataWight, 0, dataWight.Length);
            _stream.Write(dataHeight, 0, dataHeight.Length);
            _stream.Write(data, 0, data.Length);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Socket error: {e}");
        }
    }

    private void SocketInit()
    {
        try
        {
            _client = new();
            _client.Connect(IPAddress.Loopback, OpenGlUlts.SocketPost);
            _stream = _client.GetStream();
        }
        catch (Exception e)
        {
            Console.WriteLine($"SocketInit error: {e}");
            throw;
        }
    }
    
    private void SocketClose()
    {
        try
        {
            _stream?.Close();
            _client?.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine($"SocketClose error: {e.Message}");
        }
    }

    private void Start()
    {
        var options = WindowOptions.Default;
        options.WindowState = WindowState.Minimized;
        options.IsVisible = false;
        _window = Silk.NET.Windowing.Window.Create(options);
        _window.Load += OnLoad;
        _window.Update += OnUpdate;
        _window.Render += OnRender;
        _window.Closing += OnClose;
        _window.Run();
    }
    
    public void StartOpenGlBackground()
    {
        Start();
        SocketInit();
    }

    public void StopOpenGlBackground()
    {
        _window.Close();
    }
}