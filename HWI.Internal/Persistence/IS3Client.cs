namespace HWI.Internal.Persistence
{
    public interface IS3Client
    {
        void Download(string bucket, string filename, string localFilename);
        void Upload(string bucket, string filename, string localFilename);
    }
}