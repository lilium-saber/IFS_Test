using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaApp.Ults;

namespace AvaloniaApp.Window;

public partial class OpenGlWindow : Avalonia.Controls.Window
{
    private OpenGlThread _openGlThread = new();
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    
    public OpenGlWindow()
    {
        InitializeComponent();
    }

}