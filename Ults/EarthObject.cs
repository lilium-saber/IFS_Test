﻿using System.Numerics;
using Silk.NET.OpenGL;

namespace AvaloniaApp.Ults;

internal class EarthObject
{
    internal uint EarthVao { get; set; }
    internal uint EarthVbo { get; set; }
    internal uint EarthProgram { get; set; }
    internal uint EarthFragmentShader { get; set; }
    internal Matrix4x4 EarthModel { get; set; } = Matrix4x4.Identity; // 作为地面不需要改变位置
    
    private const string VertexCode = """
                                      #version 330 core
                                      layout(location = 0) in vec3 aPosition;
                                      layout(location = 1) in vec3 aNormal;
                                      out vec3 Normal;
                                      out vec3 FragPos;
                                      uniform mat4 model;
                                      uniform mat4 view;
                                      uniform mat4 projection;
                                      void main()
                                      {
                                          gl_Position = projection * view * model * vec4(aPosition, 1.0);
                                          FragPos = vec3(model * vec4(aPosition, 1.0));
                                          Normal = mat3(transpose(inverse(model))) * aNormal;
                                      }
                                      """;

    private const string FragmentCode = """
                                        #version 330 core
                                        out vec4 out_color;
                                        in vec3 Normal;
                                        in vec3 FragPos;
                                        uniform vec3 objectColor;
                                        uniform vec3 lightColor;
                                        uniform vec3 lightPos;
                                        uniform vec3 viewPos;
                                        void main()
                                        {
                                            float specularStrength = 0.5;
                                            
                                            vec3 norm = normalize(Normal);
                                            vec3 lightDir = normalize(lightPos - FragPos);
                                            
                                            vec3 viewDir = normalize(viewPos - FragPos);
                                            vec3 reflectDir = reflect(-lightDir, norm);
                                            float spec = pow(max(dot(viewDir, reflectDir), 0.0), 128);
                                            vec3 specular = specularStrength * spec * lightColor;
                                            
                                            vec3 ambient = 0.1 * lightColor;
                                            float diff = max(dot(norm, lightDir), 0.0);
                                            vec3 diffuse = diff * lightColor;
                                            vec3 result = (ambient + diffuse + specular) * objectColor;
                                            out_color = vec4(result, 1.0);
                                        }
                                        """;

    private static readonly float[] PointSet = 
        [
            5.0f, 0.0f, 5.0f,   0.0f, 1.0f, 0.0f,
            5.0f, 0.0f, -5.0f,  0.0f, 1.0f, 0.0f,
            -5.0f, 0.0f, -5.0f, 0.0f, 1.0f, 0.0f,
            -5.0f, 0.0f, -5.0f, 0.0f, 1.0f, 0.0f,
            -5.0f, 0.0f, 5.0f,  0.0f, 1.0f, 0.0f,
            5.0f, 0.0f, 5.0f,   0.0f, 1.0f, 0.0f,
        ];
    
    internal unsafe void LoadEarthObject(ref GL gl)
    {
        
    }

    internal unsafe void RenderEarthObject(ref GL gl, Matrix4x4 view, Matrix4x4 projection)
    {
        
    }
}