using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Properties;
using System.Text;

namespace ImageTextExtraction
{
    public class DocumentCreatorClient
    {
        
        public byte[] GeneratePdf(string text)
        {
            var stream = new MemoryStream();
            var writer = new PdfWriter(stream);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            iText.Layout.Element.Paragraph body = new iText.Layout.Element.Paragraph(text);

            document.Add(body);
            document.Close();

            return stream.ToArray();
        }
    }
}
