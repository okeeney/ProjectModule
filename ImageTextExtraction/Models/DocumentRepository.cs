namespace ImageTextExtraction.Models
{
    public class DocumentRepository
    {
        private readonly AppDbContext _appDbContext;

        public DocumentRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public IEnumerable<UserDocument> AllDocs    
        {
            get { return _appDbContext.Documents; }
        }

        public UserDocument GetDocById(int docId)
        {
            return _appDbContext.Documents.FirstOrDefault(d => d.DocumentId == docId);
        }

        public void AddDoc(UserDocument userDocument)
        {
            _appDbContext.Documents.Add(userDocument);
            _appDbContext.SaveChanges();
        }

        public void DeleteDoc(UserDocument userDocument)
        {
            _appDbContext.Documents.Remove(userDocument);
        }
    }
}
