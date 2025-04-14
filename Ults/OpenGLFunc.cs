using System;
using System.Collections.Generic;
using System.Diagnostics;
using Silk.NET.OpenGL;

namespace AvaloniaApp.Ults;

internal static class OpenGLFunc
{ 
    private const string LightVertexCode = """
                                       #version 330 core
                                       layout (location = 0) in vec3 lightPos;
                                       uniform mat4 lightModel;
                                       uniform mat4 lightView;
                                       uniform mat4 lightProjection;
                                       void main()
                                       {
                                           gl_Position = lightProjection * lightView * lightModel * vec4(lightPos, 1.0);
                                       }
                                       """;
    private const string vertexCode3 = """
                                         #version 330 core
                                         layout(location = 0) in vec3 aPosition;
                                         out vec2 vColor;
                                         void main()
                                         {
                                             gl_Position = vec4(aPosition, 1.0);
                                         }
                                       """;
    private const string vertexCode5 = """
                                        #version 330 core
                                        layout(location = 0) in vec3 aPosition;
                                        layout(location = 1) in vec2 aColor;
                                        out vec2 vColor;
                                        void main()
                                        {
                                            gl_Position = vec4(aPosition, 1.0);
                                            vColor = aColor;
                                        }
                                      """; // 着色器Shader, 创建顶点着色器
    private const string vertexCode6 = """
                                         #version 330 core
                                         layout(location = 0) in vec3 aPosition;
                                         layout(location = 1) in vec3 aColor;
                                         out vec3 vColor;
                                         void main()
                                         {
                                             gl_Position = vec4(aPosition, 1.0);
                                             vColor = aColor;
                                         }
                                       """;
    private const string vertexCodeTex = """
                                       #version 330 core
                                       layout (location = 0) in vec3 aPosition;
                                       layout (location = 1) in vec3 aColor;
                                       layout (location = 2) in vec2 aTexCoord;
                                       out vec3 vColor;
                                       out vec2 TexCoord;
                                       void main()
                                       {
                                           gl_Position = vec4(aPosition, 1.0);
                                           vColor = aColor;
                                           TexCoord = aTexCoord;
                                       }
                                       """;

    private const string vertexCode6X3D = """
                                          #version 330 core
                                          layout(location = 0) in vec3 aPosition;
                                          layout(location = 1) in vec3 aColor;
                                          out vec3 vColor;
                                          uniform mat4 projection;
                                          uniform mat4 view;
                                          uniform mat4 model;
                                          void main()
                                          {
                                              gl_Position = projection * view * model * vec4(aPosition, 1.0);
                                              vColor = aColor;
                                          }
                                          """;
    // 临时着色器, 测试用
    private const string vertexCodeTemp = """
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
        // """
        //                                   #version 330 core
        //                                   layout(location = 0) in vec3 aPosition;
        //                                   layout(location = 1) in vec2 aTexCoord;
        //                                   out vec2 TexCoord;
        //                                   uniform mat4 model;
        //                                   uniform mat4 view;
        //                                   uniform mat4 projection;
        //                                   void main()
        //                                   {
        //                                       gl_Position = projection * view * model * vec4(aPosition, 1.0);
        //                                       TexCoord = vec2(aTexCoord.x, 1.0 - aTexCoord.y);
        //                                   }
        //                                   """;
    
    internal const string fragmentCodeTemp = """
                                             #version 330 core
                                             out vec4 out_color;
                                             uniform sampler2D ourTexture0; 
                                             uniform vec3 objectColor;
                                             uniform vec3 lightColor;
                                             uniform vec3 lightPos;
                                             uniform vec3 viewPos;
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
                                                 
                                                 vec3 viewDir = normalize(viewPos - FragPos);
                                                 vec3 reflectDir = reflect(-lightDir, norm);
                                                 float spec = pow(max(dot(viewDir, reflectDir), 0.0), 128);
                                                 vec3 specular = specularStrength * spec * lightColor;
                                                 
                                                 vec3 ambient = 0.1 * lightColor;
                                                 float diff = max(dot(norm, lightDir), 0.0);
                                                 vec3 diffuse = diff * lightColor;
                                                 vec3 result = (ambient + diffuse + specular) * objectColor;
                                                 out_color = vec4(texColor.rgb * result, texColor.a);
                                             }
                                             """;
        // """
        //                                   #version 330 core
        //                                   out vec4 out_color;
        //                                   uniform sampler2D ourTexture0;
        //                                   uniform sampler2D ourTexture1;
        //                                   in vec2 TexCoord;
        //                                   void main()
        //                                   {
        //                                       out_color = mix(texture(ourTexture0, TexCoord), texture(ourTexture1, TexCoord), 0.2);
        //                                   }
        //                                   """;
        
    internal static string LightFragmentCode() =>
            $"#version 330 core\n" +
            $"out vec4 light_color;\n" +
            $"void main()\n" +
            $"{{\n" +
            $"light_color = vec4(1.0);\n" +
            $"}}";

    internal static string ChangeColorUniform() =>
            $"#version 330 core\n" +
            $"out vec4 out_color;\n" +
            $"uniform vec4 ourColor;\n" +
            $"void main()\n" +
            $"{{" +
            $"out_color = ourColor;" +
            $"}}";

