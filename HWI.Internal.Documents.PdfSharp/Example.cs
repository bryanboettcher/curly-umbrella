using System;
using System.Collections.Generic;
using System.Text;

namespace HWI.Internal.Documents.PdfSharp
{
    class Example
    {
        public void DefaultUsage()
        {
            IPdfSplitter constructorInjection = new PdfSharpPdfSplitter();

            constructorInjection
                .SplitFile(@"\\app02\HWIApps\StericycleRecall\Hotfolder\1533756205-Output.pdf")
                .ToFolder(@"\\app02\HWIApps\StericycleRecall\Hotfolder\Test\")
                .WithPagesPerFile(500)
                .WithFilenameGenerator(PdfFilenameGenerator.UnderscoreSeparated("SteriCycleRecall_10249"))
                .WithProgress((p, t) => Console.WriteLine("Generating page {0} of {1}", p, t))
                .Start();

        }
    }
}
