using System;
using System.Threading.Tasks;
using Marcom;

namespace HWI.Internal.Services.FusionPro
{
    public static class Launcher
    {
        public static async Task GenerateTemplate(string templateFile, string templateName, string groupName)
        {
            var request = new TemplateAddRequest
            {
                FilePath = templateFile,
                GroupName = groupName,
                TemplateName = templateName
            };

            var client = new FPQueueWCFServiceClient();
            var response = await client.AddTemplateFromFileAsync(request);

            if (!string.IsNullOrEmpty(response.Message))
                throw new InvalidOperationException(response.Message);
        }

        // Launch a job with the specified input file, template and group, to the specified output file. It is hardcoded to PDF. 
        // Optionally wait until the job is complete before returning.
        public static async Task<string> ComposeJob(string templateName, string groupName, string inputFilePath, string outputFolder, bool waitUntilFinished)
        {
            var request = new CompositionRequest
            {
                GroupName = groupName,
                TemplateName = templateName,
                //OnDemand = true,
                Options = new JobOptions
                {
                    OutputFormat = OutputFormat.PDF,
                    UseImposition = true,
                    OutputFolder = outputFolder
                }
            };

            var client = new FPQueueWCFServiceClient();

            /* Code below is optional to output every record to its own file. 
            request.Options.NamedSettings = new KeyValue[]
            {
                new KeyValue { Name = "OutputSource", Value = "bychunk" },
                new KeyValue { Name = "RecordsPerChunk", Value = settings.RecordsPerFile.ToString()},
                new KeyValue { Name = "ReducedRecordsForPreview", Value = "6" }, // whatever you want here
            };  */

            var resp = await client.CreateCompositionSessionAsync(request);

            if (!string.IsNullOrEmpty(resp.Message))
                throw new CompositionSessionFailedException(resp.Message);

            if (!string.IsNullOrEmpty(inputFilePath))
            {
                var compReq = new AddCompositionComponentRequest
                {
                    Type = FileType.InputData,
                    RemoteFilePath = inputFilePath,
                    CompositionID = resp.CompositionID,
                    ComponentType = FPCompositionComponentType.FromFile
                };

                var addResp = await client.AddCompositionFileAsync(compReq);
                if (!string.IsNullOrEmpty(addResp.Message))
                    throw new CompositionFailedException(resp.CompositionID, addResp.Message);
            }

            var startReq = new StartCompositionRequest
            {
                CompositionID = resp.CompositionID
            };

            var startResp = await client.StartCompositionFromSessionAsync(startReq);
            if (!string.IsNullOrEmpty(startResp.Message))
                throw new CompositionFailedException(resp.CompositionID, startResp.Message);

            if (!waitUntilFinished) return resp.CompositionID.ToString(); // no error

            while (true)
            {
                var compFilePathReq = new CompositionFilePathURLRequest
                {
                    CompositionID = resp.CompositionID
                };

                var status = await client.CheckCompositionStatusAsync(compFilePathReq);
                //if (!string.IsNullOrEmpty(status.Message))
                //    throw new CompositionStatusFailedException(resp.CompositionID, status.Message);

                switch (status.JobStatus)
                {
                    case JobStatus.Cancelled:
                    case JobStatus.Failed:
                    case JobStatus.None:
                        throw new CompositionJobFailedException(resp.CompositionID,
                            status.JobStatus.ToString(),
                            status.Message);

                    case JobStatus.Queueing:
                    case JobStatus.InProcess:
                        //continue;
                        // Here you can check status.PercentComplete for updates.
                        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
                        break;

                    case JobStatus.Done:
                        if (status.ReturnCode != 0)
                            throw new CompositionJobFailedException(resp.CompositionID,
                                "ReturnCode - " + status.ReturnCode,
                                status.Message);

                        return resp.CompositionID.ToString();
                }
            }
        }
    }
}