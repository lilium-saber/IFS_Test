using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaApp.Mold;
using AvaloniaApp.OpenGLUse;
using AvaloniaApp.Window;
using MathNet.Numerics.LinearAlgebra;
using Vector = MathNet.Numerics.LinearAlgebra.Vector<float>;

namespace AvaloniaApp;

public partial class MainWindow : Avalonia.Controls.Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void OpenShowWindowClick(object sender, RoutedEventArgs e)
    {
        List<(float xAngle, float yAngle, float zAngle, float xScale, float yScale, float zScale, float TX,
            float TY, float TZ, float p)> tran =
        [
            (0, 0, 0, 1, 1, 1, 0, 0, 0, 20),
            (-5, 0, 0, 1, 1, 0.8f, 0, 0, 500, 20),
            (5, 0, 0, 1, 1, 0.8f, 0, 0, 500, 20),
            (0, -5, 0, 1, 1, 0.8f, 0, 0, 500, 20),
            (0, 5, 0, 1, 1, 0.8f, 0, 0, 500, 20)
        ];
        List<(float a, float b, float c, float d, float e, float f, float g, float h, float k, float u, float v, float r
            , float p)> temp =
            [];
        var start = Vector.Build.DenseOfArray([0.0f, 0.0f, 0.0f]);
        var points = MoldCalculator.GetMatrixListRes(tran, start, 50);
        Thread.Sleep(2000);
        var silk = new SILKOpenGLOnly(points);
        silk.PubStartOpenGl();
        // var showMold = new Test(points);
        // await showMold.ShowDialog(this).ConfigureAwait(true);
    }
}