    internal static string ChangeCoordTex2() =>
            $"#version 330 core\n" +
            $"out vec4 out_color;\n" +
            $"in vec3 vColor;\n" +
            $"in vec2 TexCoord;\n" +
            $"uniform sampler2D ourTexture;\n" +
            $"uniform sampler2D ourTexture0;\n" +
            $"void main()\n" +
            $"{{\n" +
            $"vec4 color1 = texture(ourTexture, TexCoord);\n" +
            $"vec4 color2 = texture(ourTexture0, TexCoord);\n" +
            $"out_color = mix(color1, color2, 0.5);\n" +
            $"}}";
    
    internal static string ChangeCoordTexUniform() =>
            $"#version 330 core\n" +
            $"out vec4 out_color;\n" +
            $"in vec3 vColor;\n" +
            $"in vec2 TexCoord;\n" +
            $"uniform sampler2D ourTexture;\n" +
            $"uniform vec4 ourColor;\n" +
            $"void main()\n" +
            $"{{\n" +
            $"out_color = texture(ourTexture, TexCoord) * ourColor;" +
            $"}}";
    
    internal static string ChangeColor(float red, float green, float blue, float alpha) =>
            $"#version 330 core\n" +
            $"out vec4 out_color;\n" +
            $"void main()\n" +
            $"{{\n" +
            $"out_color = vec4({red}, {green}, {blue}, {alpha});\n" +
            $"}}";
    
    internal static string ChangeColor(float blue, float alpha) =>
            $"#version 330 core\n" +
            $"in vec2 vColor;" +
            $"out vec4 out_color;\n" +
            $"void main()\n" +
            $"{{\n" +
            $"out_color = vec4(vColor.x, vColor.y, {blue}, {alpha});\n" +
            $"}}";
    
    internal static string ChangeColor(float alpha) =>
            $"#version 330 core\n" +
            $"in vec3 vColor;" +
            $"out vec4 out_color;\n" +
            $"void main()\n" +
            $"{{\n" +
            $"out_color = vec4(vColor.x, vColor.y, vColor.z, {alpha});\n" +
            $"}}";
    
    // 创建着色器程序
    // 使用的program与fragmentShader是最后创建的
    internal static void CreateShaderProgram(ref GL gl, ref List<uint> program, ref uint vertexShader, 
        ref List<uint> fragmentShader, int index = -1)
    {
        program.Add(gl.CreateProgram());
        gl.AttachShader(program[^1], vertexShader);
        gl.AttachShader(program[^1], index == -1 ? fragmentShader[^1] : fragmentShader[index]);
        gl.LinkProgram(program[^1]);
        gl.GetProgram(program[^1], ProgramPropertyARB.LinkStatus, out var lstatus);
        if (lstatus != (int)GLEnum.True)
        {
            var infoLog = gl.GetProgramInfoLog(program[^1]);
            Console.WriteLine($"Program Link Error: {infoLog}");
        }
    }
    
    // 片段着色器
    internal static void CreateFragmentShader(ref GL gl, ref List<uint> shader, string source)
    {
        shader.Add(gl.CreateShader(ShaderType.FragmentShader));
        gl.ShaderSource(shader[^1], source);
        gl.CompileShader(shader[^1]);
        gl.GetShader(shader[^1], ShaderParameterName.CompileStatus, out var fstatus);
        if (fstatus != (int)GLEnum.True)
        {
            var infoLog = gl.GetShaderInfoLog(shader[^1]);
            Console.WriteLine($"Fragment Shader Error: {infoLog}");
        }
    }
    
    // 片段着色器
    internal static void CreateVertexShader(ref GL _gl, ref uint shader, VertexCodeType type)
    {
        shader = _gl.CreateShader(ShaderType.VertexShader);
        switch (type)
        {
            case VertexCodeType.Input3:
                _gl.ShaderSource(shader, vertexCode3);
                break;
            case VertexCodeType.Input5:
                _gl.ShaderSource(shader, vertexCode5);
                break;
            case VertexCodeType.Input6:
                _gl.ShaderSource(shader, vertexCode6);
                break;
            case VertexCodeType.InputTex:
                _gl.ShaderSource(shader, vertexCodeTex);
                break;
            case VertexCodeType.Input6X3D:
                _gl.ShaderSource(shader, vertexCode6X3D);
                break;
            case VertexCodeType.InputTemp:
                _gl.ShaderSource(shader, vertexCodeTemp);
                break;
            case VertexCodeType.Light:
                _gl.ShaderSource(shader, LightVertexCode);
                break;
            default:
                _gl.ShaderSource(shader, vertexCode3);
                break;
        }
        _gl.CompileShader(shader);
        _gl.GetShader(shader, ShaderParameterName.CompileStatus, out var vtatus);
        if (vtatus != (int)GLEnum.True)
        {
            var infoLog = _gl.GetShaderInfoLog(shader);
            Console.WriteLine($"Vertex Shader Error: {infoLog}");
        }
    }
    
    // 删除着色器, program与fragmentShader长度相同
    internal static void DeleteShader(ref GL gl, ref List<uint> program, ref List<uint> fragmentShader, ref uint vertexShader, int index)
    {
        gl.DetachShader(program[index], vertexShader);
        gl.DetachShader(program[index], fragmentShader[index]);
        gl.DeleteShader(vertexShader);
        gl.DeleteShader(fragmentShader[index]);
    }
    
}