namespace AvaloniaApp.Ults;

public struct X3dParameters
{
    public decimal Level { get; set; } // Tree branch level
    public decimal Height { get; set; } // round table height
    public decimal RRadius { get; set; } // button radius
    public decimal Alpha { get; set; } // branch alpha
    public decimal K { get; set; } // height radius / button radius
    public decimal P { get; set; } // branch height / Height
    public decimal Q { get; set; } // branch button radius / RRadius
    public decimal M { get; set; } // branch lenght / Height
}

public struct X3dIfs(
    decimal a = 0,
    decimal b = 0,
    decimal c = 0,
    decimal d = 0,
    decimal e = 0,
    decimal f = 0,
    decimal g = 0,
    decimal h = 0,
    decimal k = 0,
    decimal ur = 0,
    decimal vr = 0,
    decimal rr = 0,
    decimal p = 0)
{
    public decimal A { get; set; } = a;
    public decimal B { get; set; } = b;
    public decimal C { get; set; } = c;
    public decimal D { get; set; } = d;
    public decimal E { get; set; } = e;
    public decimal F { get; set; } = f;
    public decimal G { get; set; } = g;
    public decimal H { get; set; } = h;
    public decimal K { get; set; } = k;
    public decimal Ur { get; set; } = ur;
    public decimal Vr { get; set; } = vr;
    public decimal Rr { get; set; } = rr;
    public decimal P { get; set; } = p;
}