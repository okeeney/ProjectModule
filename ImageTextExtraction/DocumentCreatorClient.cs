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

            iText.Layout.Element.Paragraph body = new iText.Layout.Element.Paragraph(text)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(12);

            document.Add(body);
            document.Close();

            return stream.ToArray();
        }

        public void GenerateTxt(string text)
        {
            //var doc = new WordDocument();
            //var builder = new DocumentBuilder(doc);
            //builder.Write(text);
            //doc.Save("wwwroot\\ExtractedText.docx");

            string fileName = @"wwwroot\\ExtractedText.txt";

            try
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }

                using (FileStream fs = File.Create(fileName))
                {
                    byte[] textBody = new UTF8Encoding(true).GetBytes(text);
                    fs.Write(textBody, 0, textBody.Length);
                }
            } 
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        

    }
}
