using System.Collections.ObjectModel;
using System.ComponentModel;
using Avalonia.Media;

namespace AvaloniaApp.ViewModel;

public enum PointColorType
{
    None,
    Red,
    Green,
    Blue,
    Yellow,
    Purple,
    Orange,
    Brown,
    DarkGoldenrod
}

public sealed class IfsData : INotifyPropertyChanged
{
    public decimal a { get; set; } = 0;
    public decimal b { get; set; } = 0;
    public decimal c { get; set; } = 0;
    public decimal d { get; set; } = 0;
    public decimal e { get; set; } = 0;
    public decimal f { get; set; } = 0;
    public decimal p { get; set; } = 0;
    public IBrush SelectedColor { get; set; } = Brushes.Black;

    public ObservableCollection<IBrush> Colors { get; set; } =
    [
        Brushes.Black,
        Brushes.Red,
        Brushes.Green,
        Brushes.Blue,
        Brushes.Yellow,
        Brushes.Purple,
        Brushes.Orange,
        Brushes.Brown,
        Brushes.DarkGoldenrod
    ];
    
    public IfsData(decimal a = 0, decimal b = 0, decimal c = 0, decimal d = 0, decimal e = 0, decimal f = 0, decimal p = 0)
    {
        this.a = a;
        this.b = b;
        this.c = c;
        this.d = d;
        this.e = e;
        this.f = f;
        this.p = p;
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}