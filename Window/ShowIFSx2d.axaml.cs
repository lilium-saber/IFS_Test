using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Image = Avalonia.Controls.Image;

namespace AvaloniaApp.Window;

public partial class ShowIFSx2d : Avalonia.Controls.Window
{
    private Avalonia.Media.Imaging.Bitmap? _bitmap;
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    [Obsolete("Obsolete")]
    private async void SaveImageClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        try //async void
        {
            var saveFileDialog = new SaveFileDialog
            {
                DefaultExtension = "png",
                Filters =
                [
                    new FileDialogFilter { Name = "PNG 图片", Extensions = { "png" } },
                    new FileDialogFilter { Name = "JPEG 图片", Extensions = { "jpg", "jpeg" } }
                ]
            };
            var result = await saveFileDialog.ShowAsync(this);
            if (result == null || _bitmap == null)
            {
                Console.WriteLine("Failed to save image");
                return;
            }
            await using var fileStream = new FileStream(result, FileMode.Create);
            _bitmap.Save(fileStream);
        }
        catch (Exception err)
        {
            Console.WriteLine($"An error occurred on Func-SaveImageClick: {err.Message}");
        }
    }

    private void CloseClick(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    public void SetImage(byte[] imageData)
    {
        using var stream = new MemoryStream(imageData);
        _bitmap = new Avalonia.Media.Imaging.Bitmap(stream);
        this.FindControl<Image>("ImageShow")!.Source = _bitmap;
    }
    
    public ShowIFSx2d()
    {
        _bitmap = null;
        InitializeComponent();
    }
    
    
}