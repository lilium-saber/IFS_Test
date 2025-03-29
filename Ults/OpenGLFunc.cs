using System;
using System.Collections.Generic;
using Silk.NET.OpenGL;

namespace AvaloniaApp.Ults;

internal static class OpenGLFunc
{ 
    private const string vertexCode = """
                                        #version 330 core
                                        layout(location = 0) in vec3 aPosition;
                                        void main()
                                        {
                                            gl_Position = vec4(aPosition, 1.0);
                                        }
                                      """; // 着色器Shader, 创建顶点着色器
    
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
    internal static void CreateVertexShader(ref GL _gl, ref uint shader)
    {
        var vertexShader = _gl.CreateShader(ShaderType.VertexShader);
        _gl.ShaderSource(vertexShader, vertexCode);
        _gl.CompileShader(vertexShader);
        _gl.GetShader(vertexShader, ShaderParameterName.CompileStatus, out var vtatus);
        if (vtatus != (int)GLEnum.True)
        {
            var infoLog = _gl.GetShaderInfoLog(vertexShader);
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
}