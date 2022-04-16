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

        private IWebHostEnvironment Environment;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment _environment)
        {
            _logger = logger;
            Environment = _environment;
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

            List<string>extractedText = textractClient.GetLineText();
            string result = "";

            foreach (string s in extractedText)
            {
                result += s + "\n";
            }
            ViewData["result"] = result;

            UserDocument userDocument = new UserDocument();
            userDocument.DocumentBody = result;

            ViewData["userDocument"] = userDocument;
            
            return View();
        }

        public IActionResult CreatePdf(string output)
        {
 
            DocumentCreatorClient docClient = new DocumentCreatorClient();
            byte[] stream = docClient.GeneratePdf(output);

            return File(stream, "application/pdf", "ExtractedText.pdf");

        }

        public IActionResult CreateTxt(string output)
        {
            string wwwPath = this.Environment.WebRootPath;
            DocumentCreatorClient docClient = new DocumentCreatorClient();
            docClient.GenerateTxt(output);
            byte[] stream = System.IO.File.ReadAllBytes(wwwPath + "\\ExtractedText.txt");

            return File(stream, "text/plain", "ExtractedText.txt");
        }

        [HttpPost]
        public void DbCommit(string Title, string Body)
        {
            AppDbContext appDbContext = new AppDbContext();
            DocumentRepository repo = new DocumentRepository(appDbContext);

            UserDocument userDocument = new UserDocument();

            userDocument.DocumentTitle = Title;
            userDocument.DocumentBody = Body;

            repo.AddDoc(userDocument);

        }

        public IActionResult Records()
        {
            AppDbContext dbContext = new AppDbContext();
            DocumentRepository repo = new DocumentRepository(dbContext);
            return View(repo.AllDocs);
        }

        [HttpPost,ActionName("DeleteRecord")]
        public IActionResult Record(int docId)
        {
            AppDbContext dbContext = new AppDbContext();
            DocumentRepository repo = new DocumentRepository(dbContext);
            UserDocument userDocument = repo.GetDocById(docId);
            dbContext.Documents.Remove(userDocument);
            dbContext.SaveChanges();
            return View();
        }

    }

    
}