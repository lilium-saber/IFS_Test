// using SixLabors.ImageSharp;
// using SixLabors.ImageSharp.Formats.Gif;
// using SixLabors.ImageSharp.PixelFormats;
// using SixLabors.ImageSharp.Processing;
// using System.IO;
//
// public class GifEncoder : IDisposable
// {
//     private readonly List<Image<Rgba32>> frames = new();
//     private readonly Stream outputStream;
//
//     public GifEncoder(Stream outputStream)
//     {
//         this.outputStream = outputStream;
//     }
//
//     public void AddFrame(Image<Rgba32> image, int delay)
//     {
//         var gifMetaData = image.Frames.RootFrame.Metadata.GetGifMetadata();
//         gifMetaData.FrameDelay = delay / 10; // delay in 1/100th seconds
//         frames.Add(image.Clone());
//     }
//
//     public void Save()
//     {
//         if (frames.Count == 0)
//             throw new InvalidOperationException("No frames added to the GIF encoder.");
//
//         using var gif = new Image<Rgba32>(frames[0].Width, frames[0].Height);
//         foreach (var frame in frames)
//         {
//             gif.Frames.AddFrame(frame.Frames.RootFrame);
//         }
//
//         gif.Frames.RemoveFrame(0); // Remove the default empty frame
//         gif.SaveAsGif(outputStream);
//     }
//
//     public void Dispose()
//     {
//         foreach (var frame in frames)
//         {
//             frame.Dispose();
//         }
//     }
// }
