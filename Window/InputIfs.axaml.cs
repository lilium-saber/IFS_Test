﻿using System;
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
using AvaloniaApp.ViewModel;
using IFS_line.PictureSave;
using SixLabors.ImageSharp.PixelFormats;

namespace AvaloniaApp.Window;

public partial class InputIfs : Avalonia.Controls.Window
{
    private byte[] _imageData;
    private Bitmap? _bitmap = null;
    private bool _hasImage = false;
    public ObservableCollection<IfsData> IfsDatas { get; set; } = [new IfsData()];
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void UpdateInputIfsSaveButton()
    {
        var button = this.FindControl<Button>("InputSaveImageButton");
        var openGlButton = this.FindControl<Button>("InputImage2OpenGlButton");
        if(button is not null && openGlButton is not null)
        {
            openGlButton.IsEnabled = _hasImage;
            button.IsEnabled = _hasImage;
            button.Content = _hasImage ? "Save" : "Unused";
            openGlButton.Content = _hasImage ? "Go To OpenGL" : "Unused";
        }
    }

    private void InputIfsTurn2Tab()
    {
        var tab = this.FindControl<TabControl>("InputIfsTab");
        if (tab is not null)
        {
            tab.SelectedIndex = 0;
        }
    }

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
        InputIfsChangeDataGrid(TransformationCode.TransformationTree0);
        InputIfsTurn2Tab();
    }

    private void GetIfsTree1PresetsClick(object sender, RoutedEventArgs e)
    {
        InputIfsChangeDataGrid(TransformationCode.TransformationTree1);
        InputIfsTurn2Tab();
    }

    private void GetIfsFern0PresetsClick(object sender, RoutedEventArgs e)
    {
        InputIfsChangeDataGrid(TransformationCode.TransformationFern0);
        InputIfsTurn2Tab();
    }

    private void GetIfsLeaf0PresetsClick(object sender, RoutedEventArgs e)
    {
        InputIfsChangeDataGrid(TransformationCode.TransformationLeaf0);
        InputIfsTurn2Tab();
    }

    private async void GetJpgImageClick(object sender, RoutedEventArgs e)
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

        List<(decimal a, decimal b, decimal c, decimal d, decimal e, decimal f, decimal p)> list =
            [..IfsDatas.Select(_ => (_.a, _.b, _.c, _.d, _.e, _.f, _.p))];
        var colors = IfsDatas.Select(_ => GetColorType(_.SelectedColor)).ToList();
        // foreach (var ifsData in IfsDatas)
        // {
        //     Console.WriteLine(ifsData.SelectedColor.ToString());
        // }
        // colors.ForEach(_ => Console.WriteLine($"{_}"));
        IImageSave imageSave = new ImageSave();
        _imageData = imageSave.GetWhitePngEncode(list, colors); // RGBA格式
        
        _hasImage = true;
        UpdateInputIfsSaveButton();
        
        // foreach (var data in IfsDatas)
        // {
        //     Console.WriteLine($"a={data.a}, b={data.b}, c={data.c}, d={data.d}, e={data.e}, f={data.f}, p={data.p}");
        // }
        using var stream = new MemoryStream(_imageData);
        _bitmap = new Bitmap(stream);
        var image = this.FindControl<Image>("InputImageShow");
        if (image is not null)
        {
            image.Source = _bitmap;
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
    
    private void InputAddRowClick(object sender, RoutedEventArgs e)
    {
        Console.WriteLine($"InputAddRowClick{IfsDatas?.Count}");
        IfsDatas!.Add(new());
    }
    
    private void InputDeleteRowClick(object sender, RoutedEventArgs e)
    {
        if(IfsDatas.Count > 1)
            IfsDatas.RemoveAt(IfsDatas.Count - 1);
        if (IfsDatas.Count == 1)
        {
            IfsDatas.RemoveAt(IfsDatas.Count - 1);
            IfsDatas.Add(new());
        }
        Console.WriteLine($"InputDeleteRowClick{IfsDatas?.Count}");
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
        _imageData = [];
        DataContext = this;
        InitializeComponent();
        UpdateInputIfsSaveButton();

    }
}