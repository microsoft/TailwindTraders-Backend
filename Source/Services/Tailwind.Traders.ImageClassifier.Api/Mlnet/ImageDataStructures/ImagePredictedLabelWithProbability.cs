using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tailwind.Traders.ImageClassifier.Api.Mlnet.ImageDataStructures
{
    public class ImagePredictedLabelWithProbability
    {
        public string ImagePath;

        public string PredictedLabel;
        public float Probability { get; set; }
    }
}
