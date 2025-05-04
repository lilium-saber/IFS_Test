using System;
using System.IO;
using System.Numerics;
using Silk.NET.OpenGL;
using StbImageSharp;

namespace AvaloniaApp.Ults.Object;

internal class IfsResObject
{
    private uint Vao {get; set;}
    private uint Vbo {get; set;}
    private uint Program {get; set;}
    private uint FragmentShader {get; set;}
    private uint Texture {get; set;}
    
    internal Matrix4x4 ModelMatrix {get; set;} = Matrix4x4.Identity;
    internal Vector3 ObjectColor {get; set;} = new(1.0f, 0.5f, 0.3f); 
    
    private readonly float[] _vertex = 
        [
            -0.5f, -0.5f, 0.0f,  0.0f, 0.0f,  1.0f, 0.0f, 1.0f,
            0.5f, -0.5f, 0.0f,  1.0f, 0.0f,  1.0f, 0.0f, 1.0f,
            0.5f,  0.5f, 0.0f,  1.0f, 1.0f,  1.0f, 0.0f, 1.0f,
            0.5f,  0.5f, 0.0f,  1.0f, 1.0f,  1.0f, 0.0f, 1.0f,
            -0.5f,  0.5f, 0.0f,  0.0f, 1.0f,  1.0f, 0.0f, 1.0f,
            -0.5f, -0.5f, 0.0f,  0.0f, 0.0f,  1.0f, 0.0f, 1.0f,

            0.0f, -0.5f, -0.5f,  0.0f, 0.0f,  1.0f, 0.0f, 1.0f,
            0.0f,  0.5f, -0.5f,  0.0f, 1.0f,  1.0f, 0.0f, 1.0f,
            0.0f,  0.5f,  0.5f,  1.0f, 1.0f,  1.0f, 0.0f, 1.0f,
            0.0f,  0.5f,  0.5f,  1.0f, 1.0f,  1.0f, 0.0f, 1.0f,
            0.0f, -0.5f,  0.5f,  1.0f, 0.0f,  1.0f, 0.0f, 1.0f,
            0.0f, -0.5f, -0.5f,  0.0f, 0.0f,  1.0f, 0.0f, 1.0f,
        ];
    private const string VertexCode =   """
                                        #version 330 core
                                        layout(location = 0) in vec3 aPosition;
                                        layout(location = 1) in vec2 aTexCoord;
                                        layout(location = 2) in vec3 aNormal;
                                        out vec2 TexCoord;
                                        out vec3 Normal;
                                        out vec3 FragPos;
                                        uniform mat4 model;
                                        uniform mat4 view;
                                        uniform mat4 projection;
                                        void main()
                                        {
                                            gl_Position = projection * view * model * vec4(aPosition, 1.0);
                                            TexCoord = vec2(aTexCoord.x, 1.0 - aTexCoord.y);
                                            FragPos = vec3(model * vec4(aPosition, 1.0));
                                            Normal = mat3(transpose(inverse(model))) * aNormal;
                                        }
                                        """;
    private const string FragmentCode = """
                                        #version 330 core
                                        out vec4 out_color;
                                        uniform sampler2D ourTexture0; 
                                        uniform vec3 objectColor;
                                        uniform vec3 lightColor;
                                        uniform vec3 lightPos;
                                        uniform vec3 viewPos;
                                        uniform float linear2;
                                        uniform float quadratic3;
                                        uniform float minLight;
                                        in vec2 TexCoord;
                                        in vec3 Normal;
                                        in vec3 FragPos;
                                        void main()
                                        {
                                            float specularStrength = 0.5;
                                            vec4 texColor = texture(ourTexture0, TexCoord);
                                            if(texColor.a < 0.1)
                                            {
                                                discard;
                                            }
                                            
                                            vec3 norm = normalize(Normal);
                                            vec3 lightDir = normalize(lightPos - FragPos);
                                            
                                            float distance = length(lightPos - FragPos);
                                            float attenuation = 1.0 / (1.0 + linear2 * distance + quadratic3 * (distance * distance));
                                            
                                            vec3 viewDir = normalize(viewPos - FragPos);
                                            vec3 reflectDir = reflect(-lightDir, norm);
                                            float spec = pow(max(dot(viewDir, reflectDir), 0.0), 128);
                                            vec3 specular = specularStrength * spec * lightColor;
                                            
                                            vec3 ambient = 0.25 * lightColor;
                                            float diff = max(dot(norm, lightDir), 0.0);
                                            vec3 diffuse = diff * lightColor;
                                            
                                            ambient *= attenuation;
                                            diffuse *= attenuation;
                                            specular *= attenuation;
                                            vec3 result = (ambient + diffuse + specular);
                                            result = max(result, vec3(minLight));
                                            out_color = vec4(texColor.rgb * result, texColor.a);
                                        }
                                        """;

