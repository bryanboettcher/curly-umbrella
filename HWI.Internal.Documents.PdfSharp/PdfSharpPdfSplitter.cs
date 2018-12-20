using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace HWI.Internal.Documents.PdfSharp
{
    public class PdfSharpPdfSplitter : IPdfSplitter, IPdfSplitWithSource, IPdfSplitToDestination, IPdfSplitOptions
    {
        private string _sourceFile;
        private string _destinationFolder;
        private int _pageSplitThreshold;
        private bool _doReversePages;
        
        private IPdfFilenameGenerator _filenameGenerator = PdfFilenameGenerator.Simple();

        private Action<int, int> _progressCallback = null;
        private Action<PdfDocumentStatus> _completeCallback = null;

        public IPdfSplitWithSource SplitFile(string sourceFile)
        {
            _sourceFile = sourceFile;
            return this;
        }

        public IPdfSplitToDestination ToFolder(string destinationFolder)
        {
            _destinationFolder = destinationFolder;
            return this;
        }

        public IPdfSplitOptions WithPagesPerFile(int pages)
        {
            _pageSplitThreshold = pages;
            return this;
        }

        public IPdfSplitOptions Reverse()
        {
            _doReversePages = true;
            return this;
        }

        public IPdfSplitOptions WithFilenameGenerator(IPdfFilenameGenerator pdfFilenameGenerator)
        {
            _filenameGenerator = pdfFilenameGenerator;
            return this;
        }

        public IPdfSplitOptions WithProgress(Action<int, int> pagesOfTotal)
        {
            _progressCallback = pagesOfTotal;
            return this;
        }

        public IPdfSplitOptions WhenComplete(Action<PdfDocumentStatus> documentStatus)
        {
            _completeCallback = documentStatus;
            return this;
        }

        public void Start()
        {
            var originalFilename = Path.GetFileNameWithoutExtension(_sourceFile);

            var sourcePdf = PdfReader.Open(_sourceFile, PdfDocumentOpenMode.Import);

            var totalPages = sourcePdf.PageCount;
            _progressCallback?.Invoke(0, totalPages);

            var outputPdf = new PdfDocument { Version = sourcePdf.Version };

            var chunkList = new List<string>();
            var numOfChunks = 0;
            for (var currentPage = 0; currentPage < totalPages; currentPage++)
            {
                var index = _doReversePages ? (totalPages - 1) - currentPage : currentPage;
                var page = sourcePdf.Pages[index];
                outputPdf.AddPage(page);

                _progressCallback?.Invoke(currentPage, totalPages);

                if (outputPdf.PageCount < _pageSplitThreshold) continue;

                var newFileName = Path.Combine(_destinationFolder,
                    _filenameGenerator.BuildFilename(currentPage, totalPages, _pageSplitThreshold, originalFilename));
                outputPdf.Save(newFileName);

                chunkList.Add(newFileName);
                numOfChunks++;
                outputPdf = new PdfDocument { Version = sourcePdf.Version };
            }

            if (outputPdf.PageCount > 0)
            {
                outputPdf.Save(Path.Combine(
                    _destinationFolder, 
                    _filenameGenerator.BuildFilename(
                        numOfChunks * _pageSplitThreshold, 
                        totalPages, 
                        _pageSplitThreshold, 
                        originalFilename)
                    )
                );
            }

            var status = new PdfDocumentStatus
            {
                ListOfChunks = chunkList,
                NumberOfChucks = numOfChunks
            };
            _completeCallback?.Invoke(status);
        }

        public Task StartAsync()
        {
            return Task.Run(() => Start());
        }

    }
}

