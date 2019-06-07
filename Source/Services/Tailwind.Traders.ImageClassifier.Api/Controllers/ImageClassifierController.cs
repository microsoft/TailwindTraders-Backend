using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Drawing;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Tailwind.Traders.ImageClassifier.Api.Dtos;
using Tailwind.Traders.ImageClassifier.Api.Mlnet.ImageDataStructures;

namespace Tailwind.Traders.ImageClassifier.Api.Controllers
{
    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class ImageClassifierController : ControllerBase
    {
        private readonly ILogger<ImageClassifierController> _logger;
        private readonly IImageScoringService _scoringSvc;
        public ImageClassifierController(ILogger<ImageClassifierController> logger, IImageScoringService scoringSvc)
        {
            _logger = logger;
            _scoringSvc = scoringSvc;
        }

        // POST v1/imageclassifier
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Post(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest();
            }

            var fileName = $"{Guid.NewGuid()}_{ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"')}";
            var fullPath = Path.Combine(_scoringSvc.ImagesFolder, fileName);

            try
            {

                MemoryStream imageMemoryStream = new MemoryStream();
                await file.CopyToAsync(imageMemoryStream);

                //Convert to Bitmap
                Bitmap bitmapImage = (Bitmap)Image.FromStream(imageMemoryStream);

                //Set the specific image data into the ImageInputData type used in the DataView
                ImageInputData imageInputData = new ImageInputData { Image = bitmapImage };

                _logger.LogInformation($"Start processing image file { fullPath }");

                var scoring = _scoringSvc.Score(imageInputData);

                _logger.LogInformation($"Image processed");

                return Ok(ClassificationResponse.CreateFrom(scoring));
            }
            finally
            {
                try
                {
                    _logger.LogInformation($"Deleting Image {fullPath}");
                    System.IO.File.Delete(fullPath);
                }
                catch (Exception)
                {
                    _logger.LogInformation("Error deleting image: " + fileName);
                }
            }
        }
    }
}

