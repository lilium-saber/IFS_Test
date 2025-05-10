using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using AvaloniaApp.OpenGLUse;
using AvaloniaApp.Ults;
using AvaloniaApp.Ults.Object;
using AvaloniaApp.ViewModel;
using IFS_line.PictureSave;
using SixLabors.ImageSharp.PixelFormats;

namespace AvaloniaApp.Window;

public partial class InputIfs : Avalonia.Controls.Window
{
    private byte[] _imageData = [];
    private Bitmap? _bitmap = null;
    private bool _hasImage = false;
    public ObservableCollection<IfsData> IfsDatas { get; set; } = [new IfsData()];
    public ObservableCollection<IfsDataX3d> IfsDatasX3d { get; set; } = [new IfsDataX3d()];
    
    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    private void UpdateInputIfsSaveButton()
    {
        var button = this.FindControl<Button>("InputSaveImageButton");
        var openGlButton = this.FindControl<Button>("InputImage2OpenGlTextureButton");
        var openGlScatters = this.FindControl<Button>("InputImage2OpenGlScattersButton");
        if (button is null || openGlButton is null || openGlScatters is null) return;
        openGlButton.IsEnabled = _hasImage;
        button.IsEnabled = _hasImage;
        openGlScatters.IsEnabled = _hasImage;
        button.Content = _hasImage ? "Save" : "Unused";
        openGlButton.Content = _hasImage ? "OpenGL Texture" : "Unused";
        openGlScatters.Content = _hasImage ? "OpenGL Scatters" : "Unused";
    }

    private void InputIfsTurn2Tab()
    {
        var tab = this.FindControl<TabControl>("InputIfsTab");
        if (tab is not null)
        {
            tab.SelectedIndex = 0;
        }
    }
    
    // IfsDatasX3d X3d

    private void InputIfsChangeDataGrid()
    {
        IfsDatasX3d.Clear();
        foreach (var data in IfsDatas)
        {
            IfsDatasX3d.Add(new IfsDataX3d(data));
        }
    }

    private void InputIfsChangeDataGrid(List<X3dIfs> list)
    {
        IfsDatasX3d.Clear();
        foreach (var data in list)
        {
            IfsDatasX3d.Add(new IfsDataX3d(data));
        }
    }
    
    private void InputClearX3dRowClick(object sender, RoutedEventArgs e)
    {
        IfsDatasX3d.Clear();
        IfsDatasX3d.Add(new());
        
    }

    private void InputX3dAddRowsClick(object sender, RoutedEventArgs e) => IfsDatasX3d.Add(new());
    
    private void InputX3dDeleteRowClick(object sender, RoutedEventArgs e)
    {
        var index = IfsDatasX3d.Count - 1;
        switch (IfsDatasX3d.Count)
        {
            case > 1:
                IfsDatasX3d.RemoveAt(index);
                return;
            case 1:
                IfsDatasX3d.RemoveAt(index);
                IfsDatasX3d.Add(new());
                return;
        }
    }
    
    private void InputX3dUseIfsClick(object sender, RoutedEventArgs e)
    {
        if(IfsDatasX3d.Sum(_ => _.P) != 100.0m)
        {
            var warning = new Warning("Please add all the probability to 100%");
            warning.ShowDialog(this).ConfigureAwait(true);
            return;
        }

        var list = IfsDatasX3d.Select(OpenGlUlts.IfsDataX3d2X3dIfs).ToList();
    }
    
    // IfsDatas X2d

    private void InputIfsChangeDataGrid(List<(decimal a, decimal b, decimal c, decimal d, decimal e, decimal f, decimal p)> list)
    {
        IfsDatas.Clear();
        foreach (var data in list)
        {
            IfsDatas.Add(new IfsData(data.a, data.b, data.c, data.d, data.e, data.f, data.p));
        }
    }

    private void GetIfsTree0PresetsClick(object sender, RoutedEventArgs e)
    {
        InputIfsChangeDataGrid(TransCodeX2D.TransformationTree0);
        InputIfsTurn2Tab();
    }

    private void GetIfsTree1PresetsClick(object sender, RoutedEventArgs e)
    {
        InputIfsChangeDataGrid(TransCodeX2D.TransformationTree1);
        InputIfsTurn2Tab();
    }

    private void GetIfsFern0PresetsClick(object sender, RoutedEventArgs e)
    {
        InputIfsChangeDataGrid(TransCodeX2D.TransformationFern0);
        InputIfsTurn2Tab();
    }

    private void GetIfsMapleLeaf0PresetsClick(object sender, RoutedEventArgs e)
    {
        InputIfsChangeDataGrid(TransCodeX2D.TransformationLeaf0);
        InputIfsTurn2Tab();
    }

