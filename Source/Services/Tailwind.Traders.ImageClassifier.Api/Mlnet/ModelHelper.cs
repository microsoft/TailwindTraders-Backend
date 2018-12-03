using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Tailwind.Traders.ImageClassifier.Api.Mlnet
{
    public static class ModelHelpers
    {
        static FileInfo _dataRoot = new FileInfo(typeof(Program).Assembly.Location);

        public static string GetAssetsPath(params string[] paths)
        {
            if (paths == null || paths.Length == 0)
                return null;

            return Path.Combine(paths.Prepend(_dataRoot.Directory.FullName).ToArray());
        }



        public static (string, float) GetBestLabel(string[] labels, float[] probs)
        {
            var max = probs.Max();
            var index = probs.AsSpan().IndexOf(max);


            if (max > 0.7)
                return (labels[index], max);
            else
                return ("None", max);
        }

        public static string[] ReadLabels(string labelsLocation)
        {
            return File.ReadAllLines(labelsLocation);
        }

    }
}
