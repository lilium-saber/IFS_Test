using System;
using System.IO;
using System.Numerics;
using Silk.NET.OpenGL;
using StbImageSharp;

namespace AvaloniaApp.Ults;

internal class EarthObject
{
    internal uint EarthVao { get; set; }
    internal uint EarthVbo { get; set; }
    internal uint EarthProgram { get; set; }
    internal uint EarthFragmentShader { get; set; }
    internal uint EarthTexture { get; set; } 
    internal Matrix4x4 EarthModel { get; set; } = Matrix4x4.Identity;// 作为地面不需要改变位置
    internal Vector3 EarthColor { get; set; } = new(0.486f, 0.988f, 0.0f); // 地面颜色
    
    private const string VertexCode = """
                                      #version 330 core
                                      layout(location = 0) in vec3 aPosition;
                                      layout(location = 1) in vec3 aNormal;
                                      //layout(location = 2) in vec2 aTexCoord;
                                      out vec3 Normal;
                                      out vec3 FragPos;
                                      out vec2 TexCoord;
                                      uniform mat4 model;
                                      uniform mat4 view;
                                      uniform mat4 projection;
                                      void main()
                                      {
                                          gl_Position = projection * view * model * vec4(aPosition, 1.0);
                                          FragPos = vec3(model * vec4(aPosition, 1.0));
                                          Normal = mat3(transpose(inverse(model))) * aNormal;
                                          //TexCoord = aTexCoord;
                                      }
                                      """;

    private const string FragmentCode = """
                                        #version 330 core
                                        out vec4 out_color;
                                        in vec3 Normal;
                                        in vec3 FragPos;
                                        //in vec2 TexCoord;
                                        //uniform sampler2D texture;
                                        uniform vec3 objectColor;
                                        uniform vec3 lightColor;
                                        uniform vec3 lightPos;
                                        uniform vec3 viewPos;
                                        uniform float minLight;
                                        uniform float attenuation;
                                        void main()
                                        {
                                            float specularStrength = 0.5;
                                            vec3 norm = normalize(Normal);
                                            vec3 lightDir = normalize(lightPos - FragPos);
                                            
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
                                            vec3 result = (ambient + diffuse + specular) * objectColor;
                                            result = max(result, vec3(minLight));
                                            //vec4 textureColor = texture(texture, TexCoord);
                                            out_color = vec4(result, 1.0);
                                        }
                                        """;

    private static readonly float[] PointSet = 
        [
            5.0f, 0.0f, 5.0f,   0.0f, 1.0f, 0.0f, 1.0f, 1.0f,
            5.0f, 0.0f, -5.0f,  0.0f, 1.0f, 0.0f, 1.0f, 0.0f,
            -5.0f, 0.0f, -5.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f,
            -5.0f, 0.0f, -5.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f,
            -5.0f, 0.0f, 5.0f,  0.0f, 1.0f, 0.0f, 0.0f, 1.0f,
            5.0f, 0.0f, 5.0f,   0.0f, 1.0f, 0.0f, 1.0f, 1.0f,
        ];
    
    internal unsafe void LoadEarthObject(ref GL gl)
    {
        EarthVao = gl.GenVertexArray();
        EarthVbo = gl.GenBuffer();
        gl.BindVertexArray(EarthVao);
        gl.BindBuffer(BufferTargetARB.ArrayBuffer, EarthVbo);
        fixed (float* buf = PointSet)
        {
            gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(PointSet.Length * sizeof(float)), buf, BufferUsageARB.StaticDraw); 
        }
        var vertexShader = gl.CreateShader(ShaderType.VertexShader);
        gl.ShaderSource(vertexShader, VertexCode);
        gl.CompileShader(vertexShader);
        EarthFragmentShader = gl.CreateShader(ShaderType.FragmentShader);
        gl.ShaderSource(EarthFragmentShader, FragmentCode);
        gl.CompileShader(EarthFragmentShader);
        EarthProgram = gl.CreateProgram();
        gl.AttachShader(EarthProgram, vertexShader);
        gl.AttachShader(EarthProgram, EarthFragmentShader);
        gl.LinkProgram(EarthProgram);
        gl.DetachShader(EarthProgram, vertexShader);
        gl.DetachShader(EarthProgram, EarthFragmentShader);
        gl.DeleteShader(vertexShader);
        gl.DeleteShader(EarthFragmentShader);
        
