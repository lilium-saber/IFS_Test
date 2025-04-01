using System;
using System.Collections.Generic;
using System.Diagnostics;
using Silk.NET.OpenGL;

namespace AvaloniaApp.Ults;

internal static class OpenGLFunc
{ 
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
    
    internal const string fragmentCodeTemp = """
                                          #version 330 core
                                          out vec4 out_color;
                                          uniform vec4 ourColor;
                                          void main()
                                          {
                                              out_color = ourColor;
                                          }
                                          """;

    internal static string ChangeColorUniform()
    {
        return 
            $"#version 330 core\n" +
            $"out vec4 out_color;\n" +
            $"uniform vec4 ourColor;\n" +
            $"void main()\n" +
            $"{{" +
            $"out_color = ourColor;" +
            $"}}";
    }

    internal static string ChangeCoordTex2()
    {
        return 
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
    }
    
    internal static string ChangeCoordTexUniform()
    {
        return 
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
    }
    
    internal static string ChangeColor(float red, float green, float blue, float alpha)
    {
        return
            $"#version 330 core\n" +
            $"out vec4 out_color;\n" +
            $"void main()\n" +
            $"{{\n" +
            $"out_color = vec4({red}, {green}, {blue}, {alpha});\n" +
            $"}}";
    }
    
    internal static string ChangeColor(float blue, float alpha)
    {
        return
            $"#version 330 core\n" +
            $"in vec2 vColor;" +
            $"out vec4 out_color;\n" +
            $"void main()\n" +
            $"{{\n" +
            $"out_color = vec4(vColor.x, vColor.y, {blue}, {alpha});\n" +
            $"}}";
    }
    
    internal static string ChangeColor(float alpha)
    {
        return
            $"#version 330 core\n" +
            $"in vec3 vColor;" +
            $"out vec4 out_color;\n" +
            $"void main()\n" +
            $"{{\n" +
            $"out_color = vec4(vColor.x, vColor.y, vColor.z, {alpha});\n" +
            $"}}";
    }
    
    // 创建着色器程序
    internal static void CreateShaderProgram(ref GL _gl, ref List<uint> program, ref uint vertexShader, 
        ref List<uint> fragmentShader, int index)
    {
        program.Add(_gl.CreateProgram());
        _gl.AttachShader(program[^1], vertexShader);
        _gl.AttachShader(program[^1], fragmentShader[index]);
        _gl.LinkProgram(program[^1]);
        _gl.GetProgram(program[^1], ProgramPropertyARB.LinkStatus, out var lstatus);
        if (lstatus != (int)GLEnum.True)
        {
            var infoLog = _gl.GetProgramInfoLog(program[^1]);
            Console.WriteLine($"Program Link Error: {infoLog}");
        }
    }
    
    // 片段着色器
    internal static void CreateFragmentShader(ref GL _gl, ref List<uint> shader, string source)
    {
        shader.Add(_gl.CreateShader(ShaderType.FragmentShader));
        _gl.ShaderSource(shader[^1], source);
        _gl.CompileShader(shader[^1]);
        _gl.GetShader(shader[^1], ShaderParameterName.CompileStatus, out var fstatus);
        if (fstatus != (int)GLEnum.True)
        {
            var infoLog = _gl.GetShaderInfoLog(shader[^1]);
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
    internal static void DeleteShader(ref GL _gl, ref List<uint> program, ref List<uint> fragmentShader, ref uint vertexShader)
    {
        var programCount = program.Count;
        for (var i = 0; i < programCount; i++)
        {
            _gl.DetachShader(program[i], vertexShader);
            _gl.DetachShader(program[i], fragmentShader[i]);
        }
        _gl.DeleteShader(vertexShader);
        for (var i = 0; i < programCount; i++)
        {
            _gl.DeleteShader(fragmentShader[i]);
        }
    }
    
}