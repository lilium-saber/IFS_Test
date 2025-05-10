using System.Collections.Generic;
using System.Drawing.Printing;
using AvaloniaApp.ViewModel;

namespace AvaloniaApp.Ults;

internal static class OpenGlUlts
{
    private static float _maxLightStrength = 0.25f;

    internal static float MinLightStrength
    {
        get => _maxLightStrength;
        set => _maxLightStrength = value <= 0.1f ? 0.1f : value;
    }
    
    internal static int SocketPost { get; set; } = 12345;

    public static X3dIfs Tuple2X3dIfs(
        (decimal a, decimal b, decimal c, decimal d, decimal e, decimal f, decimal g, decimal h,
            decimal k, decimal ur, decimal vr, decimal rr, decimal p) tuple) =>
        new(tuple.a, tuple.b, tuple.c, tuple.d, tuple.e, tuple.f, tuple.g, tuple.h,
            tuple.k, tuple.ur, tuple.vr, tuple.rr, tuple.p);
    
    public static (decimal a, decimal b, decimal c, decimal d, decimal e, decimal f, decimal g, decimal h,
        decimal k, decimal ur, decimal vr, decimal rr, decimal p) X3dIfs2Tuple(X3dIfs ifs) =>
        (ifs.A, ifs.B, ifs.C, ifs.D, ifs.E, ifs.F, ifs.G, ifs.H,
            ifs.K, ifs.Ur, ifs.Vr, ifs.Rr, ifs.P);
    
    public static X3dIfs X2dIfs2X3dIfs(
        (decimal a, decimal b, decimal c, decimal d, decimal e, decimal f, decimal p) x2dIfs) =>
        new(x2dIfs.a, x2dIfs.b, 0, x2dIfs.c, x2dIfs.d, 0, 0, 0, 0, x2dIfs.e, x2dIfs.f, 0, x2dIfs.p);

    public static X3dIfs IfsDataX3d2X3dIfs(IfsDataX3d ifs) => 
        new(ifs.A, ifs.B, ifs.C, ifs.D, ifs.E, ifs.F, ifs.G, ifs.H, ifs.K, ifs.Ur, ifs.Vr, ifs.Rr, ifs.P);
}