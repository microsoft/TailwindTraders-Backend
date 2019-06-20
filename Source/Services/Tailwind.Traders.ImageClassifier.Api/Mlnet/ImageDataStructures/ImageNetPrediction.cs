using Microsoft.ML.Data;

namespace Tailwind.Traders.ImageClassifier.Api.Mlnet.ImageDataStructures
{
    public class ImageNetPrediction
    {

        [ColumnName(TensorFlowModelSettings.outputTensorName)]
        public float[] PredictedLabels;

    }
}
