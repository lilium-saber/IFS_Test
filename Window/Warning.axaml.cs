using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace AvaloniaApp.Window;

public partial class Warning : Avalonia.Controls.Window
{
    public string WarningMessage { get; set; } = "Warning";
    
    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
    
    private void WarningCloseClick(object sender, RoutedEventArgs e) => Close();
    
    public Warning() => InitializeComponent();

    public Warning(string warningMessage)
    {
        WarningMessage = warningMessage;
        DataContext = this;
        InitializeComponent();
    }
}