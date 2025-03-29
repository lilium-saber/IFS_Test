using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaApp.OpenGLUse;

namespace AvaloniaApp.Window;

public partial class Test : Avalonia.Controls.Window
{
    
    public Test(List<MathNet.Numerics.LinearAlgebra.Vector<float>> points)
    {
        InitializeComponent();
        var silkOpenGlControl = new SILKOpenGLControl(points);
        this.FindControl<Panel>("OpenGLPanel")!
            .Children.Add(silkOpenGlControl);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}