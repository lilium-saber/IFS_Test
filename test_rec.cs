// namespace IFS_line;
//
// using SkiaSharp;
//
// public class TestRec
// {
//     /*
//     static void Main()
//     {
//         int width = 800;
//         int height = 800;
//         using (var bitmap = new SKBitmap(width, height))
//         using (var canvas = new SKCanvas(bitmap))
//         {
//             canvas.Clear(SKColors.White);
//
//             // 初始三角形的顶点
//             var p1 = new SKPoint(width / 2, 0);
//             var p2 = new SKPoint(0, height);
//             var p3 = new SKPoint(width, height);
//
//             DrawSierpinski(canvas, p1, p2, p3, 5);
//
//             using (var image = SKImage.FromBitmap(bitmap))
//             using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
//             {
//                 using (var stream = File.OpenWrite("sierpinski_triangle.png"))
//                 {
//                     data.SaveTo(stream);
//                 }
//             }
//         }
//     }
//     */
//     public static void DrawSierpinski(SKCanvas canvas, SKPoint p1, SKPoint p2, SKPoint p3, int depth)
//     {
//         if (depth == 0)
//         {
//             var path = new SKPath();
//             path.MoveTo(p1);
//             path.LineTo(p2);
//             path.LineTo(p3);
//             path.Close();
//
//             var paint = new SKPaint
//             {
//                 Style = SKPaintStyle.Fill,
//                 Color = SKColors.Black
//             };
//
//             canvas.DrawPath(path, paint);
//         }
//         else
//         {
//             var mid1 = MidPoint(p1, p2);
//             var mid2 = MidPoint(p2, p3);
//             var mid3 = MidPoint(p3, p1);
//
//             DrawSierpinski(canvas, p1, mid1, mid3, depth - 1);
//             DrawSierpinski(canvas, mid1, p2, mid2, depth - 1);
//             DrawSierpinski(canvas, mid3, mid2, p3, depth - 1);
//         }
//     }
//
//     public static SKPoint MidPoint(SKPoint p1, SKPoint p2)
//     {
//         return new SKPoint((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
//     }
// }
