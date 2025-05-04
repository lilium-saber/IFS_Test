using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaApp.Ults;

namespace AvaloniaApp.Window;

public partial class OpenGlWindow : Avalonia.Controls.Window
{
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    
    public OpenGlWindow()
    {
        InitializeComponent();
    }

}