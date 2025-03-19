// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;
// using System.Drawing;
// using System.Drawing.Imaging;
//
// namespace IFS_line;
//
// public class JpgPaint
// {
//     private static readonly Lazy<JpgPaint> instance = new(() => new JpgPaint());
//     private static readonly int _weight = 600;
//     private static readonly int _height = 600;
//
//     public static JpgPaint Instance { get => instance.Value; }
//
//     private JpgPaint()
//     {
//     }
//
//     private static System.Drawing.PointF GetPoint(decimal a, decimal b, decimal c, decimal d, decimal e, decimal f, 
//         ref decimal x, ref decimal y)
//     {
//         var p = new PointF();
//         x = a * x + b * y + e;
//         y = c * x + d * y + f;
//         p.X = (float)(x * 500);
//         p.Y = (float)(y * 500);
//         return p;
//     }
//
//     private static (decimal x1, decimal y1) GetPointXY(decimal a, decimal b, decimal c, decimal d, decimal e, decimal f,
//        ref decimal x, ref decimal y)
//     {
//         x = a * x + b * y + e;
//         y = c * x + d * y + f;
//         var x1 = x * 500;
//         var y1 = y * 500;
//         return (x1, y1);
//     }
//
//     private static ImageCodecInfo? GetEncoder(ImageFormat format)
//     {
//         var codecs = ImageCodecInfo.GetImageEncoders();
//         foreach (var codec in codecs)
//         {
//             if (codec.FormatID == format.Guid)
//                 return codec;
//         }
//         return null;
//     }
//
//     public void Paintjpg(decimal x, decimal y, Stream stream)
//     {
//         var points = new List<PointF>();
//         decimal _x = x;
//         decimal _y = y;
//         List<(decimal a, decimal b, decimal c, decimal d, decimal e, decimal f)> transformation =
//         [(0.05m, 0, 0, 0.6m, 0, 0),
//     (0.05m, 0, 0, -0.5m, 0, 1m),
//     (0.46m, 0.32m, -0.386m, 0.383m, 0, 0.6m),
//     (0.47m, -0.154m, 0.171m, 0.423m, 0, 1m),
//     (0.43m, 0.275m, -0.26m, 0.476m, 0, 1m),
//     (0.421m, -0.357m, 0.354m, 0.307m, 0, 0.7m)];
//         var random = new Random();
//
//         Console.WriteLine("Point calculate");
//         for (int i = 0; i < 1e5; i++)
//         {
//             int r = random.Next(100);
//             if (r < 10)
//                 points.Add(GetPoint(transformation[0].a, transformation[0].b, transformation[0].c, transformation[0].d, transformation[0].e, transformation[0].f, ref _x, ref _y));
//             else if (r >= 10 && r < 20)
//                 points.Add(GetPoint(transformation[1].a, transformation[1].b, transformation[1].c, transformation[1].d, transformation[1].e, transformation[1].f, ref _x, ref _y));
//             else if (r >= 20 && r < 40)
//                 points.Add(GetPoint(transformation[2].a, transformation[2].b, transformation[2].c, transformation[2].d, transformation[2].e, transformation[2].f, ref _x, ref _y));
//             else if (r >= 40 && r < 60)
//                 points.Add(GetPoint(transformation[3].a, transformation[3].b, transformation[3].c, transformation[3].d, transformation[3].e, transformation[3].f, ref _x, ref _y));
//             else if (r >= 60 && r < 80)
//                 points.Add(GetPoint(transformation[4].a, transformation[4].b, transformation[4].c, transformation[4].d, transformation[4].e, transformation[4].f, ref _x, ref _y));
//             else
//                 points.Add(GetPoint(transformation[5].a, transformation[5].b, transformation[5].c, transformation[5].d, transformation[5].e, transformation[5].f, ref _x, ref _y));
//         }
//
//         Console.WriteLine("Point calculate is done.\nGraph start.");
//         var minX = points.Min(p => p.X);
//         var minY = points.Min(p => p.Y);
//         var maxX = points.Max(p => p.X);
//         var maxY = points.Max(p => p.Y);
//         float offsetX = 0 - minX;
//         float offsetY = 0 - minY;
//         var scaleX = _weight / (maxX - minX);
//         var scaleY = _height / (maxY - minY);
//         var bitmap = new Bitmap(_weight, _height);
//         using var graphics = Graphics.FromImage(bitmap);
//         graphics.Clear(Color.White);
//         using var pen = new Pen(Color.Black, 1);
//         var bush = new SolidBrush(Color.Black);
//         graphics.ScaleTransform(1, -1);
//         graphics.TranslateTransform(0, -_height);
//         foreach (var p in points)
//         {
//             var x1 = (p.X + offsetX) * scaleX;
//             var y1 = (p.Y + offsetY) * scaleY;
//             graphics.FillEllipse(bush, x1, y1, 1, 1);
//         }
//
//         bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
//         Console.WriteLine("JpgPaint is done");
//     }
//
//
//     public void PaintGif(decimal x, decimal y, Stream stream, int frameCount = 500, int delay = 100)
//     {
//         var points = new List<PointF>();
//         decimal _x = x;
//         decimal _y = y;
//         List<(decimal a, decimal b, decimal c, decimal d, decimal e, decimal f)> transformation =
//         [
//             (0.05m, 0, 0, 0.6m, 0, 0),
//         (0.05m, 0, 0, -0.5m, 0, 1m),
//         (0.46m, 0.32m, -0.386m, 0.383m, 0, 0.6m),
//         (0.47m, -0.154m, 0.171m, 0.423m, 0, 1m),
//         (0.43m, 0.275m, -0.26m, 0.476m, 0, 1m),
//         (0.421m, -0.357m, 0.354m, 0.307m, 0, 0.7m)
//         ];
//         var random = new Random();
//
//         Console.WriteLine("Point calculate");
//         for (int i = 0; i < 1e4; i++)
//         {
//             int r = random.Next(100);
//             if (r < 10)
//                 points.Add(GetPoint(transformation[0].a, transformation[0].b, transformation[0].c, transformation[0].d, transformation[0].e, transformation[0].f, ref _x, ref _y));
//             else if (r >= 10 && r < 20)
//                 points.Add(GetPoint(transformation[1].a, transformation[1].b, transformation[1].c, transformation[1].d, transformation[1].e, transformation[1].f, ref _x, ref _y));
//             else if (r >= 20 && r < 40)
//                 points.Add(GetPoint(transformation[2].a, transformation[2].b, transformation[2].c, transformation[2].d, transformation[2].e, transformation[2].f, ref _x, ref _y));
//             else if (r >= 40 && r < 60)
//                 points.Add(GetPoint(transformation[3].a, transformation[3].b, transformation[3].c, transformation[3].d, transformation[3].e, transformation[3].f, ref _x, ref _y));
//             else if (r >= 60 && r < 80)
//                 points.Add(GetPoint(transformation[4].a, transformation[4].b, transformation[4].c, transformation[4].d, transformation[4].e, transformation[4].f, ref _x, ref _y));
//             else
//                 points.Add(GetPoint(transformation[5].a, transformation[5].b, transformation[5].c, transformation[5].d, transformation[5].e, transformation[5].f, ref _x, ref _y));
//         }
//
//         Console.WriteLine("Point calculate is done\nGraph start");
//         var minX = points.Min(p => p.X);
//         var minY = points.Min(p => p.Y);
//         var maxX = points.Max(p => p.X);
//         var maxY = points.Max(p => p.Y);
//         float offsetX = 0 - minX;
//         float offsetY = 0 - minY;
//         var scaleX = _weight / (maxX - minX);
//         var scaleY = _height / (maxY - minY);
//
//         using var bitmap = new Bitmap(_weight, _height);
//         using (var graphics = Graphics.FromImage(bitmap))
//         {
//             graphics.Clear(Color.White);
//         }
//         var gifEncoder = GetEncoder(ImageFormat.Gif);
//         var encoderParams = new EncoderParameters(1);
//         encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.SaveFlag, (long)EncoderValue.MultiFrame);
//
//         bitmap.Save(stream, gifEncoder, encoderParams);
//
//         var allPoints = new List<PointF>(); // 用于存储所有帧的累积点
//
//         for (int frameIndex = 0; frameIndex < frameCount; frameIndex++)
//         {
//             using var frameBitmap = new Bitmap(_weight, _height);
//             using (var graphics = Graphics.FromImage(frameBitmap))
//             {
//                 graphics.Clear(Color.White);
//                 using var pen = new Pen(Color.Black, 1);
//                 var bush = new SolidBrush(Color.Black);
//                 graphics.ScaleTransform(1, -1);
//                 graphics.TranslateTransform(0, -_height);
//
//                 int pointsPerFrame = points.Count / frameCount;
//                 var framePoints = points.Skip(frameIndex * pointsPerFrame).Take(pointsPerFrame).ToList();
//                 allPoints.AddRange(framePoints); // 将当前帧的点添加到累积点集合
//
//                 // 绘制所有累积的点
//                 foreach (var p in allPoints)
//                 {
//                     var x1 = (p.X + offsetX) * scaleX;
//                     var y1 = (p.Y + offsetY) * scaleY;
//                     graphics.FillEllipse(bush, x1, y1, 1, 1);
//                 }
//             }
//
//             encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.SaveFlag, (long)EncoderValue.FrameDimensionTime);
//             bitmap.SaveAdd(frameBitmap, encoderParams);
//         }
//
//         encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.SaveFlag, (long)EncoderValue.Flush);
//         bitmap.SaveAdd(encoderParams);
//
//         Console.WriteLine("GifPaint is done");
//     }
//
//
//
// }
