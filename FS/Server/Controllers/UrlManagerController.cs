using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using System;

using System.IO;
using System.Threading.Tasks;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;

namespace FileExplorer.Server.Controllers
{
    [ApiController]
    
    public class UrlManagerController : ControllerBase
    {
        private readonly string _storageAccountName;
        private readonly StorageSharedKeyCredential _sharedKeyCredential;
        
       public UrlManagerController(IConfiguration conf)
        {
            _storageAccountName = conf["BlobStorageAccountName"];
            var storageAccountKey1 = conf["BlobStorageAccountKey1"];

            _sharedKeyCredential = new StorageSharedKeyCredential(_storageAccountName, storageAccountKey1);
        }

        [HttpGet]
        [Route("[controller]/get-upload-url/{blobName}")]
        public string GetURLForUpload([FromRoute]string blobName)
        {
            
            var containerName = GetContainerName();

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerName,
                BlobName = blobName,
                Protocol = SasProtocol.Https,
                ExpiresOn = new DateTimeOffset(DateTime.UtcNow.AddDays(1)),
                Resource = "b" // Blob
            };
            sasBuilder.SetPermissions(BlobSasPermissions.Write);

            var sasQueryParams = sasBuilder.ToSasQueryParameters(_sharedKeyCredential);

            var fullUri = new UriBuilder()
            {
                Scheme = "https",
                Host = $"{_storageAccountName}.blob.core.windows.net",
                Path = $"{containerName}/{blobName}",
                Query = sasQueryParams.ToString(),
            };

            return fullUri.ToString();
        }

        private string GetContainerName() => "usercontainer";
    }
}