    private void GetIfsMapleLeaf1PresetsClick(object sender, RoutedEventArgs e)
    {
        InputIfsChangeDataGrid(TransCodeX2D.TransformationLeaf1);
        InputIfsTurn2Tab();
    }

    private async void GetJpgImageClick(object sender, RoutedEventArgs e)
    {
        try
        {
            if (IfsDatas.Count == 0)
            {
                var warning = new Warning("Please add at least one row");
                await warning.ShowDialog(this).ConfigureAwait(true);
                return;
            }
            if(IfsDatas.Sum(_ => _.p) != 100.0m)
            {
                var warning = new Warning("Please add all the probability to 100%");
                await warning.ShowDialog(this).ConfigureAwait(true);
                return;
            }

            var list = IfsDatas.Select(_ => (_.a, _.b, _.c, _.d, _.e, _.f, _.p)).ToList();
            var colors = IfsDatas.Select(_ => GetColorType(_.SelectedColor)).ToList();
            // foreach (var ifsData in IfsDatas)
            // {
            //     Console.WriteLine(ifsData.SelectedColor.ToString());
            // }
            // colors.ForEach(_ => Console.WriteLine($"{_}"));
            IImageSave imageSave = new ImageSave();
            _imageData = imageSave.GetWhitePngEncode(list, colors, (int)1.5e5); // RGBA格式
        
            _hasImage = true;
            UpdateInputIfsSaveButton();
        
            using var stream = new MemoryStream(_imageData);
            _bitmap = new Bitmap(stream);
            var image = this.FindControl<Image>("InputImageShow");
            image!.Source = _bitmap;
        }
        catch (Exception errException)
        {
            Console.WriteLine(errException.Message);
        }
    }

    [Obsolete("Obsolete")]
    private async void InputSaveImageClick(object sender, RoutedEventArgs e)
    {
        try
        {
            if (!_hasImage || _bitmap is null)
            {
                var warning = new Warning("Please generate an image first");
                await warning.ShowDialog(this).ConfigureAwait(true);
                return;
            }
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
            Console.WriteLine($"An error occurred on Func-InputSaveImageClick: {err.Message}");
        }
    }

    private void InputImage2OpenGlClick(object sender, RoutedEventArgs e)
    {
        List<byte[]> pictures =
        [
            _imageData
        ];
        var openGl = new SILKOpenGLOnly(pictures);
        openGl.PubStartOpenGl();
    }

    private void InputImage2OpenGlScattersClick(object sender, RoutedEventArgs e)
    {
        var openGl = new OpenGlScatter([..IfsDatas.Select(_ => (_.a, _.b, _.c, _.d, _.e, _.f, _.p))], [..IfsDatas.Select(_ => GetColorType(_.SelectedColor))]);
        var success = openGl.PubStartOpenGl();
        if (!success)
        {
            var warning = new Warning("OpenGL failed to start");
            warning.ShowDialog(this).ConfigureAwait(true);
        }
    }
    
    private void InputAddRowClick(object sender, RoutedEventArgs e) => IfsDatas.Add(new());
    
    private void InputClearRowClick(object sender, RoutedEventArgs e)
    {
        // Console.WriteLine($"InputClearRowClick{IfsDatas?.Count}");
        IfsDatas.Clear();
        IfsDatas.Add(new());
        _bitmap = null;
        _imageData = [];
        var image = this.FindControl<Image>("InputImageShow");
        image!.Source = null;
        _hasImage = false;
        UpdateInputIfsSaveButton();
    }
    
    private void InputDeleteRowClick(object sender, RoutedEventArgs e)
    {
        switch (IfsDatas.Count)
        {
            case > 1:
                IfsDatas.RemoveAt(IfsDatas.Count - 1);
                return;
            case 1:
                IfsDatas.RemoveAt(IfsDatas.Count - 1);
                IfsDatas.Add(new());
                return;
        }
        // Console.WriteLine($"InputDeleteRowClick{IfsDatas?.Count}");
    }
    
    private PointColorType GetColorType(IBrush color)
    {
        return color.ToString() switch
        {
            "Black" => PointColorType.None,
            "Red" => PointColorType.Red,
            "Green" => PointColorType.Green,
            "Blue" => PointColorType.Blue,
            "Yellow" => PointColorType.Yellow,
            "Purple" => PointColorType.Purple,
            "Orange" => PointColorType.Orange,
            "Brown" => PointColorType.Brown,
            "DarkGoldenrod" => PointColorType.DarkGoldenrod,
            _ => PointColorType.None
        };
    }
    
    public InputIfs()
    {
        DataContext = this;
        InitializeComponent();
        UpdateInputIfsSaveButton();
    }
}