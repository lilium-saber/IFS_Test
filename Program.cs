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

List<(decimal a, decimal b, decimal c, decimal d, decimal e, decimal f, decimal p)> transformation9 =
    [(0.010m, -0.022m, 0.001m, -0.143m, -0.006m, 0.080m, 3m),
    (0.010m, -0.024m, 0.001m, 0.168m, 0, 0, 3m),
    (0.010m, 0.023m, -0.001m, 0.159m, -0.015m, 0.100m, 3m),
    (0.009m, 0.160m, -0.010m, 0.146m, 0, 0.200m, 3m),
    (0.008m, -0.099m, 0.006m, 0.130m, 0, 0.200m, 3m),
    (0.002m, -0.117m, 0.008m, 0.027m, -0.060m, 0.280m, 3m)];

List<(decimal a, decimal b, decimal c, decimal d, decimal e, decimal f, decimal p)> transformation9Chose3 =
    [(0.569m, 0.267m, -0.315m, 0.481m, 0.100m, 0.290m, 28m), // one
    (0.531m, 0.116m, -0.142m, 0.435m, -0.052m, 0.280m, 24m), // two
    (0.420m, -0.125m, 0.161m, 0.327m, -0.130m, 0.300m, 20m), // three
    (0.607m, 0.197m, -0.233m, 0.513m, 0.100m, 0.290m, 28m), // change one, two and three is the same
    (0.548m, 0.039m, -0.048m, 0.448m, -0.052m, 0.280m, 24m), // change two
    (0.377m, -0.191m, 0.245m, 0.294m, -0.130m, 0.300m, 20m)]; // change three

//List<(decimal a, decimal b, decimal c, decimal d, decimal e, decimal f, decimal p)> tran9Cats = 
//    [.. transformation9, ..transformation9Chose3.Skip(0).Take(3)];

IImageSave imageSave = new ImageSave();
var pictureByte = imageSave.GetWhiteJpegEncode(transformation);
var picture = Image.Load(pictureByte);
picture.Save(savePath);
//var jpegQuality = new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder { Quality = 80 };
//picture.Save(savePath, jpegQuality);


