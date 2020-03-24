using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using System;

namespace FileExplorer.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SasController : ControllerBase
    {
        public string ConnString { get; set; }
        public SasController(IConfiguration conf)
        {
            ConnString = conf["BlobStorageConnectionString"];
        }

        [HttpGet]
        public string Get(string fileUri)
        {
            var account = CloudStorageAccount.Parse(ConnString);
            var blobClient = account.CreateCloudBlobClient();
            var blob = blobClient.GetBlobReferenceFromServer(new Uri(fileUri));

            SharedAccessBlobPolicy adHocSAS = new SharedAccessBlobPolicy()
            {
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(2),
                Permissions = SharedAccessBlobPermissions.Read,
            };
            var token = blob.GetSharedAccessSignature(adHocSAS);
            
            var tkn = token.Remove(0, 1);
            var connstring = $@"BlobEndpoint={account.BlobEndpoint};SharedAccessSignature={tkn}";
            return connstring;
        }
    }
}