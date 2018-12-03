using Microsoft.ML.Runtime.Api;

namespace Tailwind.Traders.ImageClassifier.Api.Mlnet.ImageDataStructures
{
    public class ImageNetPrediction
    {

        [ColumnName(InceptionSettings.OutputTensorName)]
        public float[] PredictedLabels;

    }
}
