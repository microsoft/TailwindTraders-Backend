using System.Net;

namespace Tailwind.Traders.MobileBff.Models
{
    public class ClassificationResult
    {
        public string Label { get; set; }
        public decimal Probability { get; set; }

        public bool IsOk { get; }
        public HttpStatusCode Code { get; }

        public ClassificationResult()
        {
            IsOk = true;
        }

        private ClassificationResult(HttpStatusCode code)
        {
            Code = code;
            IsOk = false;
        }

        public static ClassificationResult InvalidResult(HttpStatusCode code)
        {
            return new ClassificationResult(code);
        }
    }

    public class ClassifiedProductItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public string ImageUrl { get; set; }
    }
}
