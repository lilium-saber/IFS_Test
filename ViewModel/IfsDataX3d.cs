using System.ComponentModel;
using AvaloniaApp.Ults;

namespace AvaloniaApp.ViewModel;

public sealed class IfsDataX3d : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    
    public decimal A { get; set; } = 0;
    public decimal B { get; set; } = 0;
    public decimal C { get; set; } = 0;
    public decimal D { get; set; } = 0;
    public decimal E { get; set; } = 0;
    public decimal F { get; set; } = 0;
    public decimal G { get; set; } = 0;
    public decimal H { get; set; } = 0;
    public decimal K { get; set; } = 0;
    public decimal Ur { get; set; } = 0;
    public decimal Vr { get; set; } = 0;
    public decimal Rr { get; set; } = 0;
    public decimal P { get; set; } = 0;
    
    public IfsDataX3d(decimal a = 0, decimal b = 0, decimal c = 0, decimal d = 0, decimal e = 0, decimal f = 0,
        decimal g = 0, decimal h = 0, decimal k = 0, decimal ur = 0, decimal vr = 0, decimal rr = 0, decimal p = 0)
    {
        A = a;
        B = b;
        C = c;
        D = d;
        E = e;
        F = f;
        G = g;
        H = h;
        K = k;
        Ur = ur;
        Vr = vr;
        Rr = rr;
        P = p;
    }

    public IfsDataX3d(IfsData ifsData)
    {
        A = ifsData.a;
        B = ifsData.b;
        C = ifsData.c;
        D = ifsData.d;
        Ur = ifsData.e;
        Vr = ifsData.f;
        P = ifsData.p;
    }

    public IfsDataX3d(X3dIfs x3dIfs)
    {
        A = x3dIfs.A;
        B = x3dIfs.B;
        C = x3dIfs.C;
        D = x3dIfs.D;
        E = x3dIfs.E;
        F = x3dIfs.F;
        G = x3dIfs.G;
        H = x3dIfs.H;
        K = x3dIfs.K;
        Ur = x3dIfs.Ur;
        Vr = x3dIfs.Vr;
        Rr = x3dIfs.Rr;
        P = x3dIfs.P;
    }
}