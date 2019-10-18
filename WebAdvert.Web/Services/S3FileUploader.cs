using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Configuration;
using PutObjectRequest = Amazon.S3.Model.PutObjectRequest;

namespace WebAdvert.Web.Services
{
    

    public class S3FileUploader:IFileUploader
    {
        private readonly IConfiguration _configuration;
        private const string bucketName = "webadvertsimgs";
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.USEast2;
        private static IAmazonS3 _s3Client;

        public S3FileUploader(IConfiguration configuration)
        {
            _configuration = configuration;
            _s3Client = new AmazonS3Client(bucketRegion);
            

        }
        public async Task<bool> UploadFileAsync(string fileName, Stream storageStream)
        {
            if (string.IsNullOrEmpty(fileName)) throw new ArgumentException("File name must be specified.");

            var fileTransferUtility =
                new TransferUtility(_s3Client);

            using (var client = new AmazonS3Client(bucketRegion))
            {
                if (storageStream.Length > 0)
                    if (storageStream.CanSeek)
                        storageStream.Seek(0, SeekOrigin.Begin);

                var request = new PutObjectRequest
                {
                    AutoCloseStream = true,
                    BucketName = bucketName,
                    InputStream = storageStream,
                    Key = fileName
                };
                var response = await client.PutObjectAsync(request).ConfigureAwait(false);
                return response.HttpStatusCode == HttpStatusCode.OK;
            }
        }
    }
}
