using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using AvaloniaApp.Ults;
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
    private uint _fbo;
    private uint _texture;
    private uint _vao;
    private uint _vbo;
    private uint _program;
    private uint _fragmentShader;
    
    private float _deltaTime;
    private float _lastTime;
    // private uint _rbo;
    
    // ReSharper disable once MemberCanBePrivate.Global
    public int VWidth { get; set; } = 500;
    // ReSharper disable once MemberCanBePrivate.Global
    public int VHeight { get; set; } = 500;

    private unsafe void OnRender(double delta)
    {
        _gl.BindFramebuffer(FramebufferTarget.Framebuffer, _fbo);
        _gl.Viewport(0, 0, (uint)VWidth, (uint)VHeight);
        
        // Render
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
        _lightObject.RenderLight(ref _gl, view, proj);
        
        _gl.UseProgram(_program);
        _gl.BindVertexArray(_vao);
        var modelLoc = _gl.GetUniformLocation(_program, "model");
        _gl.UniformMatrix4(modelLoc, 1, false, (float*)&model);
        var viewLoc = _gl.GetUniformLocation(_program, "view");
        _gl.UniformMatrix4(viewLoc, 1, false, (float*)&view);
        var projLoc = _gl.GetUniformLocation(_program, "projection");
        _gl.UniformMatrix4(projLoc, 1, false, (float*)&proj);
        
        _gl.ActiveTexture(TextureUnit.Texture0);
        _gl.BindTexture(TextureTarget.Texture2D, _texture);
        _gl.Uniform1(_gl.GetUniformLocation(_program, "ourTexture0"), 0);
        const float radius = 5.0f;
        _lightObject.LightPosition = new(radius * MathF.Cos(nowTime), 2.0f, radius * MathF.Sin(nowTime));
        _gl.Uniform3(_gl.GetUniformLocation(_program, "lightColor"), _lightObject.LightColor);
        _gl.Uniform3(_gl.GetUniformLocation(_program, "objectColor"), 1.0f, 0.5f, 0.31f);
        _gl.Uniform3(_gl.GetUniformLocation(_program, "lightPos"), _lightObject.LightPosition);
        _gl.Uniform3(_gl.GetUniformLocation(_program, "viewPos"), _camera.CameraPos);
        _gl.DrawArrays(PrimitiveType.Triangles, 0, 12); 
        
        
        // Socket
        var pixels = new byte[VWidth * VHeight * 4];
        fixed(byte* p = pixels)
        {
            _gl.ReadPixels(0, 0, (uint)VWidth, (uint)VHeight, PixelFormat.Rgba, PixelType.UnsignedByte, p);
        }
        SendSocket(pixels);
        _gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }
    
    private unsafe void OnLoad()
    {
        _fbo = _gl.GenFramebuffer();
        _gl.BindFramebuffer(FramebufferTarget.Framebuffer, _fbo);
        
        // Load
        _gl.Enable(EnableCap.DepthTest);
        _lightObject.LoadLight(ref _gl);
        _vao = _gl.GenVertexArray();
        _gl.BindVertexArray(_vao);
        float[] vertices =
        [
            -0.5f, -0.5f, 0.0f,  0.0f, 0.0f,  1.0f, 0.0f, 1.0f,
            0.5f, -0.5f, 0.0f,  1.0f, 0.0f,  1.0f, 0.0f, 1.0f,
            0.5f,  0.5f, 0.0f,  1.0f, 1.0f,  1.0f, 0.0f, 1.0f,
            0.5f,  0.5f, 0.0f,  1.0f, 1.0f,  1.0f, 0.0f, 1.0f,
            -0.5f,  0.5f, 0.0f,  0.0f, 1.0f,  1.0f, 0.0f, 1.0f,
            -0.5f, -0.5f, 0.0f,  0.0f, 0.0f,  1.0f, 0.0f, 1.0f,

            // 第二个面
            0.0f, -0.5f, -0.5f,  0.0f, 0.0f,  1.0f, 0.0f, 1.0f,
            0.0f,  0.5f, -0.5f,  1.0f, 0.0f,  1.0f, 0.0f, 1.0f,
            0.0f,  0.5f,  0.5f,  1.0f, 1.0f,  1.0f, 0.0f, 1.0f,
            0.0f,  0.5f,  0.5f,  1.0f, 1.0f,  1.0f, 0.0f, 1.0f,
            0.0f, -0.5f,  0.5f,  0.0f, 1.0f,  1.0f, 0.0f, 1.0f,
            0.0f, -0.5f, -0.5f,  0.0f, 0.0f,  1.0f, 0.0f, 1.0f
        ];
        _vbo = _gl.GenBuffer();
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);
        fixed (float* buf = vertices)
        {
            _gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(vertices.Length * sizeof(float)), buf, BufferUsageARB.StaticDraw); //StaticDraw- 设置一次数据, DynamicDraw- 多次更新数据
        }
        uint vertexShader = 0;
        OpenGLFunc.CreateVertexShader(ref _gl, ref vertexShader, VertexCodeType.InputTemp);
        const string fragmentCode = OpenGLFunc.fragmentCodeTemp;
        _fragmentShader = _gl.CreateShader(ShaderType.FragmentShader);
        _gl.ShaderSource(_fragmentShader, fragmentCode);
        _gl.CompileShader(_fragmentShader);
        _program = _gl.CreateProgram();
        _gl.AttachShader(_program, vertexShader);
        _gl.AttachShader(_program, _fragmentShader);
        _gl.LinkProgram(_program);
        _gl.DetachShader(_program, vertexShader);
        _gl.DetachShader(_program, _fragmentShader);
        _gl.DeleteShader(vertexShader);
        _gl.DeleteShader(_fragmentShader);
        const uint positionLoc = 0;
        _gl.EnableVertexAttribArray(positionLoc);
        _gl.VertexAttribPointer(positionLoc, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), (void*) 0);
        const uint texCoordLoc = 1;
        _gl.EnableVertexAttribArray(texCoordLoc);
        _gl.VertexAttribPointer(texCoordLoc, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), (void*) (3 * sizeof(float)));
        const uint normalLoc = 2;
        _gl.EnableVertexAttribArray(normalLoc);
        _gl.VertexAttribPointer(normalLoc, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), (void*) (5 * sizeof(float)));
        var picPath = Path.Combine(AppContext.BaseDirectory, "Picture", "silk.png");
        LoadTexture(picPath);
        
        _gl.BindVertexArray(0);
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
        _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);
        
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
        _gl?.Dispose();
        _window?.Dispose();
    }

    private void SendSocket(byte[] data)
    {
        try
        {
            using var client = new TcpClient();
            client.Connect(IPAddress.Loopback, 11010);
            using var stream = client.GetStream();
            var dataWight = BitConverter.GetBytes(VWidth);
            var dataHeight = BitConverter.GetBytes(VHeight);
            stream.Write(dataWight, 0, dataWight.Length);
            stream.Write(dataHeight, 0, dataHeight.Length);
            stream.Write(data, 0, data.Length);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
    
    private unsafe void LoadTexture(string pngPath, int pictureType = 0)
    {
        if (!File.Exists(pngPath))
        {
            Console.WriteLine($"Texture file not found: {pngPath}");
            return;
        }
        Console.WriteLine($"{pngPath} Loading texture...");
        _texture = _gl.GenTexture();
        _gl.BindTexture(TextureTarget.Texture2D, _texture);
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
        _gl = GL.GetApi(_window);
    }
    
    public void StartOpenGlBackground()
    {
        Start();
    }

    public void StopOpenGlBackground()
    {
        _window.Close();
    }
}