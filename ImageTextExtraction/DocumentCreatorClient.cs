using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System.IO;


namespace ImageTextExtraction
{
    public class DocumentCreatorClient
    {
        
        public Document ExportToPDF(string text)
        {
            
            PdfWriter writer = new PdfWriter("c:\\Output.pdf");
            PdfDocument pdf = new PdfDocument(writer);
            Document document = new Document(pdf);

            Paragraph body = new Paragraph(text)
                .SetTextAlignment(TextAlignment.JUSTIFIED)
                .SetFontSize(12);

            document.Add(body);
            document.Close();

            return document;
        }

        public byte[] GeneratePdf(string text)
        {
            var stream = new MemoryStream();
            var writer = new PdfWriter(stream);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            document.Add(new Paragraph(text));
            document.Close();

            return stream.ToArray();
        }

       

        

    }
}
