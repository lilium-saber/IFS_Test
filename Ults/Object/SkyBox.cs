using System;
using System.IO;
using System.Linq;
using System.Numerics;
using Silk.NET.OpenGL;
using StbImageSharp;

namespace AvaloniaApp.Ults.Object;

internal class SkyBox
{
    private uint _skyboxVao;
    private uint _skyboxVbo;
    private uint _skyboxTexture;
    private uint _skyboxProgram;
    private uint _skyboxShader;
    internal Matrix4x4 Mold { get; set; } = Matrix4x4.Identity;

    private readonly float[] _skyboxVertices =
        [
            -1.0f,  1.0f, -1.0f,
            -1.0f, -1.0f, -1.0f,
            1.0f, -1.0f, -1.0f,
            1.0f, -1.0f, -1.0f,
            1.0f,  1.0f, -1.0f,
            -1.0f,  1.0f, -1.0f,

            -1.0f, -1.0f,  1.0f,
            -1.0f, -1.0f, -1.0f,
            -1.0f,  1.0f, -1.0f,
            -1.0f,  1.0f, -1.0f,
            -1.0f,  1.0f,  1.0f,
            -1.0f, -1.0f,  1.0f,

            1.0f, -1.0f, -1.0f,
            1.0f, -1.0f,  1.0f,
            1.0f,  1.0f,  1.0f,
            1.0f,  1.0f,  1.0f,
            1.0f,  1.0f, -1.0f,
            1.0f, -1.0f, -1.0f,

            -1.0f, -1.0f,  1.0f,
            -1.0f,  1.0f,  1.0f,
            1.0f,  1.0f,  1.0f,
            1.0f,  1.0f,  1.0f,
            1.0f, -1.0f,  1.0f,
            -1.0f, -1.0f,  1.0f,

            -1.0f,  1.0f, -1.0f,
            1.0f,  1.0f, -1.0f,
            1.0f,  1.0f,  1.0f,
            1.0f,  1.0f,  1.0f,
            -1.0f,  1.0f,  1.0f,
            -1.0f,  1.0f, -1.0f,

            -1.0f, -1.0f, -1.0f,
            -1.0f, -1.0f,  1.0f,
            1.0f, -1.0f, -1.0f,
            1.0f, -1.0f, -1.0f,
            -1.0f, -1.0f,  1.0f,
            1.0f, -1.0f,  1.0f,
        ];

    private const string VertexCode =
        """
        #version 330 core
        layout (location = 0) in vec3 aPos;
        out vec3 TexCoords;
        uniform mat4 view;
        uniform mat4 projection;
        void main()
        {
            TexCoords = aPos;
            vec4 pos = projection * view * vec4(aPos, 1.0);
            gl_Position = pos.xyww;
        }
        """;

    private const string FragmentCode =
        """
        #version 330 core
        out vec4 FragColor;
        in vec3 TexCoords;
        uniform samplerCube skybox;
        void main()
        {
            FragColor = texture(skybox, TexCoords);
        }
        """;
    
    private unsafe void LoadTexture(ref GL gl, string[] cubeFacePaths)
    {
        if (cubeFacePaths.Length != 6)
        {
            Console.WriteLine("Cube map requires 6 texture paths.");
            return;
        }

        _skyboxTexture = gl.GenTexture();
        gl.BindTexture(TextureTarget.TextureCubeMap, _skyboxTexture);

        for (var i = 0; i < 6; i++)
        {
            if (!File.Exists(cubeFacePaths[i]))
            {
                Console.WriteLine($"Texture file not found: {cubeFacePaths[i]}");
                continue;
            }

            using var imageStream = File.OpenRead(cubeFacePaths[i]);
            var image = ImageResult.FromStream(imageStream, ColorComponents.RedGreenBlueAlpha);
            fixed (byte* data = image.Data)
            {
                gl.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, (int)InternalFormat.Rgba,
                    (uint)image.Width, (uint)image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data);
            }
            // gl.GenerateMipmap(TextureTarget.TextureCubeMap);
        }

