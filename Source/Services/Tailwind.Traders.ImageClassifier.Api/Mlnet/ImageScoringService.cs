using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Data.IO;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms.Image;
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
        private PredictionEngine<ImageInputData, ImageNetPrediction> _model;
        private static string LabelTokey = nameof(LabelTokey);

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

        public ImagePredictedLabelWithProbability Score(ImageInputData imageName)
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

        private PredictionEngine<ImageInputData, ImageNetPrediction> CreatePredictionFunction()
        {
            try
            {
   
               var pipeline = _mlContext.Transforms.ResizeImages(outputColumnName: TensorFlowModelSettings.inputTensorName, imageWidth: ImageSettings.imageWidth, imageHeight: ImageSettings.imageHeight, inputColumnName: nameof(ImageInputData.Image))
               .Append(_mlContext.Transforms.ExtractPixels(outputColumnName: TensorFlowModelSettings.inputTensorName, interleavePixelColors: ImageSettings.channelsLast, offsetImage: ImageSettings.mean))
               .Append(_mlContext.Model.LoadTensorFlowModel(_modelLocation).
               ScoreTensorFlowModel(outputColumnNames: new[] { TensorFlowModelSettings.outputTensorName },
                                   inputColumnNames: new[] { TensorFlowModelSettings.inputTensorName }, addBatchDimensionInput: false));

                ITransformer model = pipeline.Fit(CreateEmptyDataView());

                return _mlContext.Model.CreatePredictionEngine<ImageInputData, ImageNetPrediction>(model);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private IDataView CreateEmptyDataView()
        {
            //Create empty DataView. We just need the schema to call fit()
            List<ImageInputData> list = new List<ImageInputData>();
            list.Add(new ImageInputData() { Image = new System.Drawing.Bitmap(ImageSettings.imageWidth, ImageSettings.imageHeight) }); //Test: Might not need to create the Bitmap.. = null; ?

            var dv = _mlContext.Data.LoadFromEnumerable(list);
            return dv;
        }

        private ImagePredictedLabelWithProbability PredictDataUsingModel(ImageInputData imageName)
        {
            var image1Probabilities = _model.Predict(imageName).PredictedLabels;

            var bestLabelPrediction = new ImagePredictedLabelWithProbability();
   
            (bestLabelPrediction.PredictedLabel, bestLabelPrediction.Probability) = ModelHelpers.GetBestLabel(_labels, image1Probabilities);

            return bestLabelPrediction;
        }
    }
}
