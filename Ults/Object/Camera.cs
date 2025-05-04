using System.Numerics;

namespace AvaloniaApp.Ults.Object;

public class Camera
{
    public Matrix4x4 ViewMatrix => Matrix4x4.CreateLookAt(CameraPos, CameraPos + CameraFront, CameraUp); // 第一个参数是定义的摄像机位置, 第二个参数是摄像机朝向的目标点, 第三个参数是表示世界空间的上方向
    public Vector3 CameraPos { get; set; } = new Vector3(0.0f, 0.0f, 80.0f);
    public Vector3 CameraFront { get; set; } = new Vector3(0.0f, 0.0f, -1.0f);
    public Vector3 CameraUp { get; set; } = new Vector3(0.0f, 1.0f, 0.0f);
    public float Fov { get; set; } = 45.0f; // 视场角
    public float CameraSpeed { get; set; } = 60.0f;
    public float Sensitivity { get; set; } = 0.005f; // 鼠标灵敏度
    
    public void CameraReset() { }
}