using System.Threading;
using Amazon.S3;
using Amazon.S3.Model;

namespace HWI.Internal.Persistence.Aws
{
    public abstract class AwsS3Client
    {
        public void Download(string bucket, string filename, string localFilename)
        {
            var client = GetS3Client();
            
            var request = new GetObjectRequest
            {
                BucketName = bucket,
                Key = filename
            };

            var response = client.GetObjectAsync(request).Result;
            
            response.WriteResponseStreamToFileAsync(localFilename, false, CancellationToken.None).Wait();
        }

        public void Upload(string bucket, string filename, string localFilename)
        {
            var client = GetS3Client();

            var request = new PutObjectRequest
            {
                BucketName = bucket,
                Key = filename,
                FilePath = localFilename
            };

            client.PutObjectAsync(request).Wait();
        }

        protected abstract AmazonS3Client GetS3Client();
    }
}