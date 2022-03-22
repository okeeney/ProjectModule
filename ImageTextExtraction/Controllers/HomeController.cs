using Microsoft.AspNetCore.Mvc;
using POCTest.Models;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using ImageTextExtraction.Models;

namespace ImageTextExtraction.Controllers
{
    public class HomeController : Controller
    {

        // GET: HomeController

        private readonly ILogger<HomeController> _logger;



        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }


        //public static void UploadObject(string filePath, string url)
        //{
        //    HttpWebRequest httpRequest = WebRequest.Create(url) as HttpWebRequest;
        //    httpRequest.Method = "PUT";
        //    using (Stream dataStream = httpRequest.GetRequestStream())
        //    {
        //        var buffer = new byte[8000];
        //        using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        //        {
        //            int bytesRead = 0;
        //            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
        //            {
        //                dataStream.Write(buffer, 0, bytesRead);
        //            }
        //        }
        //    }
        //    HttpWebResponse response = httpRequest.GetResponse() as HttpWebResponse;
        //}

        public string GeneratePreSignedURL(string bucketName, string objectKey, double duration)
        {
            using var s3Client = GetIAmazonS3Client();

            var request = new GetPreSignedUrlRequest
            {
                BucketName = bucketName,
                Key = objectKey,
                Protocol = Protocol.HTTPS,
                Verb = HttpVerb.PUT,
                Expires = DateTime.UtcNow.AddHours(duration)
            };

            string url = s3Client.GetPreSignedURL(request);
            return url;
        }

        private IAmazonS3 GetIAmazonS3Client()
        {
            // Future updates to this library should provide a constructor that allows region, another allowing region & credentials to be provided.
            return new AmazonS3Client();
        }

        public IActionResult Index()
        {
            string bucketName = "oisinsapps";
            string objectKey = "userImage";
            double duration = 1.0;
            string signedURL = GeneratePreSignedURL(bucketName, objectKey, duration);
            ViewData["signedURL"] = signedURL;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> Index(FileUploadFormModel FileUpload)
        {
            List<String> extractedText = new List<String>();
            string bucketName = "oisinsapps";
            string fileKey = "temp";
            using (var memoryStream = new MemoryStream())
            {
                await FileUpload.FormFile.CopyToAsync(memoryStream);

                if(memoryStream.Length < 10000000)
                {
                    await S3Upload.UploadFileAsync(memoryStream, bucketName, fileKey);
                }
                else
                {
                    ModelState.AddModelError("File", "The file is too large.");
                }

            }
            TextractClient textractClient = new TextractClient();
            await textractClient.StartDetectAsync();
            extractedText = textractClient.getBlockText();
            string result = "";
            foreach(string s in extractedText)
            {
                result += s + " ";
            }

            ViewData["result"] = result;

            return View();
        }

       
    }

    
}