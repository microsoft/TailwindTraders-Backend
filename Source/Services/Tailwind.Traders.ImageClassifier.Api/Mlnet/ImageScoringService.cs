using Microsoft.ML;
using Microsoft.ML.Runtime.Api;
using Microsoft.ML.Runtime.Data;
using Microsoft.ML.Runtime.ImageAnalytics;
using Microsoft.ML.Transforms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tailwind.Traders.ImageClassifier.Api.Mlnet.ImageDataStructures;

namespace Tailwind.Traders.ImageClassifier.Api.Mlnet
{
    public class ImageScoringService : IImageScoringService
    {
        private readonly MLContext _mlContext;
        private readonly string _imagesFolder;
        private readonly string _modelLocation;
        private readonly string[] _labels;
        private readonly string _contentRoot;
        private PredictionFunction<ImageInputData, ImageNetPrediction> _model;


        public ImageScoringService(string contentRoot, string rootImagesFolder)
        {
            _contentRoot = contentRoot;
            _mlContext = new MLContext();
            _modelLocation = Path.Combine(_contentRoot, "model", "model.pb");
            _labels = ModelHelpers.ReadLabels(Path.Combine(_contentRoot, "model", "labels.txt"));
            _imagesFolder = rootImagesFolder;
            _model = null;
        }

        public string ImagesFolder => _imagesFolder;

        public ImagePredictedLabelWithProbability Score(string imageName)
        {
            if (_model == null)
            {
                Init();
            }
            var predictions = PredictDataUsingModel(imageName);
            return predictions;
        }

        public void Init()
        {
            _model = CreatePredictionFunction();
        }


        private PredictionFunction<ImageInputData, ImageNetPrediction> CreatePredictionFunction()
        {
            try
            {
                var pipeline = ImageEstimatorsCatalog.LoadImages(catalog: _mlContext.Transforms, imageFolder: _imagesFolder, columns: ("ImagePath", "ImageReal"))
                                .Append(ImageEstimatorsCatalog.Resize(_mlContext.Transforms, "ImageReal", "ImageReal", ImageNetSettings.imageHeight, ImageNetSettings.imageWidth))
                                .Append(ImageEstimatorsCatalog.ExtractPixels(_mlContext.Transforms, new[] { new ImagePixelExtractorTransform.ColumnInfo("ImageReal", InceptionSettings.InputTensorName, interleave: ImageNetSettings.channelsLast, offset: ImageNetSettings.mean) }))
                                .Append(new TensorFlowEstimator(_mlContext, _modelLocation, new[] { InceptionSettings.InputTensorName }, new[] { InceptionSettings.OutputTensorName }));
                var model = pipeline.Fit(CreateDataView());
                var predictionFunction = model.MakePredictionFunction<ImageInputData, ImageNetPrediction>(_mlContext);
                return predictionFunction;

            }
            catch (Exception e)
            {
                throw e;
            }
        }


        private IDataView CreateDataView()
        {
            //Create empty DataView. We just need the schema to call fit()
            var list = new List<ImageInputData>
            {
                new ImageInputData() { ImagePath = "" }
            };
            IEnumerable<ImageInputData> enumerableData = list;
            var dv = _mlContext.CreateStreamingDataView(enumerableData);
            return dv;
        }

        private ImagePredictedLabelWithProbability PredictDataUsingModel(string imageName)
        {

            var inputImage = new ImageInputData { ImagePath = imageName };
            var image1Probabilities = _model.Predict(inputImage).PredictedLabels;

            //Set a single label as predicted or even none if probabilities were lower than 70%
            var bestLabelPrediction = new ImagePredictedLabelWithProbability()
            {
                ImagePath = inputImage.ImagePath,
            };
            (bestLabelPrediction.PredictedLabel, bestLabelPrediction.Probability) = ModelHelpers.GetBestLabel(_labels, image1Probabilities);

            return bestLabelPrediction;
        }
    }
}