        gl.TexParameter(TextureTarget.TextureCubeMap, GLEnum.TextureMinFilter, (int)TextureMinFilter.Linear);
        gl.TexParameter(TextureTarget.TextureCubeMap, GLEnum.TextureMagFilter, (int)TextureMagFilter.Linear);
        gl.TexParameter(TextureTarget.TextureCubeMap, GLEnum.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        gl.TexParameter(TextureTarget.TextureCubeMap, GLEnum.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
        gl.TexParameter(TextureTarget.TextureCubeMap, GLEnum.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
        
        Console.WriteLine($"skybox texture loaded successfully.");
    }

    internal unsafe void LoadSkyBox(ref GL gl)
    {
        _skyboxVao = gl.GenVertexArray();
        _skyboxVbo = gl.GenBuffer();
        gl.BindVertexArray(_skyboxVao);
        gl.BindBuffer(BufferTargetARB.ArrayBuffer, _skyboxVbo);
        fixed(float* ptr = _skyboxVertices)
        {
            gl.BufferData(BufferTargetARB.ArrayBuffer, (uint)(sizeof(float) * _skyboxVertices.Length), ptr, BufferUsageARB.StaticDraw);
        }
        
        var vertexShader = gl.CreateShader(ShaderType.VertexShader);
        gl.ShaderSource(vertexShader, VertexCode);
        gl.CompileShader(vertexShader);
        _skyboxShader = gl.CreateShader(ShaderType.FragmentShader);
        gl.ShaderSource(_skyboxShader, FragmentCode);
        gl.CompileShader(_skyboxShader);
        gl.GetShader(vertexShader, GLEnum.CompileStatus, out var status);
        if (status != (int)GLEnum.True)
        {
            var infoLog = gl.GetShaderInfoLog(vertexShader);
            Console.WriteLine($"skybox Vertex shader compile error: {infoLog}");
        }
        _skyboxProgram = gl.CreateProgram();
        gl.AttachShader(_skyboxProgram, vertexShader);
        gl.AttachShader(_skyboxProgram, _skyboxShader);
        gl.LinkProgram(_skyboxProgram);
        gl.GetProgram(_skyboxProgram, GLEnum.LinkStatus, out status);
        if (status != (int)GLEnum.True)
        {
            var infoLog = gl.GetProgramInfoLog(_skyboxProgram);
            Console.WriteLine($"skybox Program link error: {infoLog}");
        }
        gl.DetachShader(_skyboxProgram, vertexShader);
        gl.DetachShader(_skyboxProgram, _skyboxShader);
        gl.DeleteShader(vertexShader);
        gl.DeleteShader(_skyboxShader);

        
        const uint positionLoc = 0;
        gl.EnableVertexAttribArray(positionLoc);
        gl.VertexAttribPointer(positionLoc, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), (void*)0);

        var picturePath = Path.Combine(AppContext.BaseDirectory, "Picture", "BlueSky.jpg");
        string[] paths = [..Enumerable.Repeat(picturePath, 6)];
        LoadTexture(ref gl, paths);
        
        gl.BindVertexArray(0);
    }

    internal unsafe void RenderSkyBox(ref GL gl, Matrix4x4 view, Matrix4x4 projection)
    {
        gl.DepthFunc(DepthFunction.Lequal);
        
        gl.UseProgram(_skyboxProgram);
        gl.BindVertexArray(_skyboxVao);
        
        var viewNoTranslation = new Matrix4x4(view.M11, view.M12, view.M13, 0,
            view.M21, view.M22, view.M23, 0,
            view.M31, view.M32, view.M33, 0,
            0, 0, 0, 1);
        var viewLoc = gl.GetUniformLocation(_skyboxProgram, "view");
        var projectionLoc = gl.GetUniformLocation(_skyboxProgram, "projection");
        gl.UniformMatrix4(viewLoc, 1, false, (float*)&viewNoTranslation);
        gl.UniformMatrix4(projectionLoc, 1, false, (float*)&projection);
        
        gl.ActiveTexture(TextureUnit.Texture0);
        gl.BindTexture(TextureTarget.TextureCubeMap, _skyboxTexture);
        gl.Uniform1(gl.GetUniformLocation(_skyboxProgram, "skybox"), 0);
        
        gl.DrawArrays(PrimitiveType.Triangles, 0, 36);
        gl.BindVertexArray(0);
        
        gl.DepthFunc(DepthFunction.Less);
    }
}