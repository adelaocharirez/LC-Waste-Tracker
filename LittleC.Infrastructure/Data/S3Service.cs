using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Configuration;

namespace LittleC.Infrastructure.Data
{
    public class S3Service
    {
        private readonly string _bucketName;
        private readonly IAmazonS3 _s3Client;

        public S3Service(IConfiguration config)
        {
            _bucketName = config["AWS:BucketName"]!;
            _s3Client = new AmazonS3Client(
                config["AWS:AccessKey"],
                config["AWS:SecretKey"],
                RegionEndpoint.GetBySystemName(config["AWS:Region"])
            );
        }

        public async Task<string> UploadPhotoAsync(Stream fileStream, string fileName, string contentType)
        {
            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = fileStream,
                Key = $"shift-photos/{DateTime.UtcNow:yyyy/MM/dd}/{fileName}",
                BucketName = _bucketName,
                ContentType = contentType,
                CannedACL = S3CannedACL.PublicRead
            };

            var transferUtility = new TransferUtility(_s3Client);
            await transferUtility.UploadAsync(uploadRequest);

            return $"https://{_bucketName}.s3.amazonaws.com/{uploadRequest.Key}";
        }
    }
}