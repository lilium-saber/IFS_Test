using System.IO;
using IFS_line;
using IFS_line.PictureSave;
using IFS_line.Point;
using SixLabors.ImageSharp;

var targetF = AppDomain.CurrentDomain.BaseDirectory;
var savePath = Path.Combine(targetF, "IFS_paper0.jpg");
//var stream = File.Create(savePath);
//var pngExporter = new PngExporter {Width = 600, Height = 400};
//pngExporter.Export(model, stream);
//Console.WriteLine($"{savePath} is done");
//jetbrains://rd/navigate/reference?project=IFS_line&path=PngRes

// 尽量生成PNG图片。GIf图片在点数较多时，带不动。

//var setCanve = SkCanveT.Instance;
//await setCanve.Rec(p1, p2, p3, 5, stream);
//setCanve.IfsTree(0, 0, stream);
//var jpgPaint = JpgPaint.Instance;
////jpgPaint.PaintJpg(0, 0, stream);
//jpgPaint.PaintGif(0, 0, stream);

List<(decimal a, decimal b, decimal c, decimal d, decimal e, decimal f, decimal p)> transformation =
        [(0.04m, 0, 0, -0.44m, 0, 0.39m, 8m),
        (0.04m, 0, 0, -0.44m, 0, 0.59m, 8m),
        (0.389m, 0.289m, -0.389m, 0.345m, 0, 0.22m, 21m),
        (0.345m, -0.257m, 0.289m, 0.306m, 0, 0.240m, 21m),
        (0.390m, -0.275m, 0.225m, 0.476m, 0, 0.440m, 21m),
        (0.408m, 0.19m, -0.19m, 0.408m, 0, 0.480m, 21m)];

List<(decimal e1, decimal e2)> eList2 = 
        [(0.04m, -0.04m),
        (0.1m, -0.09m),
        (0.02m, -0.02m),
        (0.02m, -0.02m),
        (0.09m, -0.06m),
        (0.09m, -0.06m)];

//List<(decimal a, decimal b, decimal c, decimal d, decimal e, decimal f, decimal p)> tran9Cats = 
//    [.. transformation9, ..transformation9Chose3.Skip(0).Take(3)];

IImageSave imageSave = new ImageSave();
var pictureByte = imageSave.GetWhiteJpegEncode(transformation);
var picture = Image.Load(pictureByte);
picture.Save(savePath);
//var jpegQuality = new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder { Quality = 80 };
//picture.Save(savePath, jpegQuality);


