using Microsoft.AspNetCore.Mvc;
using Amazon.S3;
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        
        [HttpPost]
        public async Task<IActionResult> Index(FileUploadFormModel FileUpload)
        {
            if(FileUpload.FormFile == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                string bucketName = "oisinsapps";
                string fileKey = "temp";

                using (var memoryStream = new MemoryStream())
                {
                    await FileUpload.FormFile.CopyToAsync(memoryStream);

                    if (memoryStream.Length < 1000000)
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

                List<string> extractedText = textractClient.GetLineText();
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
        }

        public IActionResult CreatePdf(string output)
        {
            if(output == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                DocumentCreatorClient docClient = new DocumentCreatorClient();
                byte[] stream = docClient.GeneratePdf(output);

                return File(stream, "application/pdf", "ExtractedText.pdf");
            }
           
        }

        public IActionResult CreateTxt(string output)
        {
            if(output == null)
            {
                //return RedirectToAction("Index");
                return NoContent();
            }
            else
            {
                string wwwPath = this.Environment.WebRootPath;
                DocumentCreatorClient docClient = new DocumentCreatorClient();
                docClient.GenerateTxt(output);
                byte[] stream = System.IO.File.ReadAllBytes(wwwPath + "\\ExtractedText.txt");
                return File(stream, "text/plain", "ExtractedText.txt");
            }
        }

        public IActionResult Records()
        {
            AppDbContext appDbContext = new AppDbContext();
            DocumentRepository repo = new DocumentRepository(appDbContext);
            return View(repo.AllDocs);
        }

        //[HttpPost,ActionName("DeleteRecord")]
        public IActionResult DeleteRecord(int docId)
        {
            AppDbContext appDbContext = new AppDbContext();
            DocumentRepository repo = new DocumentRepository(appDbContext);
            UserDocument userDocument = repo.GetDocById(docId);
            appDbContext.Documents.Remove(userDocument);
            appDbContext.SaveChanges();
            return RedirectToAction("Records");
        }

        [HttpPost]
        public IActionResult DbCommitForm(IFormCollection collection)
        {
            AppDbContext appDbContext = new AppDbContext();
            DocumentRepository repository = new DocumentRepository(appDbContext);   
            UserDocument userDocument = new UserDocument();
            userDocument.DocumentTitle = collection["textTitle"];
            userDocument.DocumentBody = collection["textBody"];

            if(userDocument.DocumentTitle != "" && userDocument.DocumentBody != "")
            {
                repository.AddDoc(userDocument);

                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index");
            }
            
        }

      
    }

    
}