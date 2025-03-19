using IFS_line.Point;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp.Processing;

namespace IFS_line.PictureSave
{
    public class ImageSave : IImageSave
    {
        private static byte[] JpegMixToGif(List<byte[]> jpegs)
        {
            List<Image<Rgba32>> images = [..jpegs.Select(_ => Image.Load<Rgba32>(_))];
            Image<Rgba32> gif = new(images[0].Width, images[0].Height);
            images.ForEach(_ =>
            {
                gif.Frames.AddFrame(_.Frames.RootFrame);
            });
            MemoryStream ms = new();
            gif.SaveAsGif(ms);
            return ms.ToArray();
        }

        private List<byte[]> AllWeavingEffects(List<(decimal a, decimal b, decimal c, decimal d, decimal e, decimal f, decimal p)> transformations,
            List<decimal> eList,
            int counts = (int)1e5, decimal x = 0, decimal y = 0)
        {
            List<byte[]> effects = [];
            List<(decimal a, decimal b, decimal c, decimal d, decimal e, decimal f, decimal p)> changeTran = 
                [..transformations.Zip(eList, (x, y) => (x.a, x.b, x.c, x.d, y, x.f, x.p))];

            effects.Add(GetBlackJpegEncode(changeTran, counts, x, y));
            effects.Add(GetWhiteJpegEncode(transformations, counts, x, y));

            changeTran = [.. changeTran.Select(_ => (_.a, _.b, _.c, _.d, (0 - _.e), _.f, _.p))];
            effects.Add(GetBlackJpegEncode(changeTran, counts, x, y));

            return effects;
        }

        private List<byte[]> PartWeavingEffects(List<(decimal a, decimal b, decimal c, decimal d, decimal e, decimal f, decimal p)> transformations,
            List<(decimal a, decimal b, decimal c, decimal d, decimal e, decimal f, decimal p)> tranCats,
            int fixLine, int counts = (int)1e5, decimal x = 0, decimal y = 0)
        {
            List<byte[]> effects = [];
            
            for (int i = 0; i < 3 + 1; i++)
            {
                List<(decimal a, decimal b, decimal c, decimal d, decimal e, decimal f, decimal p)> tranTemp = 
                                             [..transformations, ..tranCats.Skip(i).Take(fixLine)];
            }
            

            return effects;
        }

        // background is white
        public byte[] GetWhiteJpegEncode(List<(decimal a, decimal b, decimal c, decimal d, decimal e, decimal f, decimal p)> transformations,
            int counts = (int)1e5, decimal x = 0, decimal y = 0)
        {
            var image = new Image<Rgba32>(Ults.Ults.BitmapLen, Ults.Ults.BitmapLen, new Rgba32(255, 255, 255, 256));
            var points = PointCalculator.GetPointList(transformations, counts, x, y);
            var ms = new MemoryStream();

            if(counts != 0)
            {
                var (minX, maxX, minY, maxY) = (points.Min(p => p.x), points.Max(p => p.x), points.Min(p => p.y), points.Max(p => p.y));
                var (scaleX, scaleY) = (Ults.Ults.BitmapLen / (maxX - minX), Ults.Ults.BitmapLen / (maxY - minY));
                
                points.ForEach(_ =>
                {
                    var(x0, y0) = ((int)((_.x - minX) * scaleX), (int)((_.y - minY) * scaleY));
                    if (x0 >= 0 && x0 < Ults.Ults.BitmapLen && y0 >= 0 && y0 < Ults.Ults.BitmapLen)
                    {
                        image[x0, y0] = new Rgba32(0, 0, 0, 256);
                    }
                });
            }

            image.Mutate(_ => _.Flip(FlipMode.Vertical));
            image.SaveAsJpeg(ms);
            return ms.ToArray();
        }

        // background is black
        public byte[] GetBlackJpegEncode(List<(decimal a, decimal b, decimal c, decimal d, decimal e, decimal f, decimal p)> transformations,
            int counts = (int)1e5, decimal x = 0, decimal y = 0)
        {
            var image = new Image<Rgba32>(Ults.Ults.BitmapLen, Ults.Ults.BitmapLen, new Rgba32(0, 0, 0, 0));
            var points = PointCalculator.GetPointList(transformations, counts, x, y);
            var ms = new MemoryStream();

            if (counts != 0)
            {
                var (minX, maxX, minY, maxY) = (points.Min(p => p.x), points.Max(p => p.x), points.Min(p => p.y), points.Max(p => p.y));
                var (scaleX, scaleY) = (Ults.Ults.BitmapLen / (maxX - minX), Ults.Ults.BitmapLen / (maxY - minY));
                
                points.ForEach(_ =>
                {
                    var(x0, y0) = ((int)((_.x - minX) * scaleX), (int)((_.y - minY) * scaleY));
                    if (x0 >= 0 && x0 < Ults.Ults.BitmapLen && y0 >= 0 && y0 < Ults.Ults.BitmapLen)
                    {
                        image[x0, y0] = new Rgba32(255, 255, 255, 256);
                    }
                });
            }

            image.Mutate(_ => _.Flip(FlipMode.Vertical));
            image.SaveAsJpeg(ms, new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder { Quality = 75 });
            return ms.ToArray();
        }

        public byte[] GetGifEncode(List<(decimal a, decimal b, decimal c, decimal d, decimal e, decimal f, decimal p)> transformations,
            int counts = (int)1e4, decimal x = 0, decimal y = 0, int pointNums = 200)
        {
            List<byte[]> jpegs = [];
            
            for (int i = 0; i < counts; i += pointNums)
                jpegs.Add(GetBlackJpegEncode(transformations, i, x, y));
            
            return JpegMixToGif(jpegs);
        }

        public byte[] GetAllWeavingEffects(List<(decimal a, decimal b, decimal c, decimal d, decimal e, decimal f, decimal p)> transformations,
            List<decimal> eList,
        int counts = (int)1e5, decimal x = 0, decimal y = 0)
        {
            var allEffects = AllWeavingEffects(transformations, eList, counts, x, y);
            return JpegMixToGif(allEffects);
        }

        public byte[] GetPartWeavingEffects(List<(decimal a, decimal b, decimal c, decimal d, decimal e, decimal f, decimal p)> transformations,
            List<(decimal a, decimal b, decimal c, decimal d, decimal e, decimal f, decimal p)> tranCats, 
            int fixLine, int counts = (int)1e5, decimal x = 0, decimal y = 0)
        {
            var partEffects = PartWeavingEffects(transformations, tranCats, fixLine, counts, x, y);
            return JpegMixToGif(partEffects);
        }

    }
}
