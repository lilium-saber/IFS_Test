// using SkiaSharp;
// using ImageMagick;
// using System.Security.Principal;
// using OpenTK.Windowing.Common;
// using OpenTK.Windowing.Desktop;
// using OpenTK.Graphics.OpenGL4;
// using OpenTK.Mathematics;
// using SixLabors.ImageSharp;
// using SixLabors.ImageSharp.PixelFormats;
// using SixLabors.ImageSharp.Processing;
//
// namespace IFS_line;
//
// public class SkCanveT
// {
//     private static readonly Lazy<SkCanveT> LazyInstance = new(() => new SkCanveT());
//     private SKCanvas Canve;
//     private SKBitmap Bitmap;
//
//     private readonly int _width;
//     private readonly int _height;
//
//     public static SkCanveT Instance
//     {
//         get => LazyInstance.Value;
//     }
//
//     private SkCanveT()
//     {
//         Bitmap = new(400, 400);
//         Canve = new(Bitmap);
//         Canve.Clear(SKColors.White);
//         _width = Bitmap.Width / 2;
//         _height = Bitmap.Height / 2;
//     }
//
//     private static SKPoint MidPoint(SKPoint p1, SKPoint p2) => new((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
//
//     private static SKPoint MatrixMul2(decimal a, decimal b, decimal c, decimal d, decimal e, decimal f, decimal x,
//         decimal y) => new((float)(a * x + b * y + e), (float)(c * x + d * y + f));
//
//     private void IfsStaticPoint(decimal a, decimal b, decimal c, decimal d, decimal e, decimal f, ref decimal x,
//         ref decimal y, List<SKPoint> frames)
//     {
//         var point = MatrixMul2(a, b, c, d, e, f, x, y);
//         x = (decimal)point.X;
//         y = (decimal)point.Y;
//         point.X = _width / 2 + point.X * _width / 10;
//         point.Y = _height - point.Y * _height / 10;
//         Console.WriteLine($"x: {point.X}, y: {point.Y}");
//
//         if (point.X >= 0 && point.X < Bitmap.Width && point.Y >= 0 && point.Y < Bitmap.Height)
//         {
//             frames.Add(point);
//         }
//     }
//
//
//     private async Task Rec(SKPoint p1, SKPoint p2, SKPoint p3, int depth, List<SKBitmap> frames)
//     {
//         if(depth == 0)
//         {
//             var path = new SKPath();
//             path.MoveTo(p1); // MoveTo: Move to the specified point.
//             path.LineTo(p2); // LineTo: Add a line to the specified point.
//             path.LineTo(p3); // LineTo: Add a line to the specified point.
//             path.Close(); // Close: Close the current contour.
//             var paint = new SKPaint // SKPaint: Defines drawing attributes such as color, stroke width, and style.
//             {
//                 Style = SKPaintStyle.Fill,
//                 Color = SKColors.Black
//             };
//             Canve.DrawPath(path, paint); // DrawPath: Draw the path with the specified paint.
//         }
//         else
//         {
//             var mid1 = MidPoint(p1, p2);
//             var mid2 = MidPoint(p2, p3);
//             var mid3 = MidPoint(p3, p1);
//             await Rec(p1, mid1, mid3, depth - 1, frames);
//             await Rec(mid1, p2, mid2, depth - 1, frames);
//             await Rec(mid3, mid2, p3, depth - 1, frames);
//         }
//
//         var frame = new SKBitmap(Bitmap.Width, Bitmap.Height);
//         var canvas = new SKCanvas(frame);
//         canvas.DrawBitmap(Bitmap, 0, 0);
//         frames.Add(frame);
//         await Task.Delay(10);
//     }
//
//     public async Task Rec(SKPoint p1, SKPoint p2, SKPoint p3, int depth, Stream stream)
//     {
//         var frames = new List<SKBitmap>();
//         await Rec(p1, p2, p3, depth, frames);
//         var collection = new MagickImageCollection();
//         foreach (var frame in frames)
//         {
//             var image = SKImage.FromBitmap(frame);
//             var data = image.Encode(SKEncodedImageFormat.Png, 100);
//             var ms = new MemoryStream(data.ToArray());
//             collection.Add(new MagickImage(ms));
//             collection[^1].AnimationDelay = 5;
//         }
//         collection.Write(stream, MagickFormat.Gif);
//         Console.WriteLine("Rec Done");
//     }
//
//
//
//     public void IfsRecFunction(decimal a, decimal b, decimal c, decimal d, decimal e, decimal f, ref decimal x,
//         ref decimal y, List<SKBitmap> frames)
//     {
//         var point = MatrixMul2(a, b, c, d, e, f, x, y);
//         x = (decimal)point.X;
//         y = (decimal)point.Y;
//         point.X *= _width;
//         point.Y *= _height;
//         var paint = new SKPaint
//         {
//             Style = SKPaintStyle.Fill,
//             Color = SKColors.Black
//         };
//         Canve.DrawPoint(point, paint);
//         // gif
//         var frame = new SKBitmap(Bitmap.Width, Bitmap.Height);
//         using (var canvas = new SKCanvas(frame))
//         {
//             canvas.Scale(1, -1, Bitmap.Width / 2, Bitmap.Height / 2);
//             canvas.DrawBitmap(Bitmap, 0, 0);
//         }
//         frames.Add(frame);
//     }
//     
//     public void IfsRecFunction(decimal x, decimal y, Stream stream)
//     {
//         var frames = new List<SKBitmap>();
//         var p = new Random();
//         List<(decimal a, decimal b, decimal c, decimal d, decimal e, decimal f)> transformations = 
//             [(0.5m, 0, 0, 0.5m, 0, 0), 
//             (0.5m, 0, 0, 0.5m, 0.5m, 0), 
//             (0.5m, 0, 0, 0.5m, 0.25m, 0.5m)];
//         for (int i = 0; i < 2.5e4; i++)
//         {
//             Console.WriteLine($"now is the {i} time");
//             var getP = p.Next(120);
//             if(getP < 80)
//             {
//                 IfsRecFunction(transformations[0].a, transformations[0].b, transformations[0].c, transformations[0].d, transformations[0].e, transformations[0].f, ref x, ref y, frames);
//             }
//             else if (getP >= 100)
//             {
//                 IfsRecFunction(transformations[1].a, transformations[1].b, transformations[1].c, transformations[1].d, transformations[1].e, transformations[1].f, ref x, ref y, frames);
//             }
//             else
//             {
//                 IfsRecFunction(transformations[2].a, transformations[2].b, transformations[2].c, transformations[2].d, transformations[2].e, transformations[2].f, ref x, ref y, frames);
//             }
//         }
//         var collection = new MagickImageCollection();
//         // png only
//         //var frame = new SKBitmap(Bitmap.Width, Bitmap.Height);
//         //using (var canvas = new SKCanvas(frame))
//         //{
//         //    canvas.DrawBitmap(Bitmap, 0, 0);
//         //}
//         //frames.Add(frame);
//
//         //Parallel.ForEach(frames, frame =>
//         //{
//         //    using var image = SKImage.FromBitmap(frame);
//         //    using var data = image.Encode(SKEncodedImageFormat.Png, 100);
//         //    using var ms = new MemoryStream(data.ToArray());
//         //    var magickImage = new MagickImage(ms);
//         //    magickImage.Quantize(new QuantizeSettings { Colors = 2 });
//         //    lock (collection)
//         //    {
//         //        collection.Add(magickImage);
//         //        collection[^1].AnimationDelay = 1;
//         //    }
//         //});
//
//         foreach (var f in frames)
//         {
//             using var image = SKImage.FromBitmap(f);
//             using var data = image.Encode(SKEncodedImageFormat.Png, 100);
//             using var ms = new MemoryStream(data.ToArray());
//             var magickImage = new MagickImage(ms);
//             magickImage.Quantize(new QuantizeSettings { Colors = 2 });
//             collection.Add(magickImage);
//             collection[^1].AnimationDelay = 1;
//         }
//         Console.WriteLine("start gif");
//         Task.Run(() =>
//         {
//             collection.Optimize();
//             collection.Write(stream, MagickFormat.Gif);
//         }).Wait();
//         Console.WriteLine("IFS_Rec Done");
//     }
//
//     public void IfsBarnsley(decimal a, decimal b, decimal c, decimal d, decimal e, decimal f, ref decimal x,
//         ref decimal y, List<SKPoint> frames)
//     {
//         var point = MatrixMul2(a, b, c, d, e, f, x, y);
//         x = (decimal)point.X;
//         y = (decimal)point.Y;
//         point.X = Bitmap.Width / 2 + point.X * Bitmap.Width / 10;
//         point.Y = Bitmap.Height - point.Y * Bitmap.Height / 10;
//
//         if(point.X >= 0 && point.X < Bitmap.Width && point.Y >= 0 && point.Y < Bitmap.Height)
//         {
//             frames.Add(point);
//         }
//
//         //var paint = new SKPaint
//         //{
//         //    Style = SKPaintStyle.Fill,
//         //    Color = SKColors.Black
//         //};
//         //Canve.DrawPoint(point, paint);
//         ////
//         //var frame = new SKBitmap(Bitmap.Width, Bitmap.Height);
//         //using (var canvas = new SKCanvas(frame))
//         //{
//         //    canvas.Scale(1, -1, Bitmap.Width / 2, Bitmap.Height / 2);
//         //    canvas.DrawBitmap(Bitmap, 0, 0);
//         //}
//         //frames.Add(frame);
//     }
//
//     public void IfsBarnsley(decimal x, decimal y, Stream stream)
//     {
//         var frames = new List<SKPoint>();
//         List<(decimal a, decimal b, decimal c, decimal d, decimal e, decimal f)> transformation =
//             //[(0, 0, 0, 0.16m, 0, 0),
//             //(0.85m, 0.04m, -0.04m, 0.85m, 0, 1.6m),
//             //(0.2m, -0.26m, 0.23m, 0.22m, 0, 1.6m),
//             //(-0.15m, 0.28m, 0.26m, 0.24m, 0, 0.44m)];
//             [(0, 0, 0, 0.25m, 0, -0.14m),
//             (0.85m, 0.02m, -0.02m, 0.83m, 0, 1m),
//             (0.09m, -0.28m, 0.3m, 0.11m, 0, 0.6m),
//             (-0.09m, 0.25m, 0.3m, 0.09m, 0, 0.7m)];
//         var p = new Random();
//         for (int i = 0; i < 5e3; i++)
//         {
//             Console.WriteLine($"now is the {i} time");
//             var getP = p.Next(100); 
//             if (getP < 2)
//             {
//                 IfsStaticPoint(transformation[0].a, transformation[0].b, transformation[0].c, transformation[0].d, transformation[0].e, transformation[0].f, ref x, ref y, frames);
//             }
//             else if (getP >= 2 && getP < 86)
//             {
//                 IfsStaticPoint(transformation[1].a, transformation[1].b, transformation[1].c, transformation[1].d, transformation[1].e, transformation[1].f, ref x, ref y, frames);
//             }
//             else if (getP >= 86 && getP < 93)
//             {
//                 IfsStaticPoint(transformation[2].a, transformation[2].b, transformation[2].c, transformation[2].d, transformation[2].e, transformation[2].f, ref x, ref y, frames);
//             }
//             else
//             {
//                 IfsStaticPoint(transformation[3].a, transformation[3].b, transformation[3].c, transformation[3].d, transformation[3].e, transformation[3].f, ref x, ref y, frames);
//             }
//         }
//         Console.WriteLine("Barnsley start");
//
//         //png
//         Canve.Clear(SKColors.White);
//         using SKPaint paint = new()
//         {
//             Style = SKPaintStyle.Fill,
//             Color = SKColors.Black,
//             IsAntialias = true
//         };
//         foreach( var po in frames)
//         {
//             Canve.DrawPoint(po, paint);
//         }
//         using var image = SKImage.FromBitmap(Bitmap);
//         using var data = image.Encode(SKEncodedImageFormat.Png, 100);
//         data.SaveTo(stream);
//
//         // gif
//         // var collection = new MagickImageCollection();
//
//         //var frame = new SKBitmap(Bitmap.Width, Bitmap.Height);
//         //using (var canvas = new SKCanvas(frame))
//         //{
//         //    canvas.DrawBitmap(Bitmap, 0, 0);
//         //}
//         //frames.Add(frame);
//
//         // foreach (var f in frames)
//         // {
//         //     using var image = SKImage.FromBitmap(f);
//         //     using var data = image.Encode(SKEncodedImageFormat.Png, 100);
//         //     using var ms = new MemoryStream(data.ToArray());
//         //     var magickImage = new MagickImage(ms);
//         //     magickImage.Quantize(new QuantizeSettings { Colors = 2 });
//         //     collection.Add(magickImage);
//         //     collection[^1].AnimationDelay = 1;
//         // }
//
//         // Console.WriteLine("End gif");
//         // collection.Optimize();
//         // collection.Write(stream, MagickFormat.Gif);
//         Console.WriteLine("Barnsley Done");
//     }
//
//     public void IfsTree(decimal x, decimal y, Stream stream)
//     {
//         var Ps = new List<SKPoint>();
//         List<(decimal a, decimal b, decimal c, decimal d, decimal e, decimal f)> transformation =
//         //[(0.01m, 0, 0, 0.45m, 0, 0),
//         //(-0.01m, 0, 0, -0.45m, 0, 0.4m),
//         //(0.42m, -0.42m, 0.42m, 0.42m, 0, 0.4m),
//         //(0.42m, 0.42m, -0.42m, 0.42m, 0, 0.4m)];
//         [(0.6m, 0, 0, 0.6m, 0.18m ,0.36m),
//         (0.6m, 0, 0, 0.6m, 0.18m, 0.12m),
//         (0.4m, 0.3m, -0.3m, 0.4m, 0.27m, 0.36m),
//         (0.4m, -0.3m, 0.3m, 0.4m, 0.27m, 0.09m)];
//         var p = new Random(42);
//         for (int i = 0; i < 5e3; i++)
//         {
//             Console.WriteLine($"now is the {i} time");
//             var getP = p.Next(100);
//             if (getP < 25)
//             {
//                 IfsStaticPoint(transformation[0].a, transformation[0].b, transformation[0].c, transformation[0].d, transformation[0].e, transformation[0].f, ref x, ref y, Ps);
//             }
//             else if (getP >= 25 && getP < 50)
//             {
//                 IfsStaticPoint(transformation[1].a, transformation[1].b, transformation[1].c, transformation[1].d, transformation[1].e, transformation[1].f, ref x, ref y, Ps);
//             }
//             else if (getP >= 50 && getP < 75)
//             {
//                 IfsStaticPoint(transformation[2].a, transformation[2].b, transformation[2].c, transformation[2].d, transformation[2].e, transformation[2].f, ref x, ref y, Ps);
//             }
//             else
//             {
//                 IfsStaticPoint(transformation[3].a, transformation[3].b, transformation[3].c, transformation[3].d, transformation[3].e, transformation[3].f, ref x, ref y, Ps);
//             }
//         }
//         Console.WriteLine("Tree start");
//         Canve.Clear(SKColors.White);
//         using SKPaint paint = new()
//         {
//             Style = SKPaintStyle.Fill,
//             Color = SKColors.Black,
//             IsAntialias = true,
//             StrokeWidth = 1
//         };
//         foreach (var po in Ps)
//         {
//             var transformedPoint = new SKPoint(
//             Bitmap.Width / 2 + po.X * Bitmap.Width / 10,
//             Bitmap.Height - po.Y * Bitmap.Height / 10
//             );
//             Canve.DrawPoint(transformedPoint, paint);
//         }
//         using var image = SKImage.FromBitmap(Bitmap);
//         using var data = image.Encode(SKEncodedImageFormat.Png, 100);
//         data.SaveTo(stream);
//         Console.WriteLine("Tree Done");
//     }
//
//
// }