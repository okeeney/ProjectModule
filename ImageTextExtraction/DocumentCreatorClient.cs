using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System.Diagnostics;
using System.IO;
using Aspose.Words;
using Document = iText.Layout.Document;
using WordDocument = Aspose.Words.Document;
using System.Runtime.Serialization.Formatters.Binary;

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

            iText.Layout.Element.Paragraph body = new iText.Layout.Element.Paragraph(text)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(12);

            document.Add(body);
            document.Close();

            return stream.ToArray();
        }

        public void GenerateDocx(string text)
        {
            var doc = new WordDocument();
            var builder = new DocumentBuilder(doc);
            builder.Write(text);
            doc.Save("wwwroot\\ExtractedText.docx");

        }

        

    }
}
