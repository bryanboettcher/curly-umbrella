using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HWI.Internal.Documents
{
    public interface IPdfSplitter
    {
        IPdfSplitWithSource SplitFile(string sourceFile);
    }

    public interface IPdfSplitWithSource
    {
        IPdfSplitToDestination ToFolder(string destinationFolder);
    }

    public interface IPdfSplitToDestination
    {
        IPdfSplitOptions WithPagesPerFile(int pages);
    }

    public interface IPdfSplitOptions
    {
        IPdfSplitOptions WithFilenameGenerator(IPdfFilenameGenerator pdfFilenameGenerator);
        IPdfSplitOptions WithProgress(Action<int, int> pagesOfTotal);
        IPdfSplitOptions WhenComplete(Action<PdfDocumentStatus> documentStatus);
        IPdfSplitOptions Reverse();
        void Start();
        Task StartAsync();
    }

    public class PdfDocumentStatus
    {
        public IEnumerable<string> ListOfChunks { get; set; }
        public int NumberOfChucks { get; set; }
    }

    public interface IPdfFilenameGenerator
    {
        string BuildFilename(int currentPage, int totalPages, int pagesPerDocument, string originalFilename);
    }

}