    internal unsafe void LoadResObject(ref GL gl, ref byte[] pictureBytes)
    {
        Vao = gl.GenVertexArray();
        gl.BindVertexArray(Vao);
        Vbo = gl.GenBuffer();
        gl.BindBuffer(BufferTargetARB.ArrayBuffer, Vbo); //ArrayBuffer: 顶点缓冲区目标, ElementArrayBuffer: 数组缓冲区目标, UniformBuffer: 统一缓冲区目标
        fixed (float* buf = _vertex)
        {
            gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(_vertex.Length * sizeof(float)), buf, BufferUsageARB.StaticDraw); //StaticDraw- 设置一次数据, DynamicDraw- 多次更新数据
        }
        var vertexShader = gl.CreateShader(ShaderType.VertexShader);
        gl.ShaderSource(vertexShader, VertexCode);
        gl.CompileShader(vertexShader);
        FragmentShader = gl.CreateShader(ShaderType.FragmentShader);
        gl.ShaderSource(FragmentShader, FragmentCode);
        gl.CompileShader(FragmentShader);
        Program = gl.CreateProgram();
        gl.AttachShader(Program, vertexShader);
        gl.AttachShader(Program, FragmentShader);
        gl.LinkProgram(Program);
        gl.DetachShader(Program, vertexShader);
        gl.DetachShader(Program, FragmentShader);
        gl.DeleteShader(vertexShader);
        gl.DeleteShader(FragmentShader);
        
        const uint positionLoc = 0;
        gl.EnableVertexAttribArray(positionLoc);
        gl.VertexAttribPointer(positionLoc, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), (void*) 0);
        const uint texCoordLoc = 1;
        gl.EnableVertexAttribArray(texCoordLoc);
        gl.VertexAttribPointer(texCoordLoc, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), (void*) (3 * sizeof(float)));
        const uint normalLoc = 2;
        gl.EnableVertexAttribArray(normalLoc);
        gl.VertexAttribPointer(normalLoc, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), (void*) (5 * sizeof(float)));
        
        LoadTexture(ref gl, ref pictureBytes);
        
        gl.BindVertexArray(0);
        gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
        gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);
    }
    
    internal unsafe void DrawResObject(ref GL gl, Matrix4x4 view, Matrix4x4 proj, Vector3 lightPos, Vector3 lightColor, Vector3 viewPos)
    {
        gl.UseProgram(Program);
        gl.BindVertexArray(Vao);
        // var axis = new Vector3(0.5f, 1.0f, 0);
        // var model = Matrix4x4.CreateFromAxisAngle(axis, 50.0f) * ModelMatrix;
        var model = Matrix4x4.CreateTranslation(0.0f, 0.5f, 0.0f) * ModelMatrix;
        var modelLoc = gl.GetUniformLocation(Program, "model");
        gl.UniformMatrix4(modelLoc, 1, false, (float*)&model);
        var viewLoc = gl.GetUniformLocation(Program, "view");
        gl.UniformMatrix4(viewLoc, 1, false, (float*)&view);
        var projLoc = gl.GetUniformLocation(Program, "projection");
        gl.UniformMatrix4(projLoc, 1, false, (float*)&proj);
        
        gl.ActiveTexture(TextureUnit.Texture0);
        gl.BindTexture(TextureTarget.Texture2D, Texture);
        gl.Uniform1(gl.GetUniformLocation(Program, "ourTexture0"), 0);
        
        gl.Uniform3(gl.GetUniformLocation(Program, "lightColor"), lightColor);
        gl.Uniform3(gl.GetUniformLocation(Program, "objectColor"), ObjectColor);
        gl.Uniform3(gl.GetUniformLocation(Program, "lightPos"), lightPos);  
        gl.Uniform3(gl.GetUniformLocation(Program, "viewPos"), viewPos); // 用于计算光照
        gl.Uniform1(gl.GetUniformLocation(Program, "linear2"), 0.09f);
        gl.Uniform1(gl.GetUniformLocation(Program, "quadratic3"), 0.032f);
        gl.Uniform1(gl.GetUniformLocation(Program, "minLight"), OpenGlUlts.MinLightStrength);
        gl.DrawArrays(PrimitiveType.Triangles, 0, 12);
        gl.BindVertexArray(0);
    }

    private unsafe void LoadTexture(ref GL gl, ref byte[] picture)
    {
        if (picture.Length == 0)
        {
            Console.WriteLine("Texture file not found");
            return;
        }
        Console.WriteLine($"Loading texture...");
        using var imageStream = new MemoryStream(picture);
        var image = ImageResult.FromStream(imageStream, ColorComponents.RedGreenBlueAlpha);
        Texture = gl.GenTexture();
        gl.BindTexture(TextureTarget.Texture2D, Texture);
        gl.TexParameter(TextureTarget.Texture2D, GLEnum.TextureWrapS, (int)TextureWrapMode.Repeat);
        gl.TexParameter(TextureTarget.Texture2D, GLEnum.TextureWrapT, (int)TextureWrapMode.Repeat);
        gl.TexParameter(TextureTarget.Texture2D, GLEnum.TextureMinFilter, (int)TextureMinFilter.Linear);
        gl.TexParameter(TextureTarget.Texture2D, GLEnum.TextureMagFilter, (int)TextureMagFilter.Linear);
        
        fixed (byte* data = image.Data)
        {
            gl.TexImage2D(TextureTarget.Texture2D, 0, (int)InternalFormat.Rgba,
                IFS_line.Ults.Ults.BitmapLen, IFS_line.Ults.Ults.BitmapLen, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data);
        }
        
        gl.GenerateMipmap(TextureTarget.Texture2D);
    }

    internal IfsResObject() { }
}