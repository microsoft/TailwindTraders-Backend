using Microsoft.ML.Transforms.Image;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Tailwind.Traders.ImageClassifier.Api.Mlnet.ImageDataStructures
{
    public class ImageInputData
    {
        public string ImagePath;

        [ImageType(227, 227)]
        public Bitmap Image { get; set; }

        public static IEnumerable<ImageInputData> ReadFromCsv(string file, string folder)
        {
            return File.ReadAllLines(file)
             .Select(x => x.Split('\t'))
             .Select(x => new ImageInputData { ImagePath = Path.Combine(folder, x[0]) });
        }
    }
}