        const uint positionLoc = 0;
        gl.EnableVertexAttribArray(positionLoc);
        gl.VertexAttribPointer(positionLoc, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), (void*) 0);
        const uint normalLoc = 1;
        gl.EnableVertexAttribArray(normalLoc);
        gl.VertexAttribPointer(normalLoc, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), (void*) (3 * sizeof(float)));
        // const uint texCoordLoc = 2;
        // gl.EnableVertexAttribArray(texCoordLoc);
        // gl.VertexAttribPointer(texCoordLoc, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), (void*) (6 * sizeof(float)));
        
        // var path = Path.Combine(AppContext.BaseDirectory, "Picture", "earth.jpeg");
        // LoadTexture(ref gl, path);
        
        gl.BindVertexArray(0);
        gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
        gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);
    }

    internal unsafe void RenderEarthObject(ref GL gl, Matrix4x4 view, Matrix4x4 projection,  Vector3 lightPos, Vector3 lightColor, Vector3 viewPos)
    {
        gl.UseProgram(EarthProgram);
        gl.BindVertexArray(EarthVao);
        
        // var axis = new Vector3(0.5f, 1.0f, 0);
        var model = Matrix4x4.CreateScale(100.0f) * EarthModel;
        var modelLoc = gl.GetUniformLocation(EarthProgram, "model");
        gl.UniformMatrix4(modelLoc, 1, false, (float*)&model);
        var viewLoc = gl.GetUniformLocation(EarthProgram, "view");
        gl.UniformMatrix4(viewLoc, 1, false, (float*)&view);
        var projLoc = gl.GetUniformLocation(EarthProgram, "projection");
        gl.UniformMatrix4(projLoc, 1, false, (float*)&projection);
        
        // gl.ActiveTexture(TextureUnit.Texture0);
        // gl.BindTexture(TextureTarget.Texture2D, EarthTexture);
        // gl.Uniform1(gl.GetUniformLocation(EarthProgram, "texture"), 0);
        
        gl.Uniform3(gl.GetUniformLocation(EarthProgram, "lightColor"), lightColor);
        gl.Uniform3(gl.GetUniformLocation(EarthProgram, "objectColor"), EarthColor);
        gl.Uniform3(gl.GetUniformLocation(EarthProgram, "lightPos"), lightPos);  
        gl.Uniform3(gl.GetUniformLocation(EarthProgram, "viewPos"), viewPos); // 用于计算光照
        gl.Uniform1(gl.GetUniformLocation(EarthProgram, "minLight"), OpenGlUlts.MinLightStrength);
        
        var objectCenter = new Vector3(model.M41, model.M42, model.M43);
        var distance = Vector3.Distance(objectCenter, lightPos);
        var attenuationFactor = 1.0f / (1.0f + 0.007f * distance + 0.0005f * distance * distance);
        gl.Uniform1(gl.GetUniformLocation(EarthProgram, "attenuation"), attenuationFactor);

        gl.DrawArrays(PrimitiveType.Triangles, 0, 6);
    }
    
    private unsafe void LoadTexture(ref GL gl, string pngPath, int pictureType = 0)
    {
        if (!File.Exists(pngPath))
        {
            Console.WriteLine($"Texture file not found: {pngPath}");
            return;
        }
        Console.WriteLine($"{pngPath} Loading texture...");
        EarthTexture = gl.GenTexture();
        gl.BindTexture(TextureTarget.Texture2D, EarthTexture);
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

    internal EarthObject() { }
}