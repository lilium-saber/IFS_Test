using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaApp.Mold;
using AvaloniaApp.OpenGLUse;
using AvaloniaApp.Ults;
using AvaloniaApp.Window;
using IFS_line.PictureSave;
using MathNet.Numerics.LinearAlgebra;
using Vector = MathNet.Numerics.LinearAlgebra.Vector<float>;

namespace AvaloniaApp;

[SuppressMessage("Performance", "CA1859:尽可能使用具体类型以提高性能")]
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
        // Thread.Sleep(2000);
        var silk = new SILKOpenGLOnly(points);
        silk.PubStartOpenGl();
        // var showMold = new Test(points);
        // await showMold.ShowDialog(this).ConfigureAwait(true);
    }
    
    private void IfsImageClick(object sender, RoutedEventArgs e)
    {
        var showIfs = new ShowIFSx2d();
        IImageSave imageSave = new ImageSave();
        var transformation = TransformationCode.TransformationTree0;
        var pictureByte = imageSave.GetWhiteJpegEncode(transformation);
        showIfs.SetImage(pictureByte);

        showIfs.ShowDialog(this).ConfigureAwait(true);
    }

    private void OpenInputIfsClick(object sender, RoutedEventArgs e)
    {
        var inputIfs = new InputIfs();
        inputIfs.ShowDialog(this).ConfigureAwait(true);
    }
}