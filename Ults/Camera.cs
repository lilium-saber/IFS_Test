using System;
using System.Numerics;

namespace AvaloniaApp.Ults;

public class Camera
{
    public Matrix4x4 viewMatrix => Matrix4x4.CreateLookAt(cameraPos, cameraPos + cameraFront, cameraUp); // 第一个参数是定义的摄像机位置, 第二个参数是摄像机朝向的目标点, 第三个参数是表示世界空间的上方向
    public Vector3 cameraPos { get; set; } = new Vector3(0.0f, 0.0f, 3.0f);
    public Vector3 cameraFront { get; set; } = new Vector3(0.0f, 0.0f, -1.0f);
    public Vector3 cameraUp { get; set; } = new Vector3(0.0f, 1.0f, 0.0f);
    public float fov { get; set; } = 45.0f; // 视场角
    public float cameraSpeed { get; set; } = 0.05f;
    
    public void CameraReset()
    {
        cameraPos = new Vector3(0.0f, 0.0f, 3.0f);
        cameraFront = new Vector3(0.0f, 0.0f, -1.0f);
        cameraUp = new Vector3(0.0f, 1.0f, 0.0f);
        fov = 45.0f;
        cameraSpeed = 0.05f;
    }
}