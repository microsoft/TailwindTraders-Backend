using System;
using Tailwind.Traders.ImageClassifier.Api.Mlnet.ImageDataStructures;

namespace Tailwind.Traders.ImageClassifier.Api.Dtos
{
    public class ClassificationResponse
    {

        public static ProductItem CreateFrom(ImagePredictedLabelWithProbability scoring)
        {
            var tag = scoring.PredictedLabel;
            return new ProductItem(tag, (decimal)scoring.Probability);
        }
    }
}
