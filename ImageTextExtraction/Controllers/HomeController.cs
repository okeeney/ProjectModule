using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
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

   

        private IAmazonS3 GetIAmazonS3Client()
        {
            return new AmazonS3Client();
        }

        public IActionResult Index()
        {
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

            //Instantiate Textract local client
            TextractClient textractClient = new TextractClient();
            //Begin extraction
            await textractClient.StartDetectAsync();

            List<string>extractedText = textractClient.getLineText();
            string result = "";

            foreach (string s in extractedText)
            {
                result += s + "\n";
            }
            ViewData["result"] = result;
            
            return View();
        }

        public IActionResult CreatePdf(string output)
        {
 
            DocumentCreatorClient docClient = new DocumentCreatorClient();
            byte[] stream = docClient.GeneratePdf(output);

            return File(stream, "application/pdf");

        }

        public IActionResult CreateDocx(string output)
        {
            DocumentCreatorClient docClient = new DocumentCreatorClient();
            byte[] stream = docClient.GenerateDocx(output);

            return File(stream, "application/msword");
        }



       
    }

    
}