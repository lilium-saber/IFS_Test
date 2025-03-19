using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFS_line.PictureSave
{
    public interface IImageSave
    {
        byte[] GetWhiteJpegEncode(List<(decimal a, decimal b, decimal c, decimal d, decimal e, decimal f, decimal p)> transformations,
            int counts = (int)1e5, decimal x = 0, decimal y = 0);

        byte[] GetBlackJpegEncode(List<(decimal a, decimal b, decimal c, decimal d, decimal e, decimal f, decimal p)> transformations,
            int counts = (int)1e5, decimal x = 0, decimal y = 0);

        byte[] GetGifEncode(List<(decimal a, decimal b, decimal c, decimal d, decimal e, decimal f, decimal p)> transformations,
            int counts = (int)1e5, decimal x = 0, decimal y = 0, int pointNums = 200);

        byte[] GetAllWeavingEffects(List<(decimal a, decimal b, decimal c, decimal d, decimal e, decimal f, decimal p)> transformations,
            List<decimal> eList,
            int counts = (int)1e5, decimal x = 0, decimal y = 0);

        byte[] GetPartWeavingEffects(List<(decimal a, decimal b, decimal c, decimal d, decimal e, decimal f, decimal p)> transformations,
            List<(decimal a, decimal b, decimal c, decimal d, decimal e, decimal f, decimal p)> tranCats,
            int fixLine, int counts = (int)1e5, decimal x = 0, decimal y = 0);
    }
}
