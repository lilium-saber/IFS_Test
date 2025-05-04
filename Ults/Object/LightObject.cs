using System.Numerics;
using Silk.NET.OpenGL;

namespace AvaloniaApp.Ults.Object;

internal class LightObject
{
    private uint LightVao { get; set; }
    private uint LightVbo { get; set; }
    private uint LightProgram { get; set; }
    private uint LightFragmentShader { get; set; }
    private Matrix4x4 LightModel { get; set; } = Matrix4x4.Identity;
    internal Vector3 LightPosition { get; set; } = new(0.0f, 2.0f, 0.0f);
    internal Vector3 LightColor { get; set; } = new(1.0f, 1.0f, 1.0f);

    private const string LightFragmentCode = """
                                              #version 330 core
                                              out vec4 light_color;
                                              void main()
                                              {
                                                  light_color = vec4(1.0);
                                              }
                                              """;

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

    private readonly float[] _lightPoint = 
        [
            -0.5f, -0.5f, -0.5f,
            0.5f, -0.5f, -0.5f,
            0.5f,  0.5f, -0.5f,
            0.5f,  0.5f, -0.5f,
            -0.5f,  0.5f, -0.5f,
            -0.5f, -0.5f, -0.5f,

            -0.5f, -0.5f,  0.5f,
            0.5f, -0.5f,  0.5f,
            0.5f,  0.5f,  0.5f,
            0.5f,  0.5f,  0.5f,
            -0.5f,  0.5f,  0.5f,
            -0.5f, -0.5f,  0.5f,

            -0.5f,  0.5f,  0.5f,
            -0.5f,  0.5f, -0.5f,
            -0.5f, -0.5f, -0.5f,
            -0.5f, -0.5f, -0.5f,
            -0.5f, -0.5f,  0.5f,
            -0.5f,  0.5f,  0.5f,

            0.5f,  0.5f,  0.5f,
            0.5f,  0.5f, -0.5f,
            0.5f, -0.5f, -0.5f,
            0.5f, -0.5f, -0.5f,
            0.5f, -0.5f,  0.5f,
            0.5f,  0.5f,  0.5f,

            -0.5f, -0.5f, -0.5f,
            0.5f, -0.5f, -0.5f,
            0.5f, -0.5f,  0.5f,
            0.5f, -0.5f,  0.5f,
            -0.5f, -0.5f,  0.5f,
            -0.5f, -0.5f, -0.5f,

            -0.5f,  0.5f, -0.5f,
            0.5f,  0.5f, -0.5f,
            0.5f,  0.5f,  0.5f,
            0.5f,  0.5f,  0.5f,
            -0.5f,  0.5f,  0.5f,
            -0.5f,  0.5f, -0.5f
        ];

    internal unsafe void LoadLight(ref GL gl)
    {
        LightVao = gl.GenVertexArray();
        LightVbo = gl.GenBuffer();
        gl.BindVertexArray(LightVao);
        gl.BindBuffer(BufferTargetARB.ArrayBuffer, LightVbo);
        fixed (float* buf = _lightPoint)
        {
            gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(_lightPoint.Length * sizeof(float)), buf, BufferUsageARB.StaticDraw);
        }

        var vertexShader = gl.CreateShader(ShaderType.VertexShader);
        gl.ShaderSource(vertexShader, LightVertexCode);
        gl.CompileShader(vertexShader);
        
        LightFragmentShader = gl.CreateShader(ShaderType.FragmentShader);
        gl.ShaderSource(LightFragmentShader, LightFragmentCode);
        gl.CompileShader(LightFragmentShader);
        
        LightProgram = gl.CreateProgram();
        gl.AttachShader(LightProgram, vertexShader);
        gl.AttachShader(LightProgram, LightFragmentShader);
        gl.LinkProgram(LightProgram);
        
        gl.DetachShader(LightProgram, vertexShader);
        gl.DetachShader(LightProgram, LightFragmentShader);
        gl.DeleteShader(vertexShader);
        gl.DeleteShader(LightFragmentShader);
        
        const uint positionLoc = 0;
        gl.EnableVertexAttribArray(positionLoc);
        gl.VertexAttribPointer(positionLoc, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), (void*) 0);
        
        gl.BindVertexArray(0);
        gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
        gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);
    }

    internal unsafe void RenderLight(ref GL gl, Matrix4x4 view, Matrix4x4 projection)
    {
        gl.BindVertexArray(LightVao);
        gl.UseProgram(LightProgram);
        var lightModel = LightModel;
        lightModel = Matrix4x4.CreateTranslation(LightPosition) * lightModel;
        lightModel = Matrix4Calculator.Scale(lightModel, 0.2f, 0.2f, 0.2f);
        var modelLoc = gl.GetUniformLocation(LightProgram, "lightModel");
        gl.UniformMatrix4(modelLoc, 1, false, (float*)&lightModel);
        var viewLoc = gl.GetUniformLocation(LightProgram, "lightView");
        gl.UniformMatrix4(viewLoc, 1, false, (float*)&view);
        var projLoc = gl.GetUniformLocation(LightProgram, "lightProjection");
        gl.UniformMatrix4(projLoc, 1, false, (float*)&projection);
        
        gl.DrawArrays(PrimitiveType.Triangles, 0, 36);
        gl.BindVertexArray(0);
    }
}