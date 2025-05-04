using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using AvaloniaApp.OpenGLUse;
using AvaloniaApp.Ults;

namespace AvaloniaApp.Window;

public partial class OtherGl : Avalonia.Controls.Window
{
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    
    private void OtherStartGlClick(object? sender, RoutedEventArgs e)
    {
        var otherGl = new OtherOpenGl();
        otherGl.StartOpenGlBackground();
    }
        
    public OtherGl()
    {
        InitializeComponent();
    }

}