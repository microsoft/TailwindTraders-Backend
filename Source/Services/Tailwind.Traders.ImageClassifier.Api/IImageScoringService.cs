using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tailwind.Traders.ImageClassifier.Api.Mlnet.ImageDataStructures;

namespace Tailwind.Traders.ImageClassifier.Api
{
    public interface IImageScoringService
    {
        ImagePredictedLabelWithProbability Score(ImageInputData imageName);
        void Init();

        string ImagesFolder { get; }
    }
}
