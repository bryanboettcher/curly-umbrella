using System.IO;

namespace HWI.Internal.Documents
{
    public class PdfFilenameGenerator : IPdfFilenameGenerator
    {
        private readonly string _format;

        private PdfFilenameGenerator(string format)
        {
            _format = format;
        }

        public static IPdfFilenameGenerator Simple()
        {
            return new PdfFilenameGenerator("%originalFilename% - %currentPage%.pdf");
        }

        public static IPdfFilenameGenerator UnderscoreSeparated(string baseFilename)
        {
            return new PdfFilenameGenerator(Path.GetFileNameWithoutExtension(baseFilename) + "_%currentPage%_of_%totalPages%.pdf");
        }

        public string BuildFilename(int currentPage, int totalPages, int pagesPerDocument, string originalFilename)
        {
            return _format
                .Replace("%currentPage%", currentPage.ToString())
                .Replace("%totalPages%", totalPages.ToString())
                .Replace("%pagesPerDocument%", pagesPerDocument.ToString())
                .Replace("%originalFilename%", originalFilename);
        }
    